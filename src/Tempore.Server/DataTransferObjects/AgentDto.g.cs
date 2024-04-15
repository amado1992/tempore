using System;
using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;
using Tempore.Storage.Entities;

namespace Tempore.Server.DataTransferObjects
{
    public partial class AgentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public AgentState State { get; set; }
        public List<DeviceDto> Devices { get; set; }
        public string? ConnectionId { get; set; }
    }
}