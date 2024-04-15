// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileProcessingServiceFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.Services
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Tempore.Processing.Services.Interfaces;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The file processing service factory.
    /// </summary>
    public class FileProcessingServiceFactory : IFileProcessingServiceFactory
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FileProcessingServiceFactory> logger;

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// The services.
        /// </summary>
        private readonly Dictionary<FileType, Type> services = new Dictionary<FileType, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProcessingServiceFactory"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        public FileProcessingServiceFactory(
            ILogger<FileProcessingServiceFactory> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="fileType">
        /// The file type.
        /// </param>
        /// <returns>
        /// The <see cref="IFileProcessingService"/>.
        /// </returns>
        public IFileProcessingService Create(FileType fileType)
        {
            if (!this.services.TryGetValue(fileType, out var processingServiceType))
            {
                throw new NotSupportedException($"The file type {fileType} is not supported. Register it first.");
            }

            return (IFileProcessingService)ActivatorUtilities.CreateInstance(this.serviceProvider, processingServiceType);
        }

        /// <summary>
        /// Register a service type.
        /// </summary>
        /// <param name="fileType">
        /// The file type.
        /// </param>
        /// <typeparam name="TServiceType">
        /// The service type.
        /// </typeparam>
        public void Register<TServiceType>(FileType fileType)
            where TServiceType : IFileProcessingService
        {
            var serviceType = typeof(TServiceType);
            this.logger.LogInformation("Mapping file type {FileType} to {ServiceType}", fileType, serviceType);
            this.services[fileType] = serviceType;
        }

        /// <summary>
        /// Indicates whether the file type is supported.
        /// </summary>
        /// <param name="fileType">
        /// The file type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSupported(FileType fileType)
        {
            return this.services.ContainsKey(fileType);
        }
    }
}