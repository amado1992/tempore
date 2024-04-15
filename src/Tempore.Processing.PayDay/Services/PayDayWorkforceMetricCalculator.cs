﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayDayWorkforceMetricCalculator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.PayDay.Services
{
    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Processing.PayDay;
    using Tempore.Processing.Services.WorkforceMetricCalculators;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The pay day workforce metric processor.
    /// </summary>
    public abstract class PayDayWorkforceMetricCalculator : WorkforceMetricCalculatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayDayWorkforceMetricCalculator"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="workforceMetricRepository">
        /// The repository.
        /// </param>
        protected PayDayWorkforceMetricCalculator(
            ILogger<PayDayWorkforceMetricCalculator> logger,
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