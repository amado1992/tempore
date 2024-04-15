// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledShiftOverview.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    /// <summary>
    /// The scheduled shift overview.
    /// </summary>
    public class ScheduledShiftOverview
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the shift id.
        /// </summary>
        public Guid ShiftId { get; set; }

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
        /// Gets or sets the employees count.
        /// </summary>
        public int EmployeesCount { get; set; }
    }
}