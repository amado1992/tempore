// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmployeesFromScheduledShiftRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Shifts
{
    using System.Threading;
    using System.Threading.Tasks;

    using Mapster;

    using MediatR;

    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.Shifts;
    using Tempore.Server.Specs;
    using Tempore.Server.Specs.ShiftAssignment;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The get employees from shift assignments request handler.
    /// </summary>
    public class GetEmployeesFromScheduledShiftRequestHandler : IRequestHandler<GetEmployeesFromScheduledShiftRequest,
        PagedResponse<ScheduledShiftEmployeeDto>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetEmployeesFromScheduledShiftRequestHandler> logger;

        /// <summary>
        /// The employee repository.
        /// </summary>
        private readonly IRepository<Employee, ApplicationDbContext> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmployeesFromScheduledShiftRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="repository">
        /// The device repository.
        /// </param>
        public GetEmployeesFromScheduledShiftRequestHandler(
            ILogger<GetEmployeesFromScheduledShiftRequestHandler> logger,
            IRepository<Employee, ApplicationDbContext> repository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(repository);

            this.logger = logger;
            this.repository = repository;
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
        public async Task<PagedResponse<ScheduledShiftEmployeeDto>> Handle(GetEmployeesFromScheduledShiftRequest request, CancellationToken cancellationToken)
        {
            var paginationOptions = new PaginationOptions(request.Skip, request.Take, false);
            var employeesFromShiftAssignmentsSpec = new EmployeesFromScheduledShiftSpec(
                request.ScheduledShiftId,
                request.SearchParams,
                paginationOptions);

            var count = await this.repository.CountAsync(employeesFromShiftAssignmentsSpec);
            if (count == 0)
            {
                return new PagedResponse<ScheduledShiftEmployeeDto>(0, Enumerable.Empty<ScheduledShiftEmployeeDto>());
            }

            paginationOptions.IsEnable = true;
            var employeeFromShiftAssignments = await this.repository.FindAsync(employeesFromShiftAssignmentsSpec);
            return new PagedResponse<ScheduledShiftEmployeeDto>(count, employeeFromShiftAssignments.Adapt<List<ScheduledShiftEmployeeDto>>());
        }
    }
}