// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileProcessingServiceFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.Services.Interfaces
{
    using Tempore.Storage.Entities;

    /// <summary>
    /// The FileProcessingServiceFactory interface.
    /// </summary>
    public interface IFileProcessingServiceFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="fileType">
        /// The file type.
        /// </param>
        /// <returns>
        /// The <see cref="IFileProcessingService"/>.
        /// </returns>
        IFileProcessingService Create(FileType fileType);

        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="fileType">
        /// The file type.
        /// </param>
        /// <typeparam name="TServiceType">
        /// The service type.
        /// </typeparam>
        void Register<TServiceType>(FileType fileType)
            where TServiceType : IFileProcessingService;

        /// <summary>
        /// Indicates whether the file type is supported.
        /// </summary>
        /// <param name="fileType">
        /// The file type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsSupported(FileType fileType);
    }
}