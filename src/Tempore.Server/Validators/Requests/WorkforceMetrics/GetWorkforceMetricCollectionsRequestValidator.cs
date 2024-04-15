// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWorkforceMetricCollectionsRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.WorkforceMetrics
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.Employees;
    using Tempore.Server.Requests.WorkforceMetrics;

    public class GetWorkforceMetricCollectionsRequestValidator : AbstractValidator<GetWorkforceMetricCollectionsRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkforceMetricCollectionsRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public GetWorkforceMetricCollectionsRequestValidator(IStringLocalizer<GetEmployeesFromDevicesRequest> stringLocalizer)
        {
            this.RuleFor(request => request.Skip).GreaterThanOrEqualTo(0);
            this.RuleFor(request => request.Take).GreaterThanOrEqualTo(0);
        }
    }
}