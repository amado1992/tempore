// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The agent entity type configuration.
    /// </summary>
    public class AgentEntityTypeConfiguration : IEntityTypeConfiguration<Agent>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Configure(EntityTypeBuilder<Agent> builder)
        {
            builder.HasIndex(agent => agent.Name).IsUnique();

            builder.HasMany(agent => agent.Devices)
                .WithOne(device => device.Agent)
                .HasForeignKey(device => device.AgentId);
        }
    }
}