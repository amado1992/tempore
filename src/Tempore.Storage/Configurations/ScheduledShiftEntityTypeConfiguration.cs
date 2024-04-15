// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledShiftEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Tempore.Storage.Entities;

/// <summary>
/// The scheduled shift assignment entity type configuration.
/// </summary>
public class ScheduledShiftEntityTypeConfiguration : IEntityTypeConfiguration<ScheduledShift>
{
    /// <summary>
    /// The configure.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    public void Configure(EntityTypeBuilder<ScheduledShift> builder)
    {
        builder.HasIndex(scheduledShift => new { scheduledShift.ShiftId, scheduledShift.StartDate, scheduledShift.ExpireDate, scheduledShift.EffectiveWorkingTime }).IsUnique();
    }
}