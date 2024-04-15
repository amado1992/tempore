// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsLinkEmployeesProcessRunningRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Employees
{
    using MediatR;

    public class IsLinkEmployeesProcessRunningRequest : IRequest<bool>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IsLinkEmployeesProcessRunningRequest Instance = new();

        /// <summary>
        /// Prevents a default instance of the <see cref="IsLinkEmployeesProcessRunningRequest"/> class from being created.
        /// </summary>
        private IsLinkEmployeesProcessRunningRequest()
        {
        }
    }
}