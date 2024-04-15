// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkforceMetricCalculatorBase.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.Services.WorkforceMetricCalculators
{
    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Processing.Services.Exceptions;
    using Tempore.Processing.Services.WorkforceMetricCalculators.Interfaces;
    using Tempore.Processing.Specs;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The workforce metric calculator base.
    /// </summary>
    public abstract class WorkforceMetricCalculatorBase : IWorkforceMetricCalculator
    {
        /// <summary>
        /// The workforce metric repository.
        /// </summary>
        private readonly IRepository<WorkforceMetric, ApplicationDbContext> workforceMetricRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkforceMetricCalculatorBase"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="workforceMetricRepository">
        /// The repository.
        /// </param>
        protected WorkforceMetricCalculatorBase(
            ILogger<WorkforceMetricCalculatorBase> logger,
            IRepository<WorkforceMetric, ApplicationDbContext> workforceMetricRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(workforceMetricRepository);

            this.Logger = logger;
            this.workforceMetricRepository = workforceMetricRepository;
        }

        /// <summary>
        /// Gets the workforce metric collection name.
        /// </summary>
        public abstract string WorkforceMetricCollectionName { get; }

        /// <summary>
        /// Gets the workforce metric name.
        /// </summary>
        public abstract string WorkforceMetricName { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger<WorkforceMetricCalculatorBase> Logger { get; }

        /// <summary>
        /// Calculates the daily snapshot of the metric.
        /// </summary>
        /// <param name="scheduledDay">
        /// The scheduled day.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<WorkforceMetricDailySnapshot> CalculateDailySnapshot(ScheduledDay scheduledDay)
        {
            this.Logger.LogInformation("Calculating daily snapshot of workforce metric '{WorkforceMetricCollectionName}\\{WorkforceMetricName}' for employee '{EmployeeId}' on scheduled day '{ScheduledDayId}'", this.WorkforceMetricCollectionName, this.WorkforceMetricName, scheduledDay.Id, scheduledDay.ScheduledShiftAssignment?.EmployeeId);

            var workforceMetric = await this.workforceMetricRepository.SingleOrDefaultAsync(
                new SearchWorkforceMetricByNameAndCollectionSpecification(
                    this.WorkforceMetricName,
                    this.WorkforceMetricCollectionName));

            if (workforceMetric is null)
            {
                throw this.Logger.LogErrorAndCreateException<WorkforceMetricNotFoundException>($"The workforce metric '{this.WorkforceMetricCollectionName}\\{this.WorkforceMetricName}' is not registered.");
            }

            var workforceMetricDailySnapshot = new WorkforceMetricDailySnapshot
            {
                ScheduledDayId = scheduledDay.Id,
                WorkforceMetricId = workforceMetric.Id,
            };

            var conflictResolution = scheduledDay.WorkforceMetricConflictResolutions?
                    .Where(workforceMetricConflictResolution => workforceMetricConflictResolution.WorkforceMetricId == workforceMetric.Id)
                    .Aggregate(default(double?), (current, workforceMetricConflictResolution) => !current.HasValue ? workforceMetricConflictResolution.Value : current + workforceMetricConflictResolution.Value);

            if (conflictResolution.HasValue)
            {
                workforceMetricDailySnapshot.Value = conflictResolution.Value;

                this.Logger.LogInformation("Calculated daily snapshot of workforce metric '{WorkforceMetricCollectionName}\\{WorkforceMetricName}' for employee '{EmployeeId}' on scheduled day '{ScheduledDayId}' using conflict resolution", this.WorkforceMetricCollectionName, this.WorkforceMetricName, scheduledDay.Id, scheduledDay.ScheduledShiftAssignment?.EmployeeId);
            }
            else
            {
                workforceMetricDailySnapshot.Value = await this.CalculateDailySnapshotValueAsync(scheduledDay);

                this.Logger.LogInformation("Calculated daily snapshot of workforce metric '{WorkforceMetricCollectionName}\\{WorkforceMetricName}' for employee '{EmployeeId}' on scheduled day '{ScheduledDayId}'", this.WorkforceMetricCollectionName, this.WorkforceMetricName, scheduledDay.Id, scheduledDay.ScheduledShiftAssignment?.EmployeeId);
            }

            return workforceMetricDailySnapshot;
        }

        /// <summary>
        /// Calculate daily snapshot value async.
        /// </summary>
        /// <param name="scheduledDay">
        /// The scheduled day.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected virtual Task<double> CalculateDailySnapshotValueAsync(ScheduledDay scheduledDay)
        {
            return Task.FromResult<double>(0);
        }
    }
}