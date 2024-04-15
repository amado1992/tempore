// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDaySchedulerService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services.Interfaces
{
    using Tempore.Storage.Entities;

    /// <summary>
    /// The DaySchedulerService interface.
    /// </summary>
    public interface IDaySchedulerService
    {
        /// <summary>
        /// Schedule a next day for each employees according with the shift assignment.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<ScheduledDay> ScheduleDayAsync(DateOnly date, ScheduledShiftAssignment scheduledShiftAssignment);
    }
}