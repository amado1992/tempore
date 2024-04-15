// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Tempore.Agent.Entities;

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
            builder.HasIndex(e => e.Name).IsUnique();

            builder.HasOne(device => device.Agent)
                .WithMany(agent => agent.Devices)
                .HasForeignKey(device => device.AgentId);
        }
    }
}