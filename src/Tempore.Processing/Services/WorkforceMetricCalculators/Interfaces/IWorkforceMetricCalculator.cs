// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkforceMetricCalculator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.Services.WorkforceMetricCalculators.Interfaces
{
    using Tempore.Storage.Entities;

    /// <summary>
    /// The WorkforceMetricCalculator interface.
    /// </summary>
    public interface IWorkforceMetricCalculator
    {
        /// <summary>
        /// Gets the workforce metric collection name.
        /// </summary>
        string WorkforceMetricCollectionName { get; }

        /// <summary>
        /// Gets the workforce metric.
        /// </summary>
        string WorkforceMetricName { get; }

        /// <summary>
        /// Calculates the daily snapshot for the workforce metric.
        /// </summary>
        /// <param name="scheduledDay">
        /// The scheduled day.
        /// </param>
        /// <returns>
        /// The <see cref="Task{WorkforceMetricDailySnapshot}"/>.
        /// </returns>
        Task<WorkforceMetricDailySnapshot> CalculateDailySnapshot(ScheduledDay scheduledDay);
    }
}