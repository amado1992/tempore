// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmployeesFromDevicesRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Employees
{
    using Tempore.Server.DataTransferObjects;

    /// <summary>
    /// The employees from devices request.
    /// </summary>
    public class GetEmployeesFromDevicesRequest : PagedRequest<EmployeeFromDeviceDto>
    {
        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        public string? SearchText { get; set; }

        /// <summary>
        /// Gets or sets the is linked.
        /// </summary>
        public bool? IsLinked { get; set; }

        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the is include.
        /// </summary>
        public bool? IncludeDevice { get; set; }

        /// <summary>
        /// Gets or sets the is include.
        /// </summary>
        public bool? IncludeAgent { get; set; }

        /// <summary>
        /// Gets or sets the DiviceIds.
        /// </summary>
        public List<Guid>? DeviceIds { get; set; }
    }
}