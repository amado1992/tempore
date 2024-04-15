// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWorkforceMetricsSchemaRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.WorkforceMetrics
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using Tempore.Server.Requests.WorkforceMetrics;

    /// <summary>
    /// The GetWorkforceMetricsRequestValidator.
    /// </summary>
    public class GetWorkforceMetricsSchemaRequestValidator : AbstractValidator<GetWorkforceMetricsSchemaRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkforceMetricsSchemaRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public GetWorkforceMetricsSchemaRequestValidator(IStringLocalizer<GetWorkforceMetricsSchemaRequest> stringLocalizer)
        {
            this.RuleFor(request => request.WorkforceMetricCollectionId).NotEmpty();
            this.RuleFor(request => request.SchemaType).IsInEnum();
        }
    }
}