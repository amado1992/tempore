using System;
using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;

namespace Tempore.Server.DataTransferObjects
{
    public partial class ScheduledShiftAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public EmployeeDto Employee { get; set; }
        public Guid ScheduledShiftId { get; set; }
        public ScheduledShiftDto ScheduledShift { get; set; }
        public List<ScheduledDayDto> ScheduledDays { get; set; }
        public DateOnly? LastGeneratedDayDate { get; set; }
    }
}