// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmployeeFromDeviceByIdRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Employees
{
    using Mapster;

    using MediatR;

    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.Employees;
    using Tempore.Server.Specs;
    using Tempore.Server.Specs.EmployeeFromDevice;
    using Tempore.Server.Specs.Employees;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The employee from device request handler.
    /// </summary>
    public class GetEmployeeFromDeviceByIdRequestHandler : IRequestHandler<GetEmployeeFromDeviceByIdRequest, EmployeeFromDeviceDto>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetEmployeeFromDeviceByIdRequestHandler> logger;

        /// <summary>
        /// The employee from device repository.
        /// </summary>
        private readonly IRepository<EmployeeFromDevice, ApplicationDbContext> employeeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmployeeFromDeviceByIdRequestHandler"/> class.
        /// </summary>
        public GetEmployeeFromDeviceByIdRequestHandler(ILogger<GetEmployeeFromDeviceByIdRequestHandler> logger, IRepository<EmployeeFromDevice, ApplicationDbContext> employeeRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(employeeRepository);

            this.logger = logger;
            this.employeeRepository = employeeRepository;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation Token.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<EmployeeFromDeviceDto> Handle(GetEmployeeFromDeviceByIdRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var specification = new EmployeeFromDeviceByIdSpec(request.Id);
            var employee = await this.employeeRepository.SingleOrDefaultAsync(specification);

            if (employee is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Employee from device with Id '{request.Id}' not found");
            }

            var employeeDto = employee.Adapt<EmployeeFromDeviceDto>();
            return employeeDto;
        }
    }
}