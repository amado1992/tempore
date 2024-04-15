// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmployeesFromScheduledShiftRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Shifts
{
    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests.Shifts.SearchParams;

    /// <summary>
    /// The get employees from scheduled shift request.
    /// </summary>
    public class GetEmployeesFromScheduledShiftRequest : PagedRequest<EmployeesFromScheduledShiftSearchParams, ScheduledShiftEmployeeDto>
    {
        /// <summary>
        /// Gets or sets the scheduled shift id.
        /// </summary>
        public Guid? ScheduledShiftId { get; set; }
    }
}