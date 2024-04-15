// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeLinkRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Client
{
    /// <summary>
    /// The employee link request.
    /// </summary>
    public partial class EmployeeLinkRequest
    {
        /// <summary>
        /// Creates an instance of <see cref="EmployeeLinkRequest"/>.
        /// </summary>
        /// <param name="employeeId">
        /// The employee Id.
        /// </param>
        /// <param name="employeeFromDeviceIds">
        /// The employee from device ids.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EmployeeLinkRequest"/>.
        /// </returns>
        public static EmployeeLinkRequest Create(Guid employeeId, params Guid[] employeeFromDeviceIds)
        {
            return new EmployeeLinkRequest
            {
                EmployeeId = employeeId,
                EmployeeFromDeviceIds = employeeFromDeviceIds?.ToList(),
            };
        }
    }
}