// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledShiftAssignment.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The scheduled shift assignment.
    /// </summary>
    public class ScheduledShiftAssignment
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the employee.
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// Gets or sets the shift id.
        /// </summary>
        public Guid ScheduledShiftId { get; set; }

        /// <summary>
        /// Gets or sets the shift.
        /// </summary>
        public ScheduledShift ScheduledShift { get; set; }

        /// <summary>
        /// Gets or sets the scheduled days.
        /// </summary>
        public List<ScheduledDay> ScheduledDays { get; set; }

        /// <summary>
        /// Gets or sets the last generated day date.
        /// </summary>
        public DateOnly? LastGeneratedDayDate { get; set; }
    }
}