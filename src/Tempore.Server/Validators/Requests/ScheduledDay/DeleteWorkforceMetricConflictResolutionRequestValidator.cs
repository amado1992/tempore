// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteWorkforceMetricConflictResolutionRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.ScheduledDay
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.ScheduledDay;

    /// <summary>
    /// The DeleteWorkforceMetricConflictResolutionRequestValidator.
    /// </summary>
    public class DeleteWorkforceMetricConflictResolutionRequestValidator : AbstractValidator<DeleteWorkforceMetricConflictResolutionRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteWorkforceMetricConflictResolutionRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public DeleteWorkforceMetricConflictResolutionRequestValidator(IStringLocalizer<DeleteWorkforceMetricConflictResolutionRequest> stringLocalizer)
        {
            this.RuleFor(request => request.Id).NotEmpty();
        }
    }
}