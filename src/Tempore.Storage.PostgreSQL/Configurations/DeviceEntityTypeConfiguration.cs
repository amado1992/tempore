// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.PostgreSQL.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The device entity type configuration.
    /// </summary>
    public class DeviceEntityTypeConfiguration : IEntityTypeConfiguration<Device>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder
                .Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()");

            builder.HasIndex(r => r.MacAddress)
                .HasFilter("\"MacAddress\" IS NOT NULL")
                .IsUnique();

            builder.HasIndex(device => device.SerialNumber)
                .HasFilter("\"SerialNumber\" IS NOT NULL")
                .IsUnique();
        }
    }
}