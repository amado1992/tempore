// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmployeesFromDevicesRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Agents
{
    using System.Threading;
    using System.Threading.Tasks;

    using Mapster;

    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.Employees;
    using Tempore.Server.Specs;
    using Tempore.Server.Specs.EmployeeFromDevice;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The devices by agent id request handler.
    /// </summary>
    public class GetEmployeesFromDevicesRequestHandler : IRequestHandler<GetEmployeesFromDevicesRequest, PagedResponse<EmployeeFromDeviceDto>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetEmployeesFromDevicesRequestHandler> logger;

        /// <summary>
        /// The employee repository.
        /// </summary>
        private readonly IRepository<EmployeeFromDevice, ApplicationDbContext> employeeFromDeviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmployeesFromDevicesRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="employeeFromDeviceRepository">
        /// The device repository.
        /// </param>
        public GetEmployeesFromDevicesRequestHandler(ILogger<GetEmployeesFromDevicesRequestHandler> logger, IRepository<EmployeeFromDevice, ApplicationDbContext> employeeFromDeviceRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(employeeFromDeviceRepository);

            this.logger = logger;
            this.employeeFromDeviceRepository = employeeFromDeviceRepository;
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
        public async Task<PagedResponse<EmployeeFromDeviceDto>> Handle(GetEmployeesFromDevicesRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var specification = new EmployeeFromDeviceBasicFilterSpec(request.SearchText, request.IsLinked, request.IncludeDevice, request.IncludeAgent, request.EmployeeId, request.DeviceIds, new PaginationOptions(request.Skip, request.Take, false));
            var count = await this.employeeFromDeviceRepository.CountAsync(specification);
            if (count == 0)
            {
                return new PagedResponse<EmployeeFromDeviceDto>(0, Array.Empty<EmployeeFromDeviceDto>());
            }

            specification.PaginationOptions.IsEnable = true;
            var employeesFromDevices = await this.employeeFromDeviceRepository.FindAsync(specification);

            var items = employeesFromDevices.Adapt<List<EmployeeFromDeviceDto>>();
            return new PagedResponse<EmployeeFromDeviceDto>(count, items);
        }
    }
}