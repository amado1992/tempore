// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateWorkforceMetricConflictResolutionRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.ScheduledDay
{
    using MediatR;

    /// <summary>
    /// The UpdateWorkforceMetricConflictResolutionRequest.
    /// </summary>
    public class UpdateWorkforceMetricConflictResolutionRequest : IRequest<Guid>
    {
        /// <summary>
        /// Gets or sets the WorkforceMetricConflictResolutionRequest Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the workforce metric id.
        /// </summary>
        public Guid WorkforceMetricId { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string? Comment { get; set; }
    }
}