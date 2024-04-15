using Tempore.Storage.Entities;

namespace Tempore.Server.DataTransferObjects
{
    public partial class DeviceRegistrationDto
    {
        public string Name { get; set; }
        public string? DeviceName { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? MacAddress { get; set; }
        public DeviceState State { get; set; }
    }
}