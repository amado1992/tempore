// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddWorkforceMetricConflictResolutionRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.ScheduledDay
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.ScheduledDay;

    /// <summary>
    /// The AddWorkforceMetricConflictResolutionRequestValidator.
    /// </summary>
    public class AddWorkforceMetricConflictResolutionRequestValidator : AbstractValidator<AddWorkforceMetricConflictResolutionRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddWorkforceMetricConflictResolutionRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public AddWorkforceMetricConflictResolutionRequestValidator(IStringLocalizer<AddWorkforceMetricConflictResolutionRequest> stringLocalizer)
        {
            this.RuleFor(request => request.ScheduledDayId).NotEmpty();
            this.RuleFor(request => request.WorkforceMetricId).NotEmpty();
            this.RuleFor(request => request.Value).NotEmpty().GreaterThanOrEqualTo(0);
        }
    }
}