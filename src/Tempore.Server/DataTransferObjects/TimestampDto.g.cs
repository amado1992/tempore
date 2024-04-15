using System;
using Tempore.Server.DataTransferObjects;

namespace Tempore.Server.DataTransferObjects
{
    public partial class TimestampDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeFromDeviceId { get; set; }
        public EmployeeFromDeviceDto EmployeeFromDevice { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public Guid? ScheduledDayId { get; set; }
        public ScheduledDayDto? ScheduledDay { get; set; }
    }
}