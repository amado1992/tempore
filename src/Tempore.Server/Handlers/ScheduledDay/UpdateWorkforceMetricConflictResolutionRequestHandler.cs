// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateWorkforceMetricConflictResolutionRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.ScheduledDay
{
    using MediatR;

    using Tempore.Server.Requests.ScheduledDay;

    /// <summary>
    /// The update workforce metric conflict resolution request handler.
    /// </summary>
    public class UpdateWorkforceMetricConflictResolutionRequestHandler : IRequestHandler<UpdateWorkforceMetricConflictResolutionRequest, Guid>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UpdateWorkforceMetricConflictResolutionRequest> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateWorkforceMetricConflictResolutionRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UpdateWorkforceMetricConflictResolutionRequestHandler(ILogger<UpdateWorkforceMetricConflictResolutionRequest> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            this.logger = logger;
        }

        /// <inheritdoc />
        public Task<Guid> Handle(UpdateWorkforceMetricConflictResolutionRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            return Task.FromException<Guid>(new NotImplementedException());
        }
    }
}