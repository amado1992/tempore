// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignEmployeesToScheduledShiftRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Employees
{
    using MediatR;

    /// <summary>
    /// The assign employees scheduled shift request.
    /// </summary>
    public class AssignEmployeesToScheduledShiftRequest : IRequest<Guid>
    {
        /// <summary>
        /// Gets or sets the employee ids.
        /// </summary>
        public List<Guid> EmployeeIds { get; set; }

        /// <summary>
        /// Gets or sets the shift id.
        /// </summary>
        public Guid ShiftId { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Gets or sets the expire date.
        /// </summary>
        public DateOnly ExpireDate { get; set; }

        /// <summary>
        /// Gets or sets the effective working time.
        /// </summary>
        public TimeSpan EffectiveWorkingTime { get; set; }
    }
}