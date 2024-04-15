using System;
using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;

namespace Tempore.Server.DataTransferObjects
{
    public partial class ShiftDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<DayDto> Days { get; set; }
    }
}