using System;

namespace Tempore.Server.DataTransferObjects
{
    public partial class ScheduledShiftOverviewDto
    {
        public Guid Id { get; set; }
        public Guid ShiftId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly ExpireDate { get; set; }
        public TimeSpan EffectiveWorkingTime { get; set; }
        public int EmployeesCount { get; set; }
    }
}