// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComputeWorkforceMetricsRequestValidator.cs" company="Port Hope Investment S.A.">
//   Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.Timestamps
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.WorkforceMetrics;

    /// <summary>
    /// The compute workforce metrics request validator.
    /// </summary>
    public class ComputeWorkforceMetricsRequestValidator : AbstractValidator<ComputeWorkforceMetricsRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputeWorkforceMetricsRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public ComputeWorkforceMetricsRequestValidator(IStringLocalizer<ComputeWorkforceMetricsRequestValidator> stringLocalizer)
        {
            ArgumentNullException.ThrowIfNull(stringLocalizer);

            this.RuleFor(request => request.StartDate).NotEmpty();
            this.RuleFor(request => request.EndDate)
                .GreaterThanOrEqualTo(request => request.StartDate)
                .NotEmpty();

            this.RuleFor(request => request.WorkForceMetricCollectionIds).NotEmpty();

            this.RuleForEach(request => request.WorkForceMetricCollectionIds).NotEmpty();
        }
    }
}