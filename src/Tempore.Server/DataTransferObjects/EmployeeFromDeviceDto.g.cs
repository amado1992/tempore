using System;
using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;

namespace Tempore.Server.DataTransferObjects
{
    public partial class EmployeeFromDeviceDto
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public DeviceDto Device { get; set; }
        public string EmployeeIdOnDevice { get; set; }
        public string FullName { get; set; }
        public Guid? EmployeeId { get; set; }
        public EmployeeDto? Employee { get; set; }
        public bool IsLinked { get; set; }
        public List<TimestampDto> Timestamps { get; set; }
    }
}