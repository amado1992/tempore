// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetShiftByIdRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Shifts
{
    using MediatR;

    using Tempore.Server.DataTransferObjects;

    /// <summary>
    /// The employee request.
    /// </summary>
    public class GetShiftByIdRequest : IRequest<ShiftDto>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }
    }
}