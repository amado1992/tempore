// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWorkforceMetricsRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.WorkforceMetrics
{
    using MediatR;

    using MethodTimer;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Common.Extensions;
    using Tempore.Logging.Extensions;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.WorkforceMetrics;
    using Tempore.Server.Specs;
    using Tempore.Server.Specs.WorkforceMetrics;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The get workforce metrics request handler.
    /// </summary>
    public class GetWorkforceMetricsRequestHandler : IRequestHandler<GetWorkforceMetricsRequest, PagedResponse<Dictionary<string, object>>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetWorkforceMetricsRequestHandler> logger;

        /// <summary>
        /// The workforce metric collection repository.
        /// </summary>
        private readonly IRepository<WorkforceMetricCollection, ApplicationDbContext> workforceMetricCollectionRepository;

        /// <summary>
        /// The workforce metric daily snapshot repository.
        /// </summary>
        private readonly IRepository<WorkforceMetricDailySnapshot, ApplicationDbContext> workforceMetricDailySnapshotRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkforceMetricsRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="workforceMetricCollectionRepository">
        /// The workforce metric repository.
        /// </param>
        /// <param name="workforceMetricDailySnapshotRepository">
        /// The workforce metric daily snapshot repository.
        /// </param>
        public GetWorkforceMetricsRequestHandler(
            ILogger<GetWorkforceMetricsRequestHandler> logger,
            IRepository<WorkforceMetricCollection, ApplicationDbContext> workforceMetricCollectionRepository,
            IRepository<WorkforceMetricDailySnapshot, ApplicationDbContext> workforceMetricDailySnapshotRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(workforceMetricCollectionRepository);
            ArgumentNullException.ThrowIfNull(workforceMetricDailySnapshotRepository);

            this.logger = logger;
            this.workforceMetricCollectionRepository = workforceMetricCollectionRepository;
            this.workforceMetricDailySnapshotRepository = workforceMetricDailySnapshotRepository;
        }

        /// <inheritdoc />
        [Time]
        public async Task<PagedResponse<Dictionary<string, object>>> Handle(GetWorkforceMetricsRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var workforceMetricCollection = await this.workforceMetricCollectionRepository.SingleOrDefaultAsync(workforceMetricCollection => workforceMetricCollection.Id == request.WorkforceMetricCollectionId);
            if (workforceMetricCollection is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Workforce metric collection '{request.WorkforceMetricCollectionId}' not found");
            }

            var options = new PaginationOptions(request.Skip, request.Take, false);
            var cumulativeWorkforceMetricDailySnapshotSpec = new CumulativeWorkforceMetricDailySnapshotSpec(request.WorkforceMetricCollectionId, request.StartDate, request.EndDate, options);

            if (await this.workforceMetricDailySnapshotRepository.CountAsync(cumulativeWorkforceMetricDailySnapshotSpec) == 0)
            {
                return new PagedResponse<Dictionary<string, object>>(0, Enumerable.Empty<Dictionary<string, object>>());
            }

            var rows = new List<Dictionary<string, object>>();
            var workforceMetricAggregations = await this.workforceMetricDailySnapshotRepository.FindAsync(cumulativeWorkforceMetricDailySnapshotSpec).ToListAsync();
            for (var idx = 0; idx < workforceMetricAggregations.Count; idx++)
            {
                var row = new Dictionary<string, object>();

                var currentWorkforceMetricAggregation = workforceMetricAggregations[idx];

                row["Id"] = currentWorkforceMetricAggregation.ExternalId;
                row["Name"] = currentWorkforceMetricAggregation.EmployeeName;
                WorkforceMetricAggregation nextWorkforceMetricAggregation;
                do
                {
                    // TODO: Ensure set all metrics according file configuration?
                    nextWorkforceMetricAggregation = workforceMetricAggregations[idx];
                    row[nextWorkforceMetricAggregation.WorkforceMetricName] = nextWorkforceMetricAggregation.Value;
                    idx++;
                }
                while (idx < workforceMetricAggregations.Count
                       && currentWorkforceMetricAggregation.EmployeeId == nextWorkforceMetricAggregation.EmployeeId);

                rows.Add(row);

                if (idx < workforceMetricAggregations.Count)
                {
                    idx -= 2;
                }
            }

            return new PagedResponse<Dictionary<string, object>>(rows.Count, rows);
        }
    }
}