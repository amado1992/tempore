// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JorOrdDiurnaWorkforceMetricCalculator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.PayDay.Services.WorkforceMetricCalculators
{
    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Common.Extensions;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The <see cref="PayDayWorkforceMetrics.JorOrdDiurna"/> processor.
    /// </summary>
    public sealed class JorOrdDiurnaWorkforceMetricCalculator : PayDayWorkforceMetricCalculatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JorOrdDiurnaWorkforceMetricCalculator"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="workforceMetricRepository">
        /// The workforce metric repository.
        /// </param>
        public JorOrdDiurnaWorkforceMetricCalculator(
            ILogger<JorOrdDiurnaWorkforceMetricCalculator> logger,
            IRepository<WorkforceMetric, ApplicationDbContext> workforceMetricRepository)
            : base(logger, workforceMetricRepository)
        {
        }

        /// <summary>
        /// Gets the workforce metric.
        /// </summary>
        public override string WorkforceMetricName => PayDayWorkforceMetrics.JorOrdDiurna;

        /// <summary>
        /// Calculates daily snapshot value async.
        /// </summary>
        /// <param name="scheduledDay">
        /// The scheduled day.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task<double> CalculateDailySnapshotValueAsync(ScheduledDay scheduledDay)
        {
            // TODO: Review ....
            // if (scheduledDay.Day.IsRest)
            // {
            //    this.Logger.LogInformation("Unable to calculate the '{WorkforceMetricCollectionName}\\{WorkforceMetricName}' metric for employee '{EmployeeId}' on scheduled day '{ScheduledDayId}' because is a rest day", this.WorkforceMetricCollectionName, this.WorkforceMetricName, scheduledDay.ShiftAssignment?.EmployeeId, scheduledDay.Id);
            //    return Task.FromResult(0.0);
            // }
            if (scheduledDay.StartDateTime is not { Hour: >= 6 } || scheduledDay.EndDateTime is not { Hour: <= 18 })
            {
                this.Logger.LogInformation(
                    "Unable to calculate the '{WorkforceMetricCollectionName}\\{WorkforceMetricName}' metric for employee '{EmployeeId}' on scheduled day '{ScheduledDayId}'",
                    this.WorkforceMetricCollectionName,
                    this.WorkforceMetricName,
                    scheduledDay.ScheduledShiftAssignment?.EmployeeId,
                    scheduledDay.Id);

                return Task.FromResult(0.0);
            }

            if (scheduledDay.StartDateTime is null || scheduledDay.EndDateTime is null
                                                   || scheduledDay.CheckInStartDateTime is null
                                                   || scheduledDay.CheckInEndDateTime is null
                                                   || scheduledDay.CheckOutStartDateTime is null
                                                   || scheduledDay.CheckOutEndDateTime is null)
            {
                this.Logger.LogWarning(
                    "Unable to calculate the '{WorkforceMetricCollectionName}\\{WorkforceMetricName}' metric for employee '{EmployeeId}' on scheduled day '{ScheduledDayId}' due to missing required scheduled day data.",
                    this.WorkforceMetricCollectionName,
                    this.WorkforceMetricName,
                    scheduledDay.ScheduledShiftAssignment?.EmployeeId,
                    scheduledDay.Id);

                return Task.FromResult(0.0);
            }

            if (scheduledDay.Timestamps.Count < 2)
            {
                this.Logger.LogWarning(
                    "Unable to calculate the '{WorkforceMetricCollectionName}\\{WorkforceMetricName}' metric for the employee '{EmployeeId}' on scheduled day '{ScheduledDayId}' due to insufficient registered timestamps.",
                    this.WorkforceMetricCollectionName,
                    this.WorkforceMetricName,
                    scheduledDay.ScheduledShiftAssignment?.EmployeeId,
                    scheduledDay.Id);

                return Task.FromResult(0.0);
            }

            var (checkInTime, checkOutTime) = scheduledDay.Timestamps.Select(timestamp => timestamp.DateTime).Aggregate(
                (StartDate: DateTimeOffset.MaxValue, EndDate: DateTimeOffset.MinValue),
                (accumulate, current) => (StartDate: current.Min(accumulate.StartDate),
                                             EndDate: current.Max(accumulate.EndDate)));

            if (checkInTime <= scheduledDay.CheckInEndDateTime)
            {
                checkInTime = scheduledDay.StartDateTime.Value;
            }

            if (checkOutTime >= scheduledDay.CheckOutStartDateTime)
            {
                checkOutTime = scheduledDay.EndDateTime.Value;
            }

            var scale =
                scheduledDay.RelativeEffectiveWorkingTime > TimeSpan.Zero
                && scheduledDay.EffectiveWorkingTime > TimeSpan.Zero
                    ? scheduledDay.RelativeEffectiveWorkingTime.TotalHours
                      / scheduledDay.EffectiveWorkingTime.TotalHours
                    : 1.0d;

            // TODO: Create a computed field with the break-time?
            var breakTime = scheduledDay.EndDateTime.Value.Subtract(scheduledDay.StartDateTime.Value).Subtract(scheduledDay.EffectiveWorkingTime);
            var totalHours = checkOutTime.Subtract(checkInTime).Subtract(breakTime).TotalHours;
            return Task.FromResult(totalHours * scale);
        }
    }
}