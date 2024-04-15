// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmployeesRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Employees
{
    using Tempore.Server.DataTransferObjects;

    /// <summary>
    /// The employees request.
    /// </summary>
    public class GetEmployeesRequest : PagedRequest<EmployeeDto>
    {
        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        public string? SearchText { get; set; }

        /// <summary>
        /// Gets or sets the is include the shifts.
        /// </summary>
        public bool? IncludeShifts { get; set; }

        /// <summary>
        /// Gets or sets the is include the shift assignment.
        /// </summary>
        public bool? IncludeShiftAssignment { get; set; }
    }
}