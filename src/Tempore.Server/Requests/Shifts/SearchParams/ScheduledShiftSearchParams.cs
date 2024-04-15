// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledShiftSearchParams.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Shifts.SearchParams
{
    /// <summary>
    /// The scheduled shift search params.
    /// </summary>
    public class ScheduledShiftSearchParams
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateOnly? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the expire date.
        /// </summary>
        public DateOnly? ExpireDate { get; set; }

        /// <summary>
        /// Gets or sets the effective working time.
        /// </summary>
        public TimeSpan? EffectiveWorkingTime { get; set; }
    }
}