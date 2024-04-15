// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricCollectionSchemaProviderServiceFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.Services
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Tempore.Processing.Services.Interfaces;

    /// <summary>
    /// The MetricCollectionSchemaProviderServiceFactory.
    /// </summary>
    public class WorkforceMetricCollectionSchemaProviderFactory : IWorkforceMetricCollectionSchemaProviderFactory
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<WorkforceMetricCollectionSchemaProviderFactory> logger;

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// The services.
        /// </summary>
        private readonly Dictionary<string, Type> services = new Dictionary<string, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkforceMetricCollectionSchemaProviderFactory"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        public WorkforceMetricCollectionSchemaProviderFactory(
            ILogger<WorkforceMetricCollectionSchemaProviderFactory> logger, IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(serviceProvider)
                ;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates an implementation of <see cref="IWorkforceMetricCollectionSchemaProvider"/>.
        /// </summary>
        /// <param name="workforceMetricCollectionName">
        /// The workforce metric collection name.
        /// </param>
        /// <returns>
        /// The <see cref="IWorkforceMetricCollectionSchemaProvider"/>.
        /// </returns>
        public IWorkforceMetricCollectionSchemaProvider Create(string workforceMetricCollectionName)
        {
            if (!this.services.TryGetValue(workforceMetricCollectionName, out var workforceMetricCollectionSchemaProviderType))
            {
                // TODO: return default?
                throw new NotSupportedException($"The workforce metric collection name '{workforceMetricCollectionName}' is not supported. Register it first.");
            }

            return (IWorkforceMetricCollectionSchemaProvider)ActivatorUtilities.CreateInstance(this.serviceProvider, workforceMetricCollectionSchemaProviderType);
        }

        /// <summary>
        /// Register a service type.
        /// </summary>
        /// <param name="workforceMetricCollectionName">
        /// The workforce metric collection name.
        /// </param>
        /// <typeparam name="TServiceType">
        /// The service type.
        /// </typeparam>
        public void Register<TServiceType>(string workforceMetricCollectionName)
            where TServiceType : IWorkforceMetricCollectionSchemaProvider
        {
            var serviceType = typeof(TServiceType);
            this.logger.LogInformation("Mapping workforce metric collection '{WorkforceMetricCollectionName}' to '{ServiceType}'", workforceMetricCollectionName, serviceType);
            this.services[workforceMetricCollectionName] = serviceType;
        }

        /// <summary>
        /// Indicates whether the file type is supported.
        /// </summary>
        /// <param name="workforceMetricCollectionName">
        /// The workforce metric collection name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSupported(string workforceMetricCollectionName)
        {
            return this.services.ContainsKey(workforceMetricCollectionName);
        }
    }
}