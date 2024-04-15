// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileWriterFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using Catel;

    using Tempore.Client;
    using Tempore.Processing.Services.Interfaces;
    using Tempore.Server.Services.Interfaces;
    using Tempore.Storage.Entities;

    using FileFormat = Tempore.Storage.Entities.FileFormat;

    /// <summary>
    /// The file content write factory.
    /// </summary>
    public class FileContentWriterFactory : IFileContentWriterFactory
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FileContentWriterFactory> logger;

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// The services.
        /// </summary>
        private readonly Dictionary<FileFormat, Type> services = new Dictionary<FileFormat, Type>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public FileContentWriterFactory(ILogger<FileContentWriterFactory> logger, IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(serviceProvider);

            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public IFileContentWriter Create(FileFormat fileFormat)
        {
            if (!this.services.TryGetValue(fileFormat, out var serviceType))
            {
                throw new NotSupportedException($"The file format {fileFormat} is not supported. Register it first.");
            }

            return (IFileContentWriter)ActivatorUtilities.CreateInstance(this.serviceProvider, serviceType);
        }

        public void Register<TServiceType>(FileFormat fileFormat)
            where TServiceType : IFileContentWriter
        {
            var serviceType = typeof(TServiceType);
            this.logger.LogInformation("Mapping file format {FileFormat} to {ServiceType}", fileFormat, serviceType);
            this.services[fileFormat] = serviceType;
        }
    }
}