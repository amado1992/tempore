using System;
using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;

namespace Tempore.Server.DataTransferObjects
{
    public partial class DayDto
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public Guid? TimetableId { get; set; }
        public TimetableDto? Timetable { get; set; }
        public Guid ShiftId { get; set; }
        public ShiftDto Shift { get; set; }
        public List<ScheduledDayDto> ScheduledDays { get; set; }
    }
}