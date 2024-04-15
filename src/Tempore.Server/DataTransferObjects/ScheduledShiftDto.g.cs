using System;
using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;

namespace Tempore.Server.DataTransferObjects
{
    public partial class ScheduledShiftDto
    {
        public Guid Id { get; set; }
        public Guid ShiftId { get; set; }
        public ShiftDto Shift { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly ExpireDate { get; set; }
        public TimeSpan EffectiveWorkingTime { get; set; }
        public List<ScheduledShiftAssignmentDto> ScheduledShiftAssignments { get; set; }
        public List<EmployeeDto> Employees { get; set; }
    }
}