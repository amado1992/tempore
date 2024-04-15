// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduleDaysRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.ScheduledDay
{
    using MediatR;

    /// <summary>
    /// The schedule days request.
    /// </summary>
    public class ScheduleDaysRequest : IRequest
    {
        /// <summary>
        /// Gets or sets the shift id.
        /// </summary>
        public Guid ScheduledShiftId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether force.
        /// </summary>
        public bool Force { get; set; }
    }
}