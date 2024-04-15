// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeesFromScheduledShiftSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.ShiftAssignment
{
    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Extensions;

    using Tempore.Server.Requests.Shifts.SearchParams;
    using Tempore.Server.Specs;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The employees from scheduled shift spec.
    /// </summary>
    public class EmployeesFromScheduledShiftSpec : Specification<Employee, ScheduledShiftEmployee>
    {
        private readonly Guid? scheduledShiftId;

        private readonly EmployeesFromScheduledShiftSearchParams? searchParams;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeesFromScheduledShiftSpec"/> class.
        /// </summary>
        /// <param name="scheduledShiftId">
        /// The scheduled shift id.
        /// </param>
        /// <param name="searchParams">
        /// The search params.
        /// </param>
        /// <param name="paginationOptions">
        /// The pagination options.
        /// </param>
        public EmployeesFromScheduledShiftSpec(
            Guid? scheduledShiftId,
            EmployeesFromScheduledShiftSearchParams? searchParams,
            PaginationOptions paginationOptions)
            : base(paginationOptions)
        {
            this.scheduledShiftId = scheduledShiftId;
            this.searchParams = searchParams;
        }

        /// <summary>
        /// The build spec.
        /// </summary>
        /// <returns>
        /// The <see cref="Func{IQueryable, IQueryable}"/>.
        /// </returns>
        protected override Func<IQueryable<Employee>, IQueryable<ScheduledShiftEmployee>> BuildSpec()
        {
            return employees => employees.Include(employee => employee.ScheduledShifts)
                .Where(
                    employee => EF.Functions.ILike(employee.FullName, $"%{this.searchParams!.SearchText}%"),
                    !string.IsNullOrWhiteSpace(this.searchParams?.SearchText))
                .Where(
                    employee => (employee.ScheduledShifts.Count(scheduledShift => scheduledShift.Id == this.scheduledShiftId) > 0) == this.searchParams!.IsAssigned,
                    this.searchParams?.IsAssigned is not null)
                .Select(employee => new ScheduledShiftEmployee
                {
                    Id = employee.Id,
                    FullName = employee.FullName,
                    IsAssigned = employee.ScheduledShifts.Count(scheduledShift => scheduledShift.Id == this.scheduledShiftId) > 0,
                });
        }
    }
}