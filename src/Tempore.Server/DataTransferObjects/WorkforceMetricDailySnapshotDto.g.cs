using System;
using Tempore.Server.DataTransferObjects;

namespace Tempore.Server.DataTransferObjects
{
    public partial class WorkforceMetricDailySnapshotDto
    {
        public Guid Id { get; set; }
        public Guid ScheduledDayId { get; set; }
        public ScheduledDayDto ScheduledDay { get; set; }
        public Guid WorkforceMetricId { get; set; }
        public WorkforceMetricDto WorkforceMetric { get; set; }
        public double Value { get; set; }
    }
}