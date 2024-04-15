// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceByMacOrSerialNumberSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.Devices
{
    using System;
    using System.Linq;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The device by mac or serial number spec.
    /// </summary>
    public class DeviceByMacOrSerialNumberSpec : ISpecification<Device>
    {
        /// <summary>
        /// The mac address.
        /// </summary>
        private readonly string? macAddress;

        /// <summary>
        /// The serial number.
        /// </summary>
        private readonly string? serialNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceByMacOrSerialNumberSpec"/> class.
        /// </summary>
        /// <param name="macAddress">
        /// The mac Address.
        /// </param>
        /// <param name="serialNumber">
        /// The serial Number.
        /// </param>
        public DeviceByMacOrSerialNumberSpec(string? macAddress, string? serialNumber)
        {
            this.macAddress = macAddress;
            this.serialNumber = serialNumber;
        }

        /// <inheritdoc />
        public Func<IQueryable<Device>, IQueryable<Device>> Build()
        {
            return devices => devices.Where(device => device.MacAddress == this.macAddress || device.SerialNumber == this.serialNumber);
        }
    }
}