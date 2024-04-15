// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DaySchedulerService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using System;

    using Tempore.Common.Extensions;
    using Tempore.Logging.Extensions;
    using Tempore.Server.Services.Interfaces;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The day scheduler service.
    /// </summary>
    public class DaySchedulerService : IDaySchedulerService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DaySchedulerService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DaySchedulerService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DaySchedulerService(ILogger<DaySchedulerService> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task<ScheduledDay> ScheduleDayAsync(DateOnly date, ScheduledShiftAssignment scheduledShiftAssignment)
        {
            ArgumentNullException.ThrowIfNull(scheduledShiftAssignment);

            var scheduledShift = scheduledShiftAssignment.ScheduledShift;
            if (date < scheduledShift.StartDate)
            {
                throw this.logger.LogErrorAndCreateException<ArgumentException>($"he date argument '{date}' must be later than the shift assignment start date '{scheduledShift.StartDate}'");
            }

            if (date > scheduledShift.ExpireDate)
            {
                throw this.logger.LogErrorAndCreateException<ArgumentException>($"The date argument '{date}' must be earlier than the shift assignment expire date '{scheduledShift.ExpireDate}'");
            }

            var shift = scheduledShift.Shift;
            var index = ((int)scheduledShift.StartDate.DayOfWeek + date.DayNumber - scheduledShift.StartDate.DayNumber) % shift.Days.Count;
            var day = shift.Days.First(d => d.Index == index);

            var scheduledDay = new ScheduledDay
            {
                Date = date,
                DayId = day.Id,
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
            };

            var dailyHoursRate = 0.0d;
            var totalEffectiveWorkingHours = 0.0d;
            for (var currentDate = scheduledShift.StartDate; currentDate <= scheduledShift.ExpireDate; currentDate = currentDate.AddDays(1))
            {
                // TODO: Fix this, use the index formula?
                var currentDateDayOfWeek = (int)currentDate.DayOfWeek;
                totalEffectiveWorkingHours += shift.Days[currentDateDayOfWeek].Timetable?.EffectiveWorkingTime.TotalHours ?? 0.0d;
            }

            if (totalEffectiveWorkingHours != 0)
            {
                dailyHoursRate = scheduledShift.EffectiveWorkingTime.TotalHours / totalEffectiveWorkingHours;
            }

            if (day.Timetable is not null)
            {
                var timetable = day.Timetable;

                var startDateTime = date.Add(timetable.StartTime);
                var endDateTime = startDateTime.Add(timetable.Duration);
                var checkInStartDateTime = startDateTime.Add(timetable.CheckInTimeStart);
                var checkInEndDateTime = checkInStartDateTime.Add(timetable.CheckInTimeDuration);
                var checkOutStartDateTime = endDateTime.Add(timetable.CheckOutTimeStart);
                var checkOutEndDateTime = checkOutStartDateTime.Add(timetable.CheckOutTimeDuration);

                scheduledDay.StartDateTime = startDateTime;
                scheduledDay.EndDateTime = endDateTime;
                scheduledDay.CheckInStartDateTime = checkInStartDateTime;
                scheduledDay.CheckInEndDateTime = checkInEndDateTime;
                scheduledDay.CheckOutStartDateTime = checkOutStartDateTime;
                scheduledDay.CheckOutEndDateTime = checkOutEndDateTime;

                scheduledDay.EffectiveWorkingTime = timetable.EffectiveWorkingTime;
                scheduledDay.RelativeEffectiveWorkingTime = TimeSpan.FromHours(timetable.EffectiveWorkingTime.TotalHours * dailyHoursRate);
            }

            return scheduledDay;
        }
    }
}