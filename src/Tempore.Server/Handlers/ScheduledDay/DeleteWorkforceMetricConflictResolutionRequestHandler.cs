// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteWorkforceMetricConflictResolutionRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.ScheduledDay
{
    using MediatR;

    using Tempore.Server.Requests.ScheduledDay;

    /// <summary>
    /// The delete workforce metric conflict resolution request handler.
    /// </summary>
    public class DeleteWorkforceMetricConflictResolutionRequestHandler : IRequestHandler<DeleteWorkforceMetricConflictResolutionRequest>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DeleteWorkforceMetricConflictResolutionRequestHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteWorkforceMetricConflictResolutionRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DeleteWorkforceMetricConflictResolutionRequestHandler(ILogger<DeleteWorkforceMetricConflictResolutionRequestHandler> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            this.logger = logger;
        }

        /// <inheritdoc />
        public Task Handle(DeleteWorkforceMetricConflictResolutionRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            return Task.FromException(new NotImplementedException());
        }
    }
}