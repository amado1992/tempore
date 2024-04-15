// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignScheduledDayRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.ScheduledDay
{
    using MediatR;

    public class AssignScheduledDayRequest : IRequest
    {
        public Guid ScheduledShift { get; set; }
    }
}