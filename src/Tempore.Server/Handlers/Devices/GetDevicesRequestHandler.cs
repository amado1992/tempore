// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetDevicesRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Devices
{
    using System.Threading;
    using System.Threading.Tasks;

    using Mapster;

    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.Devices;
    using Tempore.Server.Specs;
    using Tempore.Server.Specs.Devices;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The devices by agent id request handler.
    /// </summary>
    public class GetDevicesRequestHandler : IRequestHandler<GetDevicesRequest, List<DeviceDto>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetDevicesRequestHandler> logger;

        /// <summary>
        /// The agent repository.
        /// </summary>
        private readonly IRepository<Device, ApplicationDbContext> deviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDevicesRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="deviceRepository">
        /// The device repository.
        /// </param>
        public GetDevicesRequestHandler(ILogger<GetDevicesRequestHandler> logger, IRepository<Device, ApplicationDbContext> deviceRepository)
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
        public Task<List<DeviceDto>> Handle(GetDevicesRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return Task.FromResult(this.deviceRepository.All().Adapt<List<DeviceDto>>());
        }
    }
}