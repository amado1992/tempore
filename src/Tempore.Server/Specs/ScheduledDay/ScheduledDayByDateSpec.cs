// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledDayByDateSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.ScheduledDay
{
    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The ScheduledDayByDateSpec.
    /// </summary>
    public class ScheduledDayByDateSpec : ISpecification<ScheduledDay>
    {
        /// <summary>
        /// The scheduled shift assignment id.
        /// </summary>
        private readonly Guid scheduledShiftAssignmentId;

        /// <summary>
        /// The current date.
        /// </summary>
        private readonly DateOnly currentDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledDayByDateSpec"/> class.
        /// </summary>
        /// <param name="scheduledShiftAssignmentId">
        /// The shift assignment id.
        /// </param>
        /// <param name="currentDate">
        /// The current date.
        /// </param>
        public ScheduledDayByDateSpec(Guid scheduledShiftAssignmentId, DateOnly currentDate)
        {
            this.scheduledShiftAssignmentId = scheduledShiftAssignmentId;
            this.currentDate = currentDate;
        }

        /// <inheritdoc />
        public Func<IQueryable<ScheduledDay>, IQueryable<ScheduledDay>> Build()
        {
            return scheduledDays => scheduledDays
               .Where(day => day.ScheduledShiftAssignmentId == this.scheduledShiftAssignmentId && day.Date == this.currentDate);
        }
    }
}