// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadEmployeesFromDevicesRequestHandler.cs" company="Port Hope Investment S.A.">
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
    /// The upload employees request handler.
    /// </summary>
    public class UploadEmployeesFromDevicesRequestHandler : IRequestHandler<UploadEmployeesFromDevicesRequest>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UploadEmployeesFromDevicesRequestHandler> logger;

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
        /// Initializes a new instance of the <see cref="UploadEmployeesFromDevicesRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="hubContext">
        /// The hub context.
        /// </param>
        /// <param name="hubLifetimeManager">
        /// The hub life manager.
        /// </param>
        /// <param name="deviceRepository">
        /// The device repository.
        /// </param>
        public UploadEmployeesFromDevicesRequestHandler(
            ILogger<UploadEmployeesFromDevicesRequestHandler> logger,
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
        public async Task Handle(UploadEmployeesFromDevicesRequest request, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            foreach (var deviceId in request.DeviceIds)
            {
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
                tasks.Add(agentReceiver.UploadEmployeesAsync(device.Id));
            }

            await Task.WhenAll(tasks);
        }
    }
}