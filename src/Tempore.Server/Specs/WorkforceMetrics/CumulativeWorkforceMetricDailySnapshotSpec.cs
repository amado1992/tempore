// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CumulativeWorkforceMetricDailySnapshotSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.WorkforceMetrics
{
    using Microsoft.EntityFrameworkCore;

    using Tempore.Storage.Entities;

    using WorkforceMetricAggregation = Tempore.Storage.Entities.WorkforceMetricAggregation;

    /// <summary>
    /// The CumulativeWorkforceMetricDailySnapshotSpec.
    /// </summary>
    public class CumulativeWorkforceMetricDailySnapshotSpec : Specification<WorkforceMetricDailySnapshot, WorkforceMetricAggregation>
    {
        /// <summary>
        /// The start date.
        /// </summary>
        private readonly DateOnly startDate;

        /// <summary>
        /// The end date.
        /// </summary>
        private readonly DateOnly endDate;

        /// <summary>
        /// The workforce metric collection id.
        /// </summary>
        private readonly Guid workforceMetricCollectionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="CumulativeWorkforceMetricDailySnapshotSpec"/> class.
        /// </summary>
        /// <param name="workforceMetricCollectionId">
        ///     The workforce metric collection id.
        /// </param>
        /// <param name="startDate">
        ///     The start date.
        /// </param>
        /// <param name="endDate">
        ///     The end date.
        /// </param>
        /// <param name="options">
        ///     The pagination options.
        /// </param>
        public CumulativeWorkforceMetricDailySnapshotSpec(
            Guid workforceMetricCollectionId,
            DateOnly startDate,
            DateOnly endDate,
            PaginationOptions options)
            : base(options)
        {
            this.workforceMetricCollectionId = workforceMetricCollectionId;
            this.startDate = startDate;
            this.endDate = endDate;
        }

        /// <inheritdoc />
        protected override Func<IQueryable<WorkforceMetricDailySnapshot>, IQueryable<WorkforceMetricAggregation>> BuildSpec()
        {
            return snapshots => snapshots.Include(snapshot => snapshot.ScheduledDay)
                .ThenInclude(day => day.ScheduledShiftAssignment).ThenInclude(assignment => assignment.Employee)
                .Where(snapshot => snapshot.ScheduledDay.Date >= this.startDate
                                && snapshot.ScheduledDay.Date <= this.endDate
                                && snapshot.WorkforceMetric.WorkforceMetricCollectionId == this.workforceMetricCollectionId)
                .GroupBy(snapshot => new
                {
                    snapshot.WorkforceMetricId,
                    snapshot.ScheduledDay.ScheduledShiftAssignment.EmployeeId,
                })
                .Select(dailySnapshots => new WorkforceMetricAggregation
                {
                    EmployeeId = dailySnapshots.Select(snapshot => snapshot.ScheduledDay.ScheduledShiftAssignment.EmployeeId).First(),
                    ExternalId = dailySnapshots.Select(snapshot => snapshot.ScheduledDay.ScheduledShiftAssignment.Employee.ExternalId).First(),
                    EmployeeName = dailySnapshots.Select(snapshot => snapshot.ScheduledDay.ScheduledShiftAssignment.Employee.FullName).First(),
                    WorkforceMetricId = dailySnapshots.Select(snapshot => snapshot.WorkforceMetricId).First(),
                    WorkforceMetricName = dailySnapshots.Select(snapshot => snapshot.WorkforceMetric.Name).First(),
                    Value = dailySnapshots.Sum(snapshot => snapshot.Value),
                })
                .OrderBy(aggregation => aggregation.EmployeeId);
        }
    }
}