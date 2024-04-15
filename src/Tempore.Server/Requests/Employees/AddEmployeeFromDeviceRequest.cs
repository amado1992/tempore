// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddEmployeeFromDeviceRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Employees
{
    using MediatR;

    using Tempore.Server.DataTransferObjects;

    /// <summary>
    /// The load employee from device request.
    /// </summary>
    public class AddEmployeeFromDeviceRequest : IRequest<Guid>
    {
        /// <summary>
        /// Gets or sets the employee.
        /// </summary>
        public EmployeeFromDeviceDto Employee { get; set; }
    }
}