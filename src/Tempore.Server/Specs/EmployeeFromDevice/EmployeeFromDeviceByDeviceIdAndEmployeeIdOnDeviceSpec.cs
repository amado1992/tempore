// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeFromDeviceByDeviceIdAndEmployeeIdOnDeviceSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.EmployeeFromDevice
{
    using System;
    using System.Linq;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The employee from device by employee id on device spec.
    /// </summary>
    public class EmployeeFromDeviceByDeviceIdAndEmployeeIdOnDeviceSpec : ISpecification<EmployeeFromDevice>
    {
        /// <summary>
        /// The employee id on device.
        /// </summary>
        private readonly string employeeIdOnDevice;

        /// <summary>
        /// The device id.
        /// </summary>
        private readonly Guid deviceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeFromDeviceByDeviceIdAndEmployeeIdOnDeviceSpec"/> class.
        /// </summary>
        /// <param name="deviceId">
        /// The device id.
        /// </param>
        /// <param name="employeeIdOnDevice">
        /// The employee employee id on device.
        /// </param>
        public EmployeeFromDeviceByDeviceIdAndEmployeeIdOnDeviceSpec(Guid deviceId, string employeeIdOnDevice)
        {
            this.deviceId = deviceId;
            this.employeeIdOnDevice = employeeIdOnDevice;
        }

        /// <inheritdoc />
        public Func<IQueryable<EmployeeFromDevice>, IQueryable<EmployeeFromDevice>> Build()
        {
            return employeeFromDevices => employeeFromDevices.Where(employeeFromDevice => employeeFromDevice.DeviceId == this.deviceId && employeeFromDevice.EmployeeIdOnDevice == this.employeeIdOnDevice);
        }
    }
}