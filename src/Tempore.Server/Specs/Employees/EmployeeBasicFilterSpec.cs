// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeBasicFilterSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.Employees
{
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Extensions;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The employee by spec.
    /// </summary>
    public class EmployeeBasicFilterSpec : Specification<Employee>
    {
        /// <summary>
        /// The search text.
        /// </summary>
        private readonly string? searchText;

        /// <summary>
        /// The flag include shifts.
        /// </summary>
        private readonly bool includeShifts;

        /// <summary>
        /// The flag include shift assignment.
        /// </summary>
        private readonly bool includeShiftAssignment;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeBasicFilterSpec"/> class.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        /// <param name="paginationOptions">
        /// The paginationOptions.
        /// </param>
        /// <param name="includeShifts">
        /// The include shifts.
        /// </param>
        /// <param name="includeShiftAssignment">
        /// The include shift assignment.
        /// </param>
        public EmployeeBasicFilterSpec(string? searchText, bool? includeShifts, bool? includeShiftAssignment, PaginationOptions paginationOptions)
            : base(paginationOptions)
        {
            this.searchText = searchText;
            this.includeShifts = includeShifts ?? false;
            this.includeShiftAssignment = includeShiftAssignment ?? false;
        }

        /// <inheritdoc />
        protected override Func<IQueryable<Employee>, IQueryable<Employee>> BuildSpec()
        {
            //// TODO: ILike is postgres specific function. Avoid use this here.
            return entities =>
            {
                return entities
                .Where(x => EF.Functions.ILike(x.FullName, $"%{this.searchText}%") || EF.Functions.ILike(x.ExternalId, $"%{this.searchText}%"), !string.IsNullOrWhiteSpace(this.searchText))
                .Include(e => e.ScheduledShifts, this.includeShifts)
                .Include(e => e.ScheduledShiftAssignments, this.includeShiftAssignment);
            };
        }
    }
}