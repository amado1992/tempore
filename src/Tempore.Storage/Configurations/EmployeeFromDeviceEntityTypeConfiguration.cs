// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeFromDeviceEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The employee from device entity type configuration.
    /// </summary>
    public class EmployeeFromDeviceEntityTypeConfiguration : IEntityTypeConfiguration<EmployeeFromDevice>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Configure(EntityTypeBuilder<EmployeeFromDevice> builder)
        {
            builder
                .HasOne(employeeFromDevice => employeeFromDevice.Employee)
                .WithMany(employee => employee.EmployeeFromDevice)
                .HasForeignKey(e => e.EmployeeId)
                .IsRequired(false);

            builder.HasIndex(e => new { e.EmployeeIdOnDevice, e.DeviceId }).IsUnique();
        }
    }
}