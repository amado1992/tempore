// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeesFromScheduledShiftSearchParams.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Shifts.SearchParams
{
    /// <summary>
    /// The EmployeesFromScheduledShiftSearchParams.
    /// </summary>
    public class EmployeesFromScheduledShiftSearchParams
    {
        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        public string? SearchText { get; set; }

        /// <summary>
        /// Gets or sets the is assigned.
        /// </summary>
        public bool? IsAssigned { get; set; }
    }
}