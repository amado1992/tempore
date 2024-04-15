// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkforceMetricCollectionSchemaProviderFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.Services.Interfaces
{
    /// <summary>
    /// The IWorkforceMetricCollectionSchemaProviderServiceFactory interface.
    /// </summary>
    public interface IWorkforceMetricCollectionSchemaProviderFactory
    {
        /// <summary>
        /// Creates a .
        /// </summary>
        /// <param name="workforceMetricCollectionName">
        /// The workforce metric collection name.
        /// </param>
        /// <returns>
        /// The <see cref="IWorkforceMetricCollectionSchemaProvider"/>.
        /// </returns>
        IWorkforceMetricCollectionSchemaProvider Create(string workforceMetricCollectionName);

        /// <summary>
        /// Register a service type.
        /// </summary>
        /// <param name="workforceMetricCollectionName">
        /// The workforce metric collection name.
        /// </param>
        /// <typeparam name="TServiceType">
        /// The service type.
        /// </typeparam>
        void Register<TServiceType>(string workforceMetricCollectionName)
            where TServiceType : IWorkforceMetricCollectionSchemaProvider;

        /// <summary>
        /// Indicates whether the file type is supported.
        /// </summary>
        /// <param name="workforceMetricCollectionName">
        /// The workforce metric collection name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsSupported(string workforceMetricCollectionName);
    }
}