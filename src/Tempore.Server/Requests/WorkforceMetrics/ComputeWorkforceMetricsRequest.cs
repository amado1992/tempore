// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComputeWorkforceMetricsRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.WorkforceMetrics
{
    using MediatR;

    /// <summary>
    /// The compute metric request.
    /// </summary>
    public class ComputeWorkforceMetricsRequest : IRequest<Guid>
    {
        /// <summary>
        /// Gets or sets or set the start date.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Gets or sets or set the end date.
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Gets or sets the work force metric collection id.
        /// </summary>
        public List<Guid> WorkForceMetricCollectionIds { get; set; }
    }
}