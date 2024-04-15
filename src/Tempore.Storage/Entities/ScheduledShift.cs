// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledShift.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The scheduled shift.
    /// </summary>
    public class ScheduledShift
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the shift id.
        /// </summary>
        public Guid ShiftId { get; set; }

        /// <summary>
        /// Gets or sets the shift.
        /// </summary>
        public Shift Shift { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Gets or sets the expire date.
        /// </summary>
        public DateOnly ExpireDate { get; set; }

        /// <summary>
        /// Gets or sets the effective working time.
        /// </summary>
        public TimeSpan EffectiveWorkingTime { get; set; }

        /// <summary>
        /// Gets or sets the scheduled shift assignments.
        /// </summary>
        public List<ScheduledShiftAssignment> ScheduledShiftAssignments { get; set; }

        /// <summary>
        /// Gets or sets the employees.
        /// </summary>
        public List<Employee> Employees { get; set; }
    }
}