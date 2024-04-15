// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkforceMetric.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The workforce metric.
    /// </summary>
    public class WorkforceMetric
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the workforce metric collection id.
        /// </summary>
        public Guid WorkforceMetricCollectionId { get; set; }

        /// <summary>
        /// Gets or sets the workforce metric group.
        /// </summary>
        public WorkforceMetricCollection WorkforceMetricCollection { get; set; }

        /// <summary>
        /// Gets or sets the work force metric conflict resolutions.
        /// </summary>
        public List<WorkforceMetricConflictResolution> WorkforceMetricConflictResolutions { get; set; }
    }
}