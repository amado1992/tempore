// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTimestampsFromDevicesRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.AgentCommands
{
    using System.Threading;
    using System.Threading.Tasks;

    using MediatR;

    using Microsoft.AspNetCore.SignalR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Client.Services.Interfaces;
    using Tempore.Server.Hubs;
    using Tempore.Server.Requests.DeviceCommands;
    using Tempore.Server.Services.Interfaces;
    using Tempore.Server.Specs.Devices;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The upload employees timestamps request handler.
    /// </summary>
    public class UploadTimestampsFromDevicesRequestHandler : IRequestHandler<Requests.DeviceCommands.UploadTimestampsFromDevicesRequest>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UploadTimestampsFromDevicesRequestHandler> logger;

        /// <summary>
        /// The hub context.
        /// </summary>
        private readonly IHubContext<AgentHub, IAgentReceiver> hubContext;

        /// <summary>
        /// The Hub lifetime manager.
        /// </summary>
        private readonly IHubLifetimeManager<AgentHub> hubLifetimeManager;

        /// <summary>
        /// The device repository.
        /// </summary>
        private readonly IRepository<Device, ApplicationDbContext> deviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadTimestampsFromDevicesRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="hubContext">
        /// The hubContext.
        /// </param>
        /// <param name="hubLifetimeManager">
        /// The hub context.
        /// </param>
        /// <param name="deviceRepository">
        /// The device repository.
        /// </param>
        public UploadTimestampsFromDevicesRequestHandler(
            ILogger<UploadTimestampsFromDevicesRequestHandler> logger,
            IHubContext<AgentHub, IAgentReceiver> hubContext,
            IHubLifetimeManager<AgentHub> hubLifetimeManager,
            IRepository<Device, ApplicationDbContext> deviceRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(hubContext);
            ArgumentNullException.ThrowIfNull(hubLifetimeManager);
            ArgumentNullException.ThrowIfNull(deviceRepository);

            this.logger = logger;
            this.hubContext = hubContext;
            this.hubLifetimeManager = hubLifetimeManager;
            this.deviceRepository = deviceRepository;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="request">
        /// The push employees from device request.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Handle(UploadTimestampsFromDevicesRequest request, CancellationToken cancellationToken)
        {
            // TODO: Move this in an invokable?
            var tasks = new List<Task>();
            foreach (var deviceId in request.DeviceIds)
            {
                // TODO: Send notifications errors notifications via signalR?
                var device = await this.deviceRepository.SingleOrDefaultAsync(new DeviceByIdSpec(deviceId, true));
                if (device is null)
                {
                    this.logger.LogWarning("Device {DeviceId} not found.", deviceId);
                    continue;
                }

                if (!this.hubLifetimeManager.IsAlive(device.Agent.ConnectionId))
                {
                    this.logger.LogWarning("Connection to agent {AgentId} is not alive.", device.AgentId);
                    continue;
                }

                var agentReceiver = this.hubContext.Clients.Client(device.Agent.ConnectionId!);
                tasks.Add(Task.Run(
                    async () =>
                    {
                        try
                        {
                            await agentReceiver.UploadEmployeesTimestampsAsync(
                                device.Id,
                                request.From,
                                request.To);
                        }
                        catch (Exception ex)
                        {
                            this.logger.LogError(ex, "Error occurred sending a command to agent {AgentName}", device.Agent);
                        }
                    },
                    CancellationToken.None));
            }

            await Task.WhenAll(tasks);
        }
    }
}