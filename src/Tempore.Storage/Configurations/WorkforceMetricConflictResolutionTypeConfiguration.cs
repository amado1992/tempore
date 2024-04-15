// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkforceMetricConflictResolutionTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The workforce metric conflict type configuration.
    /// </summary>
    public class WorkforceMetricConflictResolutionTypeConfiguration : IEntityTypeConfiguration<WorkforceMetricConflictResolution>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Configure(EntityTypeBuilder<WorkforceMetricConflictResolution> builder)
        {
            builder.HasIndex(workforceMetricConflictResolution => new { workforceMetricConflictResolution.ScheduledDayId, workforceMetricConflictResolution.WorkforceMetricId }).IsUnique();

            builder.HasOne(workforceMetricConflictResolution => workforceMetricConflictResolution.ScheduledDay)
                .WithMany(scheduledDay => scheduledDay.WorkforceMetricConflictResolutions)
                .HasForeignKey(workforceMetricConflictResolution => workforceMetricConflictResolution.ScheduledDayId);

            builder.HasOne(workforceMetricConflictResolution => workforceMetricConflictResolution.WorkforceMetric)
                .WithMany(workforceMetric => workforceMetric.WorkforceMetricConflictResolutions)
                .HasForeignKey(workforceMetricConflictResolution => workforceMetricConflictResolution.WorkforceMetricId);
        }
    }
}