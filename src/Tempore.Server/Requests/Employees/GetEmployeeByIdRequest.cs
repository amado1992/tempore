// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmployeeByIdRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Employees
{
    using MediatR;

    using Tempore.Server.DataTransferObjects;

    /// <summary>
    /// The employee request.
    /// </summary>
    public class GetEmployeeByIdRequest : IRequest<EmployeeDto>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }
    }
}