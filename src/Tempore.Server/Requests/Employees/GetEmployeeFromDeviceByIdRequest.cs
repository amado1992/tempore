// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmployeeFromDeviceByIdRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Employees
{
    using MediatR;

    using Tempore.Server.DataTransferObjects;

    /// <summary>
    /// The employee from device request.
    /// </summary>
    public class GetEmployeeFromDeviceByIdRequest : IRequest<EmployeeFromDeviceDto>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }
    }
}