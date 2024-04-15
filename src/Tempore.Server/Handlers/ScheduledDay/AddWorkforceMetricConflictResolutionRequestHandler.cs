// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddWorkforceMetricConflictResolutionRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.ScheduledDay
{
    using MediatR;

    using Tempore.Server.Requests.ScheduledDay;

    /// <summary>
    /// The add workforce metric conflict resolution request handler.
    /// </summary>
    public class AddWorkforceMetricConflictResolutionRequestHandler : IRequestHandler<AddWorkforceMetricConflictResolutionRequest, Guid>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AddWorkforceMetricConflictResolutionRequestHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddWorkforceMetricConflictResolutionRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AddWorkforceMetricConflictResolutionRequestHandler(ILogger<AddWorkforceMetricConflictResolutionRequestHandler> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            this.logger = logger;
        }

        /// <inheritdoc />
        public Task<Guid> Handle(AddWorkforceMetricConflictResolutionRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            return Task.FromException<Guid>(new NotImplementedException());
        }
    }
}