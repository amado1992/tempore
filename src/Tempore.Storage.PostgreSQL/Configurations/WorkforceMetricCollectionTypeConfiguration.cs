﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkforceMetricCollectionTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.PostgreSQL.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The workforce metric collection type configuration.
    /// </summary>
    public class WorkforceMetricCollectionTypeConfiguration : IEntityTypeConfiguration<WorkforceMetricCollection>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Configure(EntityTypeBuilder<WorkforceMetricCollection> builder)
        {
            builder
                .Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
        }
    }
}