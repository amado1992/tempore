// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeUnlinkRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Employees
{
    using MediatR;

    /// <summary>
    /// The employee link request.
    /// </summary>
    public class EmployeeUnlinkRequest : IRequest
    {
        /// <summary>
        /// Gets or sets the employee from device ids.
        /// </summary>
        public List<Guid> EmployeeFromDeviceIds { get; set; }
    }
}