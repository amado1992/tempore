// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeviceInfoRepository.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Services.Interfaces
{
    using Tempore.Agent.Configurations;

    /// <summary>
    /// The DeviceRepository interface.
    /// </summary>
    public interface IDeviceInfoRepository
    {
        /// <summary>
        /// Gets the devices.
        /// </summary>
        IEnumerable<DeviceInfo> Devices { get; }
    }
}