// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWorkforceMetricsRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.WorkforceMetrics
{
    /// <summary>
    /// The GetWorkforceMetricsRequest.
    /// </summary>
    public class GetWorkforceMetricsRequest : PagedRequest<Dictionary<string, object>>
    {
        /// <summary>
        /// Gets or sets the workforce metric collection id.
        /// </summary>
        public Guid WorkforceMetricCollectionId { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateOnly EndDate { get; set; }
    }
}