// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteWorkforceMetricConflictResolutionRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.ScheduledDay
{
    using MediatR;

    /// <summary>
    /// The DeleteWorkforceMetricConflictResolutionRequest.
    /// </summary>
    public class DeleteWorkforceMetricConflictResolutionRequest : IRequest
    {
        /// <summary>
        /// Gets or sets the WorkforceMetricConflictResolution Id.
        /// </summary>
        public Guid Id { get; set; }
    }
}