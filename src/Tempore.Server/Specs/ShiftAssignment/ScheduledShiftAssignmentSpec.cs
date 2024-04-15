// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledShiftAssignmentSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.ShiftAssignment
{
    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The shift assignment spec.
    /// </summary>
    public class ScheduledShiftAssignmentSpec : ISpecification<ScheduledShiftAssignment>
    {
        /// <summary>
        /// The today.
        /// </summary>
        private readonly DateOnly today;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledShiftAssignmentSpec"/> class.
        /// </summary>
        /// <param name="today">
        /// The today.
        /// </param>
        public ScheduledShiftAssignmentSpec(DateOnly today)
        {
            this.today = today;
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="Func{IQueryable}"/>.
        /// </returns>
        public Func<IQueryable<ScheduledShiftAssignment>, IQueryable<ScheduledShiftAssignment>> Build()
        {
            return assignments =>
                assignments
                    .Include(assignment => assignment.ScheduledShift)
                    .ThenInclude(scheduledShift => scheduledShift.Shift)
                    .ThenInclude(shift => shift.Days)
                    .ThenInclude(day => day.Timetable)
                    .Where(assignment => assignment.ScheduledShift.ExpireDate != null && this.today <= assignment.ScheduledShift.ExpireDate)
                    .OrderBy(assignment => assignment.ScheduledShift.ShiftId)
                    .ThenBy(assignment => assignment.EmployeeId);
        }
    }
}