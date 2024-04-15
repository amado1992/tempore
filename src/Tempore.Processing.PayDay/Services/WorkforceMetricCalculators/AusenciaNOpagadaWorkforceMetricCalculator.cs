// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AusenciaNOpagadaWorkforceMetricCalculator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.PayDay.Services.WorkforceMetricCalculators
{
    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The <see cref="PayDayWorkforceMetrics.AusenciaNOpagada"/> processor.
    /// </summary>
    public sealed class AusenciaNOpagadaWorkforceMetricCalculator : PayDayWorkforceMetricCalculatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AusenciaNOpagadaWorkforceMetricCalculator"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="workforceMetricRepository">
        /// The workforce metric repository.
        /// </param>
        public AusenciaNOpagadaWorkforceMetricCalculator(
            ILogger<AusenciaNOpagadaWorkforceMetricCalculator> logger,
            IRepository<WorkforceMetric, ApplicationDbContext> workforceMetricRepository)
            : base(logger, workforceMetricRepository)
        {
        }

        /// <summary>
        /// Gets the workforce metric.
        /// </summary>
        public override string WorkforceMetricName => PayDayWorkforceMetrics.AusenciaNOpagada;

        /// <inheritdoc />
        protected override Task<double> CalculateDailySnapshotValueAsync(ScheduledDay scheduledDay)
        {
            // TODO: Review this.
            var resolution = scheduledDay.WorkforceMetricConflictResolutions.Sum(resolution => resolution.Value);

            if (scheduledDay.Timestamps.Count < 2)
            {
                return Task.FromResult(Math.Max(scheduledDay.RelativeEffectiveWorkingTime.TotalHours - resolution, 0.0d));
            }

            return Task.FromResult(0.0d);
        }
    }
}