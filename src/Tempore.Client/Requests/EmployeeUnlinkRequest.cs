// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeUnlinkRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Client
{
    /// <summary>
    /// The employee unlink request.
    /// </summary>
    public partial class EmployeeUnlinkRequest
    {
        /// <summary>
        /// Creates an instance of <see cref="EmployeeUnlinkRequest"/>.
        /// </summary>
        /// <param name="employeeFromDeviceIds">
        /// The employee from device ids.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EmployeeUnlinkRequest"/>.
        /// </returns>
        public static EmployeeUnlinkRequest Create(params Guid[] employeeFromDeviceIds)
        {
            return new EmployeeUnlinkRequest
            {
                EmployeeFromDeviceIds = employeeFromDeviceIds?.ToList(),
            };
        }
    }
}