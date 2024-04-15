// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddEmployeeFromDeviceTimestampRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Employees
{
    using MediatR;

    using Tempore.Server.DataTransferObjects;

    /// <summary>
    /// Adds employee from device timestamp request.
    /// </summary>
    public class AddEmployeeFromDeviceTimestampRequest : IRequest<Guid>
    {
        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public TimestampDto Timestamp { get; set; }
    }
}