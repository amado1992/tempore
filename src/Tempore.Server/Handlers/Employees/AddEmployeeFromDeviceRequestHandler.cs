// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddEmployeeFromDeviceRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Employees
{
    using System.Threading;
    using System.Threading.Tasks;

    using Mapster;

    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests.Employees;
    using Tempore.Server.Specs.EmployeeFromDevice;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The employee from device request handler.
    /// </summary>
    public class AddEmployeeFromDeviceRequestHandler : IRequestHandler<AddEmployeeFromDeviceRequest, Guid>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AddEmployeeFromDeviceRequestHandler> logger;

        /// <summary>
        /// The device repository.
        /// </summary>
        private readonly IRepository<EmployeeFromDevice, ApplicationDbContext> employeeFromDeviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEmployeeFromDeviceRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="employeeFromDeviceRepository">
        /// The device repository.
        /// </param>
        public AddEmployeeFromDeviceRequestHandler(
            ILogger<AddEmployeeFromDeviceRequestHandler> logger,
            IRepository<EmployeeFromDevice, ApplicationDbContext> employeeFromDeviceRepository)
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
        public async Task<Guid> Handle(AddEmployeeFromDeviceRequest request, CancellationToken cancellationToken)
        {
            var employeeFromDevice = await this.employeeFromDeviceRepository.SingleOrDefaultAsync(new EmployeeFromDeviceByDeviceIdAndEmployeeIdOnDeviceSpec(request.Employee.DeviceId, request.Employee.EmployeeIdOnDevice));
            if (employeeFromDevice is null)
            {
                var deviceAdapterConfig = TypeAdapterConfig<EmployeeFromDeviceDto, EmployeeFromDevice>.NewConfig().Config;
                employeeFromDevice = request.Employee.Adapt<EmployeeFromDevice>(deviceAdapterConfig);
                this.employeeFromDeviceRepository.Add(employeeFromDevice);
            }
            else
            {
                var deviceAdapterConfig = TypeAdapterConfig<EmployeeFromDeviceDto, EmployeeFromDevice>.NewConfig()
                    .Ignore(device => device.Id)
                    .Ignore(device => device.EmployeeId)
                    .IgnoreNullValues(true)
                    .Config;

                request.Employee.Adapt(employeeFromDevice, deviceAdapterConfig);

                this.employeeFromDeviceRepository.Update(employeeFromDevice);
            }

            await this.employeeFromDeviceRepository.SaveChangesAsync();

            return employeeFromDevice.Id;
        }
    }
}