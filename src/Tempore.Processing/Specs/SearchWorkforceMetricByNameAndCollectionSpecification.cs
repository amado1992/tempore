// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchWorkforceMetricByNameAndCollectionSpecification.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.Specs
{
    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The search workforce metric by name and collection specification.
    /// </summary>
    public class SearchWorkforceMetricByNameAndCollectionSpecification : ISpecification<WorkforceMetric>
    {
        /// <summary>
        /// The workforce metric name.
        /// </summary>
        private readonly string workforceMetricName;

        /// <summary>
        /// The workforce metric collection name.
        /// </summary>
        private readonly string workforceMetricCollectionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchWorkforceMetricByNameAndCollectionSpecification"/> class.
        /// </summary>
        /// <param name="workforceMetricName">
        /// The workforce metric name.
        /// </param>
        /// <param name="workforceMetricCollectionName">
        /// The workforce metric collection name.
        /// </param>
        public SearchWorkforceMetricByNameAndCollectionSpecification(string workforceMetricName, string workforceMetricCollectionName)
        {
            this.workforceMetricName = workforceMetricName;
            this.workforceMetricCollectionName = workforceMetricCollectionName;
        }

        /// <inheritdoc />
        public Func<IQueryable<WorkforceMetric>, IQueryable<WorkforceMetric>> Build()
        {
            return metrics => metrics
                .Include(metric => metric.WorkforceMetricCollection)
                .Where(metric => metric.Name == this.workforceMetricName && metric.WorkforceMetricCollection.Name == this.workforceMetricCollectionName);
        }
    }
}