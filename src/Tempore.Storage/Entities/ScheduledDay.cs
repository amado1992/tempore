// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledDay.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The scheduled day.
    /// </summary>
    public class ScheduledDay
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Gets or sets the timestamps.
        /// </summary>
        public List<Timestamp> Timestamps { get; set; }

        /// <summary>
        /// Gets or sets the work force metric conflict resolutions.
        /// </summary>
        public List<WorkforceMetricConflictResolution> WorkforceMetricConflictResolutions { get; set; }

        /// <summary>
        /// Gets or sets the shift assignment id.
        /// </summary>
        public Guid ScheduledShiftAssignmentId { get; set; }

        /// <summary>
        /// Gets or sets the scheduled shiftAssignment.
        /// </summary>
        public ScheduledShiftAssignment ScheduledShiftAssignment { get; set; }

        /// <summary>
        /// Gets or sets the day id.
        /// </summary>
        public Guid DayId { get; set; }

        /// <summary>
        /// Gets or sets the days.
        /// </summary>
        public Day Day { get; set; }

        /// <summary>
        /// Gets or sets the start date time.
        /// </summary>
        public DateTimeOffset? StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the end date time.
        /// </summary>
        public DateTimeOffset? EndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the check in start date time.
        /// </summary>
        public DateTimeOffset? CheckInStartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the check in end date time.
        /// </summary>
        public DateTimeOffset? CheckInEndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the valid check out start date time .
        /// </summary>
        public DateTimeOffset? CheckOutStartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the valid check out end date time.
        /// </summary>
        public DateTimeOffset? CheckOutEndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the effective working time.
        /// </summary>
        public TimeSpan EffectiveWorkingTime { get; set; }

        /// <summary>
        /// Gets or sets the relative effective working time.
        /// </summary>
        public TimeSpan RelativeEffectiveWorkingTime { get; set; }
    }
}