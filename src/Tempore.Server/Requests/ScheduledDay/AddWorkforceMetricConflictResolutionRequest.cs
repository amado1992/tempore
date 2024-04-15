// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddWorkforceMetricConflictResolutionRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.ScheduledDay
{
    using MediatR;

    /// <summary>
    /// The AddWorkforceMetricConflictResolutionRequest.
    /// </summary>
    public class AddWorkforceMetricConflictResolutionRequest : IRequest<Guid>
    {
        /// <summary>
        /// Gets or sets the ScheduledDayId.
        /// </summary>
        public Guid? ScheduledDayId { get; set; }

        /// <summary>
        /// Gets or sets the WorkforceMetricId.
        /// </summary>
        public Guid WorkforceMetricId { get; set; }

        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the Comment.
        /// </summary>
        public string? Comment { get; set; }
    }
}