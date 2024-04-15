// --------------------------------------------------------------------------------------------------------------------
// <copyright file="STMixtoDiaFerWorkforceMetricCalculator.cs" company="Port Hope Investment S.A.">
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
    /// The <see cref="PayDayWorkforceMetrics.STMixtoDiaFer"/> processor.
    /// </summary>
    public sealed class STMixtoDiaFerWorkforceMetricCalculator : PayDayWorkforceMetricCalculatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="STMixtoDiaFerWorkforceMetricCalculator"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="workforceMetricRepository">
        /// The workforce metric repository.
        /// </param>
        public STMixtoDiaFerWorkforceMetricCalculator(
            ILogger<STMixtoDiaFerWorkforceMetricCalculator> logger,
            IRepository<WorkforceMetric, ApplicationDbContext> workforceMetricRepository)
            : base(logger, workforceMetricRepository)
        {
        }

        /// <summary>
        /// Gets the workforce metric.
        /// </summary>
        public override string WorkforceMetricName => PayDayWorkforceMetrics.STMixtoDiaFer;
    }
}