// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkEmployeesRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Employees
{
    using MediatR;

    /// <summary>
    /// The link employees notification.
    /// </summary>
    public class LinkEmployeesRequest : IRequest<Guid>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly LinkEmployeesRequest Instance = new();

        /// <summary>
        /// Prevents a default instance of the <see cref="LinkEmployeesRequest"/> class from being created.
        /// </summary>
        private LinkEmployeesRequest()
        {
        }
    }
}