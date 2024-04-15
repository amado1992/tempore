using System.Collections.Generic;
using Tempore.Server.DataTransferObjects;
using Tempore.Storage.Entities;

namespace Tempore.Server.DataTransferObjects
{
    public partial class AgentRegistrationDto
    {
        public string Name { get; set; }
        public AgentState State { get; set; }
        public List<DeviceRegistrationDto> Devices { get; set; }
        public string? ConnectionId { get; set; }
    }
}