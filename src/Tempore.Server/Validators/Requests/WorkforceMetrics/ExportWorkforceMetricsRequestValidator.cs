// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportWorkforceMetricsRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.WorkforceMetrics
{
    using FluentValidation;

    using Microsoft.Extensions.Localization;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Processing.Services.Interfaces;
    using Tempore.Server.Requests.WorkforceMetrics;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The ExportWorkforceMetricsRequestValidator.
    /// </summary>
    public class ExportWorkforceMetricsRequestValidator : AbstractValidator<ExportWorkforceMetricsRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportWorkforceMetricsRequestValidator"/> class.
        /// </summary>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        /// <param name="workforceMetricCollectionSchemaProviderFactory">
        /// The workforce metric collection schema provider factory.
        /// </param>
        /// <param name="workforceMetricCollectionRepository">
        /// The workforce metric collection repository.
        /// </param>
        public ExportWorkforceMetricsRequestValidator(
            IStringLocalizer<ExportWorkforceMetricsRequest> stringLocalizer,
            IWorkforceMetricCollectionSchemaProviderFactory workforceMetricCollectionSchemaProviderFactory,
            IRepository<WorkforceMetricCollection, ApplicationDbContext> workforceMetricCollectionRepository)
        {
            this.RuleFor(request => request.FileFormat).IsInEnum();
            this.RuleFor(request => request.StartDate).NotEmpty();
            this.RuleFor(request => request.EndDate).NotEmpty().GreaterThanOrEqualTo(request => request.StartDate);
            this.RuleFor(request => request.WorkforceMetricCollectionId).NotEmpty()
                .MustAsync(async (id, token) =>
                {
                    var workforceMetricCollection =
                        await workforceMetricCollectionRepository.SingleOrDefaultAsync(
                            collection => collection.Id == id);

                    if (workforceMetricCollection is null)
                    {
                        return true;
                    }

                    return workforceMetricCollectionSchemaProviderFactory.IsSupported(workforceMetricCollection.Name);
                });
        }
    }
}