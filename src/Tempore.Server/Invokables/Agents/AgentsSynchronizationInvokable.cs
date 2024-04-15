// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentsSynchronizationInvokable.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Invokables.Agents
{
    using System.Threading;
    using System.Threading.Tasks;

    using Coravel.Invocable;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Client.Services.Interfaces;
    using Tempore.Server.Hubs;
    using Tempore.Server.Services.Interfaces;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The AgentsSynchronizationInvokable class.
    /// </summary>
    public class AgentsSynchronizationInvokable : IInvocable, ICancellableInvocable
    {
        private readonly ILogger<AgentsSynchronizationInvokable> logger;

        private readonly IHubContext<AgentHub, IAgentReceiver> agentHubContext;

        private readonly IHubLifetimeManager<AgentHub> hubLifetimeManager;

        private readonly IServiceScopeFactory scopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentsSynchronizationInvokable"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="agentHubContext">
        /// The agent hub.
        /// </param>
        /// <param name="hubLifetimeManager">
        /// The hub life time manager.
        /// </param>
        /// <param name="scopeFactory">
        /// The scope factory.
        /// </param>
        public AgentsSynchronizationInvokable(
            ILogger<AgentsSynchronizationInvokable> logger,
            IHubContext<AgentHub, IAgentReceiver> agentHubContext,
            IHubLifetimeManager<AgentHub> hubLifetimeManager,
            IServiceScopeFactory scopeFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(agentHubContext);
            ArgumentNullException.ThrowIfNull(hubLifetimeManager);
            ArgumentNullException.ThrowIfNull(scopeFactory);

            this.logger = logger;
            this.agentHubContext = agentHubContext;
            this.hubLifetimeManager = hubLifetimeManager;
            this.scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Gets or sets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Invokes the synchronization.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Invoke()
        {
            try
            {
                using var serviceScope = this.scopeFactory.CreateScope();
                var unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork<ApplicationDbContext>>();
                var agentRepository = unitOfWork.GetRepository<Agent>();

                var agents = await agentRepository.All().Include(agent => agent.Devices).ToListAsync();
                foreach (var agent in agents)
                {
                    this.logger.LogInformation("Checking Agent '{AgentName}' state", agent.Name);

                    var connectionId = agent.ConnectionId;
                    if (!this.hubLifetimeManager.IsAlive(connectionId))
                    {
                        agent.ConnectionId = null;
                        agent.State = AgentState.Offline;

                        this.logger.LogInformation("Agent '{AgentName}' is '{AgentState}'", agent.Name, AgentState.Offline);

                        foreach (var agentDevice in agent.Devices)
                        {
                            agentDevice.State = DeviceState.Offline;

                            this.logger.LogInformation("Device '{DeviceName}' of Agent '{AgentName}' is '{DeviceState}'", agentDevice.Name, agent.Name, DeviceState.Offline);
                        }

                        await agentRepository.SaveChangesAsync();
                    }
                    else
                    {
                        agent.State = AgentState.Online;

                        this.logger.LogInformation("Agent '{AgentName}' is '{AgentState}'", agent.Name, AgentState.Online);

                        await agentRepository.SaveChangesAsync();

                        var agentDevices = agent.Devices.ToList();
                        foreach (var agentDevice in agentDevices)
                        {
                            this.logger.LogInformation("Requesting for device '{DeviceName}' state of Agent '{AgentName}' ", agentDevice.Name, agent.Name);

                            await this.agentHubContext.Clients.Client(connectionId!).ReportDeviceStateAsync(agentDevice.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error synchronizing agents states");
            }
        }
    }
}