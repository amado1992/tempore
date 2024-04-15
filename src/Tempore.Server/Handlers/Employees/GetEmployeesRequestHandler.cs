// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmployeesRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Employees
{
    using Mapster;

    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.Employees;
    using Tempore.Server.Specs;
    using Tempore.Server.Specs.Employees;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The list employees request handler.
    /// </summary>
    public class GetEmployeesRequestHandler : IRequestHandler<GetEmployeesRequest, PagedResponse<EmployeeDto>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetEmployeesRequestHandler> logger;

        /// <summary>
        /// The employee repository.
        /// </summary>
        private readonly IRepository<Employee, ApplicationDbContext> employeeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmployeesRequestHandler"/> class.
        /// </summary>
        public GetEmployeesRequestHandler(ILogger<GetEmployeesRequestHandler> logger, IRepository<Employee, ApplicationDbContext> employeeRepository)
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
        public async Task<PagedResponse<EmployeeDto>> Handle(GetEmployeesRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var employeesSpec = new EmployeeBasicFilterSpec(request.SearchText, request.IncludeShifts, request.IncludeShiftAssignment, new PaginationOptions(request.Skip, request.Take, false));
            var count = await this.employeeRepository.CountAsync(employeesSpec);
            if (count == 0)
            {
                return new PagedResponse<EmployeeDto>(0, Array.Empty<EmployeeDto>());
            }

            employeesSpec.PaginationOptions.IsEnable = true;
            var employees = await this.employeeRepository.FindAsync(employeesSpec);

            var items = employees?.Adapt<List<EmployeeDto>>();
            return new PagedResponse<EmployeeDto>(count, items);
        }
    }
}