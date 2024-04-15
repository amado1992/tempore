// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnAssignShiftToEmployeesRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Employees
{
    using MediatR;

    /// <summary>
    /// The unassign shift to employees request.
    /// </summary>
    public class UnAssignShiftToEmployeesRequest : IRequest
    {
        /// <summary>
        /// Gets or sets the shift to employees ids.
        /// </summary>
        public List<Guid> ShiftToEmployeeIds { get; set; }
    }
}