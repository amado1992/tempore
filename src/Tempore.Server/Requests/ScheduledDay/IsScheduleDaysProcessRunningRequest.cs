// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsScheduleDaysProcessRunningRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.ScheduledDay
{
    using MediatR;

    /// <summary>
    /// The IsScheduleDaysRunningRequest.
    /// </summary>
    public class IsScheduleDaysProcessRunningRequest : IRequest<bool>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IsScheduleDaysProcessRunningRequest Instance = new IsScheduleDaysProcessRunningRequest();

        private IsScheduleDaysProcessRunningRequest()
        {
        }
    }
}