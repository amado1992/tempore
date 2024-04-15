// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsProcessingFileProcessRunningRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.FileProcessing
{
    using MediatR;

    using Tempore.Server.Handlers.Employees;
    using Tempore.Server.Invokables.Employees;
    using Tempore.Server.Invokables.FileProcessing;
    using Tempore.Server.Requests.Employees;
    using Tempore.Server.Requests.FileProcessing;
    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The IsProcessingFileProcessRunningRequestHandler class.
    /// </summary>
    public class IsProcessingFileProcessRunningRequestHandler : IRequestHandler<IsProcessingFileProcessRunningRequest, bool>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<IsProcessingFileProcessRunningRequestHandler> logger;

        /// <summary>
        /// The queue service.
        /// </summary>
        private readonly IQueueService queueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsProcessingFileProcessRunningRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="queueService">
        /// The queue service.
        /// </param>
        public IsProcessingFileProcessRunningRequestHandler(ILogger<IsProcessingFileProcessRunningRequestHandler> logger, IQueueService queueService)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(queueService);

            this.logger = logger;
            this.queueService = queueService;
        }

        /// <inheritdoc/>
        public Task<bool> Handle(IsProcessingFileProcessRunningRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return Task.FromResult(this.queueService.IsScheduled<ProcessFileInvokable>());
        }
    }
}