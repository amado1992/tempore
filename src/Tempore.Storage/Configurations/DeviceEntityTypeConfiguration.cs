// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Configurations
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
            builder.HasIndex(device => device.Name).IsUnique();

            builder.Property(device => device.DeviceName).IsRequired(false);
            builder.HasIndex(device => device.DeviceName);

            builder.Property(device => device.MacAddress).IsRequired(false);
            builder.HasIndex(device => device.MacAddress);

            builder.Property(device => device.SerialNumber).IsRequired(false);
            builder.HasIndex(device => device.SerialNumber);

            builder.HasMany(device => device.EmployeesFromDevices)
                .WithOne(device => device.Device)
                .HasForeignKey(device => device.DeviceId);
        }
    }
}