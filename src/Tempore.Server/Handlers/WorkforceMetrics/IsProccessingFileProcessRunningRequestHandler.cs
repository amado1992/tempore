// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsProccessingFileProcessRunningRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.WorkforceMetrics
{
    using MediatR;

    using Tempore.Server.Invokables.Employees;
    using Tempore.Server.Invokables.WorkforceMetrics;
    using Tempore.Server.Requests.WorkforceMetrics;
    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The IsComputeWorkforceMetricsProcessRunningRequestHandler class.
    /// </summary>
    public class IsComputeWorkforceMetricsProcessRunningRequestHandler : IRequestHandler<IsComputeWorkforceMetricsProcessRunningRequest, bool>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<IsComputeWorkforceMetricsProcessRunningRequestHandler> logger;

        /// <summary>
        /// The queue service.
        /// </summary>
        private readonly IQueueService queueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsComputeWorkforceMetricsProcessRunningRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="queueService">
        /// The queue service.
        /// </param>
        public IsComputeWorkforceMetricsProcessRunningRequestHandler(ILogger<IsComputeWorkforceMetricsProcessRunningRequestHandler> logger, IQueueService queueService)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(queueService);

            this.logger = logger;
            this.queueService = queueService;
        }

        /// <inheritdoc/>
        public Task<bool> Handle(IsComputeWorkforceMetricsProcessRunningRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return Task.FromResult(this.queueService.IsScheduled<ComputeWorkforceMetricsInvokable>());
        }
    }
}