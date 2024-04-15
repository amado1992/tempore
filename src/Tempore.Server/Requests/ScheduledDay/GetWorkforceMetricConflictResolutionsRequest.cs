// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWorkforceMetricConflictResolutionsRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.ScheduledDay
{
    using Tempore.Server.DataTransferObjects;

    /// <summary>
    /// The GetWorkforceMetricConflictResolutionsRequest.
    /// </summary>
    public class GetWorkforceMetricConflictResolutionsRequest : PagedRequest<WorkforceMetricConflictResolutionDto>
    {
        /// <summary>
        /// Gets or sets the ScheduledDayId.
        /// </summary>
        public Guid ScheduledDayId { get; set; }
    }
}