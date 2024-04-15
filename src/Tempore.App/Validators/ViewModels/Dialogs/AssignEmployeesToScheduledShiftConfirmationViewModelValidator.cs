// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignEmployeesToScheduledShiftConfirmationViewModelValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Validators.ViewModels.Dialogs
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.App.ViewModels.Dialogs.Shifts;

    /// <summary>
    /// The assign shift to employees request view model validator.
    /// </summary>
    public class AssignEmployeesToScheduledShiftConfirmationViewModelValidator : AbstractValidator<AssignEmployeesToScheduledShiftConfirmationViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignEmployeesToScheduledShiftConfirmationViewModelValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string Localizer.
        /// </param>
        public AssignEmployeesToScheduledShiftConfirmationViewModelValidator(IStringLocalizer<AssignEmployeesToScheduledShiftConfirmationViewModelValidator> stringLocalizer)
        {
            this.RuleFor(request => request.StartDate)
                .NotNull()
                .WithName(stringLocalizer["Start date"]);
            this.RuleFor(request => request.ExpireDate)
                .NotNull()
                .GreaterThanOrEqualTo(vm => vm.StartDate)
                .WithName(stringLocalizer["Expire date"]);
            this.RuleFor(request => request.EffectiveWorkingHours)
                .NotNull()
                .GreaterThan(0)
                .WithName(stringLocalizer["Effective working hours"]);
        }
    }
}