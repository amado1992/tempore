// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnAssignShiftToEmployeesRequestValidator.cs" company="Port Hope Investment S.A.">
//   Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.Shifts
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.Employees;

    /// <summary>
    /// The unassign shift to employees request validator.
    /// </summary>
    public class UnAssignShiftToEmployeesRequestValidator : AbstractValidator<UnAssignShiftToEmployeesRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnAssignShiftToEmployeesRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public UnAssignShiftToEmployeesRequestValidator(IStringLocalizer<UnAssignShiftToEmployeesRequestValidator> stringLocalizer)
        {
            ArgumentNullException.ThrowIfNull(stringLocalizer);
            this.RuleForEach(request => request.ShiftToEmployeeIds).NotEmpty();
        }
    }
}