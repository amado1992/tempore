// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimestampScheduledDaysCandidatesSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Invokables.ScheduledDay
{
    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Server.Invokables.Timestamps.Models;
    using Tempore.Storage.Entities;

    public class TimestampScheduledDaysCandidatesSpec : ISpecification<Timestamp, TimestampScheduledDaysTuple>
    {
        /// <inheritdoc/>
        public Func<IQueryable<Timestamp>, IQueryable<TimestampScheduledDaysTuple>> Build()
        {
            return timestamps => timestamps
                .Include(timestamp => timestamp.EmployeeFromDevice)
                .ThenInclude(employeeFromDevice => employeeFromDevice.Employee)
                .ThenInclude(employee => employee!.ScheduledShiftAssignments)
                .ThenInclude(assignment => assignment.ScheduledDays)
                .Where(
                    timestamp => timestamp.ScheduledDayId == null
                                 && timestamp.EmployeeFromDevice != null
                                 && timestamp.EmployeeFromDevice.Employee != null
                                 && timestamp.EmployeeFromDevice.Employee.ScheduledShiftAssignments != null)
                .Select(timestamp => new TimestampScheduledDaysTuple
                {
                    Timestamp = timestamp,
                    ScheduledDays = timestamp.EmployeeFromDevice.Employee!.ScheduledShiftAssignments
                        .SelectMany(assignment => assignment.ScheduledDays!)
                        .OrderBy(day => day.Date)
                        .ToList(),
                });
        }
    }
}