// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWorkforceMetricConflictResolutionsRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.ScheduledDay
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.ScheduledDay;

    /// <summary>
    /// The GetWorkforceMetricConflictResolutionsRequestValidator.
    /// </summary>
    public class GetWorkforceMetricConflictResolutionsRequestValidator : AbstractValidator<GetWorkforceMetricConflictResolutionsRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkforceMetricConflictResolutionsRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public GetWorkforceMetricConflictResolutionsRequestValidator(IStringLocalizer<GetWorkforceMetricConflictResolutionsRequest> stringLocalizer)
        {
            this.RuleFor(request => request.Skip).GreaterThanOrEqualTo(0);
            this.RuleFor(request => request.Take).GreaterThanOrEqualTo(0);
            this.RuleFor(request => request.ScheduledDayId).NotEmpty();
        }
    }
}