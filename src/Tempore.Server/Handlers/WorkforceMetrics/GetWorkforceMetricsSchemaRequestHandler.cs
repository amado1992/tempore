// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWorkforceMetricsSchemaRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.WorkforceMetrics
{
    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Processing;
    using Tempore.Processing.Services.Interfaces;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests.WorkforceMetrics;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The compute workforce metrics schema request handler.
    /// </summary>
    public class GetWorkforceMetricsSchemaRequestHandler : IRequestHandler<GetWorkforceMetricsSchemaRequest, List<ColumnInfo>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetWorkforceMetricsSchemaRequestHandler> logger;

        /// <summary>
        /// The workforce mMetric collection schema provider factory.
        /// </summary>
        private readonly IWorkforceMetricCollectionSchemaProviderFactory workforceMetricCollectionSchemaProviderFactory;

        /// <summary>
        /// The workforce metric collection repository.
        /// </summary>
        private readonly IRepository<WorkforceMetricCollection, ApplicationDbContext> workforceMetricCollectionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkforceMetricsSchemaRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="workforceMetricCollectionSchemaProviderFactory">
        /// The workforce metric collection schema provider.
        /// </param>
        /// <param name="workforceMetricCollectionRepository">
        /// The workforce metric repository.
        /// </param>
        public GetWorkforceMetricsSchemaRequestHandler(
            ILogger<GetWorkforceMetricsSchemaRequestHandler> logger,
            IWorkforceMetricCollectionSchemaProviderFactory workforceMetricCollectionSchemaProviderFactory,
            IRepository<WorkforceMetricCollection, ApplicationDbContext> workforceMetricCollectionRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(workforceMetricCollectionSchemaProviderFactory);
            ArgumentNullException.ThrowIfNull(workforceMetricCollectionRepository);

            this.logger = logger;
            this.workforceMetricCollectionSchemaProviderFactory = workforceMetricCollectionSchemaProviderFactory;
            this.workforceMetricCollectionRepository = workforceMetricCollectionRepository;
        }

        /// <inheritdoc />
        public async Task<List<ColumnInfo>> Handle(GetWorkforceMetricsSchemaRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var workforceMetricCollection = await this.workforceMetricCollectionRepository.SingleOrDefaultAsync(workforceMetricCollection => workforceMetricCollection.Id == request.WorkforceMetricCollectionId);
            if (workforceMetricCollection is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Metric collection '{request.WorkforceMetricCollectionId}' not found");
            }

            if (!this.workforceMetricCollectionSchemaProviderFactory.IsSupported(workforceMetricCollection.Name))
            {
                throw this.logger.LogErrorAndCreateException<BadRequestException>($"Workforce metric collection '{workforceMetricCollection.Name}' is not supported.");
            }

            var workforceMetricCollectionSchemaProvider = this.workforceMetricCollectionSchemaProviderFactory.Create(workforceMetricCollection.Name);

            return workforceMetricCollectionSchemaProvider.GetSchema(request.SchemaType).ToList();
        }
    }
}