// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddEmployeeFromDeviceTimestampRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Client
{
    /// <summary>
    /// The AddEmployeeFromDeviceRequest class.
    /// </summary>
    public partial class AddEmployeeFromDeviceTimestampRequest
    {
        /// <summary>
        /// Creates an instance of <see cref="AddEmployeeFromDeviceTimestampRequest"/>.
        /// </summary>
        /// <param name="deviceId">
        /// The device id.
        /// </param>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <param name="employeeIdOnDevice">
        /// The employee id on device.
        /// </param>
        /// <returns>
        /// An instance of <see cref="AddEmployeeFromDeviceTimestampRequest"/>.
        /// </returns>
        public static AddEmployeeFromDeviceTimestampRequest Create(Guid deviceId, DateTimeOffset dateTime, string employeeIdOnDevice)
        {
            var employeeFromDevice = new EmployeeFromDeviceDto
            {
                DeviceId = deviceId,
                EmployeeIdOnDevice = employeeIdOnDevice,
            };

            var timestamp = new TimestampDto
            {
                DateTime = dateTime,
                EmployeeFromDevice = employeeFromDevice,
            };

            var request = new AddEmployeeFromDeviceTimestampRequest
            {
                Timestamp = timestamp,
            };

            return request;
        }

        /// <summary>
        /// Creates an instance of <see cref="AddEmployeeFromDeviceTimestampRequest"/>.
        /// </summary>
        /// <param name="timestamp">
        /// The timestamp.
        /// </param>
        /// <returns>
        /// An instance of <see cref="AddEmployeeFromDeviceTimestampRequest"/>.
        /// </returns>
        public static AddEmployeeFromDeviceTimestampRequest Create(TimestampDto timestamp)
        {
            return new AddEmployeeFromDeviceTimestampRequest
            {
                Timestamp = timestamp,
            };
        }
    }
}