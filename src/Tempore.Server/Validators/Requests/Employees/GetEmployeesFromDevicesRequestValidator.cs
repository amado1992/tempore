// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEmployeesFromDevicesRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.Employees
{
    using DocumentFormat.OpenXml.Math;

    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using GetEmployeesFromDevicesRequest = Tempore.Server.Requests.Employees.GetEmployeesFromDevicesRequest;

    /// <summary>
    /// The employees from devices request validator.
    /// </summary>
    public class GetEmployeesFromDevicesRequestValidator : AbstractValidator<GetEmployeesFromDevicesRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmployeesFromDevicesRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public GetEmployeesFromDevicesRequestValidator(IStringLocalizer<GetEmployeesFromDevicesRequest> stringLocalizer)
        {
            this.RuleFor(request => request.Skip).GreaterThanOrEqualTo(0);
            this.RuleFor(request => request.Take).GreaterThanOrEqualTo(0);
        }
    }
}