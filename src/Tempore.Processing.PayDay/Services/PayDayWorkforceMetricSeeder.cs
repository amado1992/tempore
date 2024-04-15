// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayDayWorkforceMetricSeeder.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.PayDay.Services
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Processing.PayDay;
    using Tempore.Processing.Services.Interfaces;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The pay day workforce metric seeder.
    /// </summary>
    public class PayDayWorkforceMetricSeeder : IWorkforceMetricSeeder
    {
        private readonly ILogger<PayDayWorkforceMetricSeeder> logger;

        private readonly IWorkforceMetricCollectionSchemaProviderFactory workforceMetricCollectionSchemaProviderFactory;

        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayDayWorkforceMetricSeeder"/> class.
        /// </summary>
        /// <param name="logger">
        ///     The logger.
        /// </param>
        /// <param name="workforceMetricCollectionSchemaProviderFactory">
        ///     The workforce metric collection schema provider factory.
        /// </param>
        /// <param name="serviceProvider">
        ///     The service provider.
        /// </param>
        public PayDayWorkforceMetricSeeder(
            ILogger<PayDayWorkforceMetricSeeder> logger,
            IWorkforceMetricCollectionSchemaProviderFactory workforceMetricCollectionSchemaProviderFactory,
            IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(workforceMetricCollectionSchemaProviderFactory);

            this.logger = logger;
            this.workforceMetricCollectionSchemaProviderFactory = workforceMetricCollectionSchemaProviderFactory;
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public async Task SeedAsync()
        {
            this.workforceMetricCollectionSchemaProviderFactory.Register<PayDayWorkforceMetricCollectionSchemaProvider>(PayDayWorkforceMetricCollection.Name);

            using var serviceScope = this.serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork<ApplicationDbContext>>();

            var workforceMetricCollectionRepository = unitOfWork.GetRepository<WorkforceMetricCollection>();
            var workforceMetricCollection = await workforceMetricCollectionRepository.SingleOrDefaultAsync(collection => collection.Name == PayDayWorkforceMetricCollection.Name);
            if (workforceMetricCollection is null)
            {
                workforceMetricCollection = new WorkforceMetricCollection
                {
                    Name = PayDayWorkforceMetricCollection.Name,
                };

                workforceMetricCollectionRepository.Add(workforceMetricCollection);
                await workforceMetricCollectionRepository.SaveChangesAsync();
            }

            var workforceMetricRepository = unitOfWork.GetRepository<WorkforceMetric>();
            foreach (var workforceMetricName in PayDayWorkforceMetrics.All)
            {
                var workforceMetric = await workforceMetricRepository.SingleOrDefaultAsync(metric => metric.Name == workforceMetricName);
                if (workforceMetric is null)
                {
                    workforceMetric = new WorkforceMetric
                    {
                        Name = workforceMetricName,
                        WorkforceMetricCollectionId = workforceMetricCollection.Id,
                    };

                    workforceMetricRepository.Add(workforceMetric);
                }
                else
                {
                    workforceMetric.WorkforceMetricCollectionId = workforceMetricCollection.Id;
                    workforceMetricRepository.Update(workforceMetric);
                }
            }

            await workforceMetricRepository.SaveChangesAsync();
        }
    }
}