// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmployeeByIdRequestHandler.cs" company="Port Hope Investment S.A.">
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
    using Tempore.Server.Specs.Employees;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The employee request handler.
    /// </summary>
    public class GetEmployeeByIdRequestHandler : IRequestHandler<GetEmployeeByIdRequest, EmployeeDto>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetEmployeeByIdRequestHandler> logger;

        /// <summary>
        /// The employee repository.
        /// </summary>
        private readonly IRepository<Employee, ApplicationDbContext> employeeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmployeeByIdRequestHandler"/> class.
        /// </summary>
        public GetEmployeeByIdRequestHandler(ILogger<GetEmployeeByIdRequestHandler> logger, IRepository<Employee, ApplicationDbContext> employeeRepository)
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
        public async Task<EmployeeDto> Handle(GetEmployeeByIdRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var specification = new EmployeeByIdSpec(request.Id);
            var employee = await this.employeeRepository.SingleOrDefaultAsync(specification);

            if (employee is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Employee with Id '{request.Id}' not found");
            }

            var employeeDto = employee.Adapt<EmployeeDto>();
            return employeeDto;
        }
    }
}