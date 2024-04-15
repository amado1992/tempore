// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadEmployeesFromDevicesRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Client
{
    /// <summary>
    /// The UploadEmployeesFromDevicesRequest class.
    /// </summary>
    public partial class UploadEmployeesFromDevicesRequest
    {
        /// <summary>
        /// Creates an instance of <see cref="UploadEmployeesFromDevicesRequest"/>.
        /// </summary>
        /// <param name="deviceIds">
        /// The device ids.
        /// </param>
        /// <returns>
        /// An instance of <see cref="UploadEmployeesFromDevicesRequest"/>.
        /// </returns>
        public static UploadEmployeesFromDevicesRequest Create(params Guid[] deviceIds)
        {
            return new UploadEmployeesFromDevicesRequest { DeviceIds = deviceIds?.ToList() };
        }
    }
}