// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimestampEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The timestamp entity type configuration.
    /// </summary>
    public class TimestampEntityTypeConfiguration : IEntityTypeConfiguration<Timestamp>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Configure(EntityTypeBuilder<Timestamp> builder)
        {
            builder
                .HasOne(timestamp => timestamp.EmployeeFromDevice)
                .WithMany(employeeFromDevice => employeeFromDevice.Timestamps)
                .HasForeignKey(timestamp => timestamp.EmployeeFromDeviceId);

            builder
                .HasOne(timestamp => timestamp.ScheduledDay)
                .WithMany(scheduledJourney => scheduledJourney.Timestamps)
                .HasForeignKey(timestamp => timestamp.ScheduledDayId)
                .IsRequired(false);

            builder.HasIndex(timestamp => new { timestamp.EmployeeFromDeviceId, timestamp.DateTime }).IsUnique();
        }
    }
}