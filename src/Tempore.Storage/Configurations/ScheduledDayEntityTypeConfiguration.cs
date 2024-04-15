// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledDayEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The journey day type configuration.
    /// </summary>
    public class ScheduledDayEntityTypeConfiguration : IEntityTypeConfiguration<ScheduledDay>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Configure(EntityTypeBuilder<ScheduledDay> builder)
        {
            builder.HasOne(scheduledDay => scheduledDay.ScheduledShiftAssignment)
                .WithMany(day => day.ScheduledDays)
                .HasForeignKey(scheduleDay => scheduleDay.ScheduledShiftAssignmentId);

            builder.HasOne(scheduledDay => scheduledDay.Day)
                .WithMany(day => day.ScheduledDays)
                .HasForeignKey(scheduledDay => scheduledDay.DayId);

            builder.HasIndex(scheduledDay => new { scheduledDay.Date, scheduledDay.ScheduledShiftAssignmentId }).IsUnique();
        }
    }
}