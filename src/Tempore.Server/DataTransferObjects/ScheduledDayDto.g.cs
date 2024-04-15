using System;
using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;

namespace Tempore.Server.DataTransferObjects
{
    public partial class ScheduledDayDto
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public List<TimestampDto> Timestamps { get; set; }
        public List<WorkforceMetricConflictResolutionDto> WorkforceMetricConflictResolutions { get; set; }
        public Guid ScheduledShiftAssignmentId { get; set; }
        public ScheduledShiftAssignmentDto ScheduledShiftAssignment { get; set; }
        public Guid DayId { get; set; }
        public DayDto Day { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
        public DateTimeOffset? CheckInStartDateTime { get; set; }
        public DateTimeOffset? CheckInEndDateTime { get; set; }
        public DateTimeOffset? CheckOutStartDateTime { get; set; }
        public DateTimeOffset? CheckOutEndDateTime { get; set; }
        public TimeSpan EffectiveWorkingTime { get; set; }
        public TimeSpan RelativeEffectiveWorkingTime { get; set; }
    }
}