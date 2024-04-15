﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledShiftAssignmentEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.PostgreSQL.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The scheduled shift assignment entity type configuration.
    /// </summary>
    public class ScheduledShiftAssignmentEntityTypeConfiguration : IEntityTypeConfiguration<ScheduledShiftAssignment>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Configure(EntityTypeBuilder<ScheduledShiftAssignment> builder)
        {
            builder
                .Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
        }
    }
}