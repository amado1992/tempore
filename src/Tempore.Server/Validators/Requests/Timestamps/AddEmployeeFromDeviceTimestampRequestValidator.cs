// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddEmployeeFromDeviceTimestampRequestValidator.cs" company="Port Hope Investment S.A.">
//   Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.Timestamps
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.Employees;

    /// <summary>
    /// The add employees from devices timestamp request validator.
    /// </summary>
    public class AddEmployeeFromDeviceTimestampRequestValidator : AbstractValidator<AddEmployeeFromDeviceTimestampRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddEmployeeFromDeviceTimestampRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public AddEmployeeFromDeviceTimestampRequestValidator(IStringLocalizer<AddEmployeeFromDeviceTimestampRequestValidator> stringLocalizer)
        {
            ArgumentNullException.ThrowIfNull(stringLocalizer);

            this.RuleFor(request => request.Timestamp)
                .ChildRules(timestampValidator =>
                {
                    timestampValidator
                        .RuleFor(timestamp => timestamp.DateTime)
                        .NotEmpty();

                    timestampValidator
                        .RuleFor(timestamp => timestamp.EmployeeFromDevice)
                        .ChildRules(employeeFromDeviceValidator =>
                        {
                            employeeFromDeviceValidator
                            .RuleFor(employeeFromDevice => employeeFromDevice.EmployeeIdOnDevice)
                            .NotEmpty();
                        }).NotNull();
                })
                .NotNull();
        }
    }
}