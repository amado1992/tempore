// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddEmployeesFromDeviceRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.Employees
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.Employees;

    /// <summary>
    /// The employees from devices request validator.
    /// </summary>
    public class AddEmployeeFromDeviceRequestValidator : AbstractValidator<AddEmployeeFromDeviceRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddEmployeeFromDeviceRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public AddEmployeeFromDeviceRequestValidator(IStringLocalizer<AddEmployeeFromDeviceRequestValidator> stringLocalizer)
        {
            this.RuleFor(request => request.Employee)
                .ChildRules(rules =>
                {
                    rules.RuleFor(dto => dto.FullName).NotEmpty();
                    rules.RuleFor(dto => dto.DeviceId).NotEmpty();
                    rules.RuleFor(dto => dto.EmployeeIdOnDevice).NotEmpty();
                })
                .NotNull();
        }
    }
}