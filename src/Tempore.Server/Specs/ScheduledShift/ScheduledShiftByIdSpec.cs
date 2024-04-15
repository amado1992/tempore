// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledShiftByIdSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.ScheduledShift
{
    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The ScheduledShiftByIdSpec.
    /// </summary>
    public class ScheduledShiftByIdSpec : ISpecification<ScheduledShift>
    {
        /// <summary>
        /// The scheduled shift id.
        /// </summary>
        private readonly Guid scheduledShiftId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledShiftByIdSpec"/> class.
        /// </summary>
        /// <param name="scheduledShiftId">
        /// The scheduled shift id.
        /// </param>
        public ScheduledShiftByIdSpec(Guid scheduledShiftId)
        {
            this.scheduledShiftId = scheduledShiftId;
        }

        /// <inheritdoc />
        public Func<IQueryable<ScheduledShift>, IQueryable<ScheduledShift>> Build()
        {
            return scheduledShifts => scheduledShifts
                .Include(scheduledShift => scheduledShift.ScheduledShiftAssignments)
                .Include(scheduledShift => scheduledShift.Shift)
                .ThenInclude(shift => shift.Days)
                .ThenInclude(day => day.Timetable)
                .Where(scheduledShift => scheduledShift.Id == this.scheduledShiftId);
        }
    }
}