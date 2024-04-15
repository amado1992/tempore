// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayDayWorkforceMetricCalculatorBase.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.PayDay.Services.WorkforceMetricCalculators
{
    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Processing.Services.WorkforceMetricCalculators;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The workforce metric calculator base.
    /// </summary>
    public abstract class PayDayWorkforceMetricCalculatorBase : WorkforceMetricCalculatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayDayWorkforceMetricCalculatorBase"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="workforceMetricRepository">
        /// The workforce metric repository.
        /// </param>
        protected PayDayWorkforceMetricCalculatorBase(
            ILogger<PayDayWorkforceMetricCalculatorBase> logger,
            IRepository<WorkforceMetric, ApplicationDbContext> workforceMetricRepository)
            : base(logger, workforceMetricRepository)
        {
        }

        /// <summary>
        /// Gets the workforce metric collection name.
        /// </summary>
        public override string WorkforceMetricCollectionName => PayDayWorkforceMetricCollection.Name;
    }
}