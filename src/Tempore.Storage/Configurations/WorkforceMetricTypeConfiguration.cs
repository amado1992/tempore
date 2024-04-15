﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkforceMetricTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The workforce metric type configuration.
    /// </summary>
    public class WorkforceMetricTypeConfiguration : IEntityTypeConfiguration<WorkforceMetric>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Configure(EntityTypeBuilder<WorkforceMetric> builder)
        {
            builder.HasIndex(workforceMetric => workforceMetric.Name).IsUnique();
            builder.HasOne(workforceMetric => workforceMetric.WorkforceMetricCollection)
                .WithMany(collection => collection.WorkforceMetrics)
                .HasForeignKey(metric => metric.WorkforceMetricCollectionId);
        }
    }
}