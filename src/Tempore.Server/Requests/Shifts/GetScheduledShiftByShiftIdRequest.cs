// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetScheduledShiftByShiftIdRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Shifts
{
    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.Shifts.SearchParams;

    /// <summary>
    /// The scheduled shift request.
    /// </summary>
    public class GetScheduledShiftByShiftIdRequest : PagedRequest<ScheduledShiftSearchParams, ScheduledShiftOverviewDto>
    {
        /// <summary>
        /// Gets or sets the shift id.
        /// </summary>
        public Guid ShiftId { get; set; }
    }
}