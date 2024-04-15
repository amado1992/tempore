// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkforceMetricDailySnapshot.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The workforce metric daily snapshot.
    /// </summary>
    public class WorkforceMetricDailySnapshot
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the scheduled day id.
        /// </summary>
        public Guid ScheduledDayId { get; set; }

        /// <summary>
        /// Gets or sets the scheduled day.
        /// </summary>
        public ScheduledDay ScheduledDay { get; set; }

        /// <summary>
        /// Gets or sets the metric id.
        /// </summary>
        public Guid WorkforceMetricId { get; set; }

        /// <summary>
        /// Gets or sets the workforce metric.
        /// </summary>
        public WorkforceMetric WorkforceMetric { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double Value { get; set; }
    }
}