using System;
using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;

namespace Tempore.Server.DataTransferObjects
{
    public partial class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string ExternalId { get; set; }
        public List<EmployeeFromDeviceDto> EmployeeFromDevice { get; set; }
        public string? IdentificationCard { get; set; }
        public string? SocialSecurity { get; set; }
        public string? Department { get; set; }
        public string? CostCenter { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public float? BaseHours { get; set; }
        public List<ScheduledShiftAssignmentDto> ScheduledShiftAssignments { get; set; }
        public List<ScheduledShiftDto> ScheduledShifts { get; set; }
    }
}