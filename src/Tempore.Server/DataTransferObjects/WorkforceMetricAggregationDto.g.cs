using System;

namespace Tempore.Server.DataTransferObjects
{
    public partial class WorkforceMetricAggregationDto
    {
        public double Value { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid WorkforceMetricId { get; set; }
        public string EmployeeName { get; set; }
        public string WorkforceMetricName { get; set; }
        public string ExternalId { get; set; }
    }
}