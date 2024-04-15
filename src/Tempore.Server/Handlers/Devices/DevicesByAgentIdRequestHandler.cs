// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DevicesByAgentIdRequestHandler.cs" company="Port Hope Investment S.A.">
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
    public class DevicesByAgentIdRequestHandler : IRequestHandler<DevicesByAgentIdRequest, PagedResponse<DeviceDto>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DevicesByAgentIdRequestHandler> logger;

        /// <summary>
        /// The agent repository.
        /// </summary>
        private readonly IRepository<Device, ApplicationDbContext> deviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicesByAgentIdRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="deviceRepository">
        /// The device repository.
        /// </param>
        public DevicesByAgentIdRequestHandler(ILogger<DevicesByAgentIdRequestHandler> logger, IRepository<Device, ApplicationDbContext> deviceRepository)
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
        public async Task<PagedResponse<DeviceDto>> Handle(DevicesByAgentIdRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var specification = new DevicesByAgentIdSpec(request.AgentId, new PaginationOptions(request.Skip, request.Take, false));
            var count = await this.deviceRepository.CountAsync(specification);
            if (count == 0)
            {
                return new PagedResponse<DeviceDto>(0, Array.Empty<DeviceDto>());
            }

            specification.PaginationOptions.IsEnable = true;
            var devices = await this.deviceRepository.FindAsync(specification);
            var items = devices?.Adapt<List<DeviceDto>>();
            return new PagedResponse<DeviceDto>(count, items);
        }
    }
}