// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWorkforceMetricConflictResolutionsRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.ScheduledDay
{
    using MediatR;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.ScheduledDay;

    /// <summary>
    /// The add workforce metric conflict resolution request handler.
    /// </summary>
    public class GetWorkforceMetricConflictResolutionsRequestHandler : IRequestHandler<GetWorkforceMetricConflictResolutionsRequest, PagedResponse<WorkforceMetricConflictResolutionDto>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetWorkforceMetricConflictResolutionsRequestHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkforceMetricConflictResolutionsRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public GetWorkforceMetricConflictResolutionsRequestHandler(ILogger<GetWorkforceMetricConflictResolutionsRequestHandler> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            this.logger = logger;
        }

        /// <inheritdoc />
        public Task<PagedResponse<WorkforceMetricConflictResolutionDto>> Handle(GetWorkforceMetricConflictResolutionsRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            return Task.FromException<PagedResponse<WorkforceMetricConflictResolutionDto>>(new NotImplementedException());
        }
    }
}