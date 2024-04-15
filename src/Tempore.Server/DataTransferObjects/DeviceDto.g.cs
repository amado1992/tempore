using System;
using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;
using Tempore.Storage.Entities;

namespace Tempore.Server.DataTransferObjects
{
    public partial class DeviceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? DeviceName { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? MacAddress { get; set; }
        public AgentDto Agent { get; set; }
        public Guid AgentId { get; set; }
        public DeviceState State { get; set; }
        public List<EmployeeFromDeviceDto> EmployeesFromDevices { get; set; }
    }
}