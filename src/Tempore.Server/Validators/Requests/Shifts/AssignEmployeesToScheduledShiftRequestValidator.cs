// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignEmployeesToScheduledShiftRequestValidator.cs" company="Port Hope Investment S.A.">
//   Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.Shifts
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.Employees;

    /// <summary>
    /// The employees to scheduled shift request validator.
    /// </summary>
    public class AssignEmployeesToScheduledShiftRequestValidator : AbstractValidator<AssignEmployeesToScheduledShiftRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignEmployeesToScheduledShiftRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public AssignEmployeesToScheduledShiftRequestValidator(IStringLocalizer<AssignEmployeesToScheduledShiftRequestValidator> stringLocalizer)
        {
            ArgumentNullException.ThrowIfNull(stringLocalizer);

            this.RuleFor(request => request.ShiftId).NotEmpty();
            this.RuleFor(request => request.EmployeeIds).NotEmpty();
            this.RuleFor(request => request.StartDate).NotNull();
            this.RuleFor(request => request.ExpireDate).NotNull().GreaterThanOrEqualTo(request => request.StartDate);
            this.RuleFor(request => request.EffectiveWorkingTime).NotNull();

            this.RuleForEach(request => request.EmployeeIds).NotEmpty();
        }
    }
}