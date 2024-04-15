// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DayEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The day entity type configuration.
    /// </summary>
    public class DayEntityTypeConfiguration : IEntityTypeConfiguration<Day>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Configure(EntityTypeBuilder<Day> builder)
        {
            builder
                .HasOne(day => day.Timetable)
                .WithMany(timetable => timetable.Days)
                .HasForeignKey(day => day.TimetableId)
                .IsRequired(false);

            builder
                .HasOne(day => day.Shift)
                .WithMany(shift => shift.Days)
                .HasForeignKey(day => day.ShiftId);
        }
    }
}