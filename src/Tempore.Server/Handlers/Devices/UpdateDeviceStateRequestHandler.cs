// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateDeviceStateRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Devices
{
    using System.Threading;
    using System.Threading.Tasks;

    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.Requests.Devices;
    using Tempore.Server.Specs.Devices;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The update device state request handler.
    /// </summary>
    public class UpdateDeviceStateRequestHandler : IRequestHandler<UpdateDeviceStateRequest>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UpdateDeviceStateRequestHandler> logger;

        /// <summary>
        /// The device repository.
        /// </summary>
        private readonly IRepository<Device, ApplicationDbContext> deviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDeviceStateRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="deviceRepository">
        /// The device repository.
        /// </param>
        public UpdateDeviceStateRequestHandler(ILogger<UpdateDeviceStateRequestHandler> logger, IRepository<Device, ApplicationDbContext> deviceRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceRepository);

            this.logger = logger;
            this.deviceRepository = deviceRepository;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Handle(UpdateDeviceStateRequest request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation(
                "Updating device {DeviceId} state to {DeviceState}",
                request.DeviceId,
                request.DeviceState);

            var device = await this.deviceRepository.SingleOrDefaultAsync(new DeviceByIdSpec(request.DeviceId));
            if (device is null)
            {
                this.logger.LogWarning("Device {DeviceId} not found.", request.DeviceId);

                return;
            }

            device.State = request.DeviceState;
            this.deviceRepository.Update(device);
            await this.deviceRepository.SaveChangesAsync();

            this.logger.LogInformation(
                "Updated device {DeviceId} state to {DeviceState}",
                request.DeviceId,
                request.DeviceState);
        }
    }
}