// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeEntityTypeConfiguration.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Storage.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Tempore.Storage.Entities;

/// <summary>
/// The employee entity type configuration.
/// </summary>
public class EmployeeEntityTypeConfiguration : IEntityTypeConfiguration<Employee>
{
    /// <summary>
    /// The configure.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasIndex(employee => employee.ExternalId).IsUnique();

        builder
            .HasMany(employee => employee.ScheduledShifts)
            .WithMany(scheduledShift => scheduledShift.Employees)
            .UsingEntity<ScheduledShiftAssignment>(
                right => right
                    .HasOne(scheduledShiftAssignment => scheduledShiftAssignment.ScheduledShift)
                    .WithMany(shift => shift.ScheduledShiftAssignments)
                    .HasForeignKey(scheduledShiftAssignment => scheduledShiftAssignment.ScheduledShiftId),
                left => left
                .HasOne(scheduledShiftAssignment => scheduledShiftAssignment.Employee)
                .WithMany(employee => employee.ScheduledShiftAssignments)
                .HasForeignKey(assignment => assignment.EmployeeId));
    }
}