// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWorkforceMetricsRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.WorkforceMetrics
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Processing.Services;
    using Tempore.Processing.Services.Interfaces;
    using Tempore.Server.Requests.WorkforceMetrics;

    /// <summary>
    /// The GetWorkforceMetricsRequestValidator.
    /// </summary>
    public class GetWorkforceMetricsRequestValidator : AbstractValidator<GetWorkforceMetricsRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkforceMetricsRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public GetWorkforceMetricsRequestValidator(IStringLocalizer<GetWorkforceMetricsRequest> stringLocalizer)
        {
            this.RuleFor(request => request.WorkforceMetricCollectionId).NotEmpty();
            this.RuleFor(request => request.StartDate).NotEmpty();
            this.RuleFor(request => request.EndDate).NotEmpty().GreaterThanOrEqualTo(request => request.StartDate);
            this.RuleFor(request => request.Skip).GreaterThanOrEqualTo(0);
            this.RuleFor(request => request.Take).GreaterThanOrEqualTo(0);
        }
    }
}