// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetShiftRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Shifts
{
    using Tempore.Server.DataTransferObjects;

    /// <summary>
    /// The shift request.
    /// </summary>
    public class GetShiftRequest : PagedRequest<ShiftDto>
    {
        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        public string? SearchText { get; set; }

        /// <summary>
        /// Gets or sets the is start date.
        /// </summary>
        public DateOnly? StartDate { get; set; }
    }
}