// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComputeWorkforceMetricsRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.WorkforceMetrics
{
    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Invokables.Interfaces;
    using Tempore.Server.Invokables.WorkforceMetrics;
    using Tempore.Server.Requests.WorkforceMetrics;
    using Tempore.Server.Services.Interfaces;
    using Tempore.Server.Specs;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The compute workforce metrics request handler.
    /// </summary>
    public class ComputeWorkforceMetricsRequestHandler : IRequestHandler<ComputeWorkforceMetricsRequest, Guid>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ComputeWorkforceMetricsRequestHandler> logger;

        /// <summary>
        /// The queue task registry.
        /// </summary>
        private readonly IQueueService queueService;

        /// <summary>
        /// The invocation context accessor.
        /// </summary>
        private readonly IInvocationContextAccessor invocationContextAccessor;

        /// <summary>
        /// The workforce metric collection repository.
        /// </summary>
        private readonly IRepository<WorkforceMetricCollection, ApplicationDbContext> workforceMetricCollectionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeWorkforceMetricsRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="queueService">
        /// The queue service.
        /// </param>
        /// <param name="invocationContextAccessor">
        /// The invocation context accessor.
        /// </param>
        /// <param name="workforceMetricCollectionRepository">
        /// The workforce metric collection repository.
        /// </param>
        public ComputeWorkforceMetricsRequestHandler(
            ILogger<ComputeWorkforceMetricsRequestHandler> logger,
            IQueueService queueService,
            IInvocationContextAccessor invocationContextAccessor,
            IRepository<WorkforceMetricCollection, ApplicationDbContext> workforceMetricCollectionRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(queueService);
            ArgumentNullException.ThrowIfNull(invocationContextAccessor);
            ArgumentNullException.ThrowIfNull(workforceMetricCollectionRepository);

            this.logger = logger;
            this.queueService = queueService;
            this.invocationContextAccessor = invocationContextAccessor;
            this.workforceMetricCollectionRepository = workforceMetricCollectionRepository;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="request">
        /// The notification.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<Guid> Handle(ComputeWorkforceMetricsRequest request, CancellationToken cancellationToken)
        {
            var workforceMetricCollectionSpec = SpecificationBuilder.Build<WorkforceMetricCollection>(collections => collections.Where(collection => request.WorkForceMetricCollectionIds.Contains(collection.Id)));
            if (await this.workforceMetricCollectionRepository.CountAsync(workforceMetricCollectionSpec) != request.WorkForceMetricCollectionIds.Count)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>("One or more workforce metric collection IDs were not found");
            }

            if (this.queueService.IsScheduled<ComputeWorkforceMetricsInvokable>())
            {
                throw this.logger.LogErrorAndCreateException<ConflictException>("Compute workforce metrics job is already scheduled");
            }

            var taskId = this.queueService.QueueInvocableWithPayload<ComputeWorkforceMetricsInvokable, IInvocationContext<ComputeWorkforceMetricsRequest>>(this.invocationContextAccessor.Create(request));
            return taskId;
        }
    }
}