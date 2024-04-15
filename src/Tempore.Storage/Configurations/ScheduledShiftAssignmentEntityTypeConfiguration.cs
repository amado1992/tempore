// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledShiftAssignmentEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Serilog.Parsing;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The shift assignment entity type configuration.
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
            builder.HasIndex(assignment => new { assignment.EmployeeId, assignment.ScheduledShiftId }).IsUnique();

            builder
                .HasOne(assignment => assignment.ScheduledShift)
                .WithMany(shift => shift.ScheduledShiftAssignments)
                .HasForeignKey(assignment => assignment.ScheduledShiftId);
        }
    }
}