using System;

namespace Tempore.Server.DataTransferObjects
{
    public partial class ScheduledShiftEmployeeDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public bool IsAssigned { get; set; }
    }
}