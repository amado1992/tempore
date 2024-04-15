using System;
using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;

namespace Tempore.Server.DataTransferObjects
{
    public partial class TimetableDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan CheckInTimeStart { get; set; }
        public TimeSpan CheckInTimeDuration { get; set; }
        public TimeSpan CheckOutTimeStart { get; set; }
        public TimeSpan CheckOutTimeDuration { get; set; }
        public TimeSpan EffectiveWorkingTime { get; set; }
        public List<DayDto> Days { get; set; }
    }
}