// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceInfo.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Configurations
{
    /// <summary>
    /// The device.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the ip address.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public int Port { get; set; } = 80;

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets the url.
        /// </summary>
        public string Url => $"http://{this.IpAddress}:{this.Port}";

        /// <summary>
        /// Gets or sets the first date online.
        /// </summary>
        public DateTime FirstDateOnline { get; set; } = new DateTime(DateTime.Now.Year - 1, 1, 1);

        /// <summary>
        /// Gets or sets a value indicating whether the device time must be synchornized with agent time.
        /// </summary>
        public bool SynchronizeTime { get; set; } = true;
    }
}