// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComputeWorkforceMetricsInvokable.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Invokables.WorkforceMetrics
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Coravel.Invocable;

    using MediatR;

    using MethodTimer;

    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Client.Services.Interfaces;
    using Tempore.Common.Extensions;
    using Tempore.Processing.Services.WorkforceMetricCalculators.Interfaces;
    using Tempore.Server.Invokables.Interfaces;
    using Tempore.Server.Notifications.WorkforceMetrics;
    using Tempore.Server.Requests.WorkforceMetrics;
    using Tempore.Server.Specs;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The compute workforce metrics invokable.
    /// </summary>
    public class ComputeWorkforceMetricsInvokable : IInvocable, IInvocableWithPayload<IInvocationContext<ComputeWorkforceMetricsRequest>>, ICancellableInvocable
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ComputeWorkforceMetricsInvokable> logger;

        /// <summary>
        /// The unit of work.
        /// </summary>
        private readonly IUnitOfWork<ApplicationDbContext> unitOfWork;

        /// <summary>
        /// The publisher.
        /// </summary>
        private readonly IPublisher publisher;

        private readonly IEnumerable<IWorkforceMetricCalculator> workforceMetricCalculators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeWorkforceMetricsInvokable"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="unitOfWork">
        /// The unit Of work.
        /// </param>
        /// <param name="publisher">
        /// The publisher.
        /// </param>
        /// <param name="workforceMetricCalculators">
        /// The workforce metric calculators.
        /// </param>
        public ComputeWorkforceMetricsInvokable(ILogger<ComputeWorkforceMetricsInvokable> logger, IUnitOfWork<ApplicationDbContext> unitOfWork, IPublisher publisher, IEnumerable<IWorkforceMetricCalculator> workforceMetricCalculators)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(unitOfWork);
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(workforceMetricCalculators);

            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.publisher = publisher;
            this.workforceMetricCalculators = workforceMetricCalculators;
        }

        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public IInvocationContext<ComputeWorkforceMetricsRequest> Payload { get; set; } = default!;

        /// <summary>
        /// Gets or sets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Time]
        public async Task Invoke()
        {
            var startDate = this.Payload.Request.StartDate;
            var endDate = this.Payload.Request.EndDate;
            var workforceMetricCollectionIds = this.Payload.Request.WorkForceMetricCollectionIds;

            this.logger.LogInformation("Computing workforce metrics from '{StartDate}' to '{EndDate}'", startDate, endDate);
            try
            {
                var workforceMetricCollectionRepository = this.unitOfWork.GetRepository<WorkforceMetricCollection>();

                var workforceMetricCollectionSpec = SpecificationBuilder.Build<WorkforceMetricCollection>(collections => collections.Where(collection => workforceMetricCollectionIds.Contains(collection.Id)));
                var metricCollections = await workforceMetricCollectionRepository.FindAsync(workforceMetricCollectionSpec).ToListAsync();

                var workforceMetricCollections = metricCollections.ToList();

                var filteredWorkforceCalculators = this.workforceMetricCalculators
                    .Where(calculator => workforceMetricCollections.Exists(collection => collection.Name == calculator.WorkforceMetricCollectionName))
                    .ToList();

                var scheduledDayRepository = this.unitOfWork.GetRepository<ScheduledDay>();
                var workforceMetricDailySnapshotRepository = this.unitOfWork.GetRepository<WorkforceMetricDailySnapshot>();

                var scheduledDaySpec = SpecificationBuilder.Build<ScheduledDay>(
                    days => days.Include(day => day.ScheduledShiftAssignment)
                        .Include(day => day.WorkforceMetricConflictResolutions)
                        .Include(day => day.Timestamps)
                        .Where(day => day.Date >= startDate && day.Date <= endDate));

                var scheduledDays = await scheduledDayRepository.FindAsync(scheduledDaySpec);
                foreach (var scheduledDay in scheduledDays)
                {
                    foreach (var calculator in filteredWorkforceCalculators)
                    {
                        var calculatedWorkforceMetricDailySnapshot = await calculator.CalculateDailySnapshot(scheduledDay);

                        var workforceMetricDailySnapshotSpec = SpecificationBuilder.Build<WorkforceMetricDailySnapshot>(snapshots => snapshots.Where(snapshot => snapshot.ScheduledDayId == calculatedWorkforceMetricDailySnapshot.ScheduledDayId && snapshot.WorkforceMetricId == calculatedWorkforceMetricDailySnapshot.WorkforceMetricId));
                        var workforceMetricDailySnapshot = await workforceMetricDailySnapshotRepository.SingleOrDefaultAsync(workforceMetricDailySnapshotSpec);
                        if (workforceMetricDailySnapshot is null)
                        {
                            workforceMetricDailySnapshotRepository.Add(calculatedWorkforceMetricDailySnapshot);
                        }
                        else
                        {
                            workforceMetricDailySnapshot.Value = calculatedWorkforceMetricDailySnapshot.Value;
                            workforceMetricDailySnapshotRepository.Update(calculatedWorkforceMetricDailySnapshot);
                        }

                        await workforceMetricDailySnapshotRepository.SaveChangesAsync();
                    }
                }

                await this.publisher.Publish(
                    new ComputeWorkforceMetricsProcessCompletedNotification(this.Payload, Severity.Success),
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error computing workforce metrics from '{StartDate}' to '{EndDate}'", startDate, endDate);

                await this.publisher.Publish(
                    new ComputeWorkforceMetricsProcessCompletedNotification(this.Payload, Severity.Error),
                    CancellationToken.None);
            }
        }
    }
}