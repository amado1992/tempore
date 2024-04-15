// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddEmployeeFromDeviceRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Client
{
    /// <summary>
    /// The AddEmployeeFromDeviceRequest class.
    /// </summary>
    public partial class AddEmployeeFromDeviceRequest
    {
        /// <summary>
        /// Creates an instance of <see cref="AddEmployeeFromDeviceRequest"/>.
        /// </summary>
        /// <param name="deviceId">
        /// The device id.
        /// </param>
        /// <param name="employeeIdOnDevice">
        /// The employee id on device.
        /// </param>
        /// <param name="fullName">
        /// The full name.
        /// </param>
        /// <returns>
        /// An instance of <see cref="AddEmployeeFromDeviceRequest"/>.
        /// </returns>
        public static AddEmployeeFromDeviceRequest Create(Guid deviceId, string employeeIdOnDevice, string fullName)
        {
            var employeeFromDevice = new EmployeeFromDeviceDto
            {
                DeviceId = deviceId,
                EmployeeIdOnDevice = employeeIdOnDevice,
                FullName = fullName,
            };

            var request = new AddEmployeeFromDeviceRequest
            {
                Employee = employeeFromDevice,
            };

            return request;
        }

        /// <summary>
        /// Creates an instance of <see cref="AddEmployeeFromDeviceRequest"/>.
        /// </summary>
        /// <param name="employeeFromDeviceDto">
        /// The employee from device.
        /// </param>
        /// <returns>
        /// An instance of <see cref="AddEmployeeFromDeviceRequest"/>.
        /// </returns>
        public static AddEmployeeFromDeviceRequest Create(EmployeeFromDeviceDto employeeFromDeviceDto)
        {
            return new AddEmployeeFromDeviceRequest
            {
                Employee = employeeFromDeviceDto,
            };
        }
    }
}