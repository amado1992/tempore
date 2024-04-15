// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkforceMetricCollectionSchemaProvider.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Processing.Services.Interfaces
{
    using Tempore.Processing;

    /// <summary>
    /// The IWorkforceMetricCollectionSchemaProvider interface.
    /// </summary>
    public interface IWorkforceMetricCollectionSchemaProvider
    {
        /// <summary>
        /// Gets the display.
        /// </summary>
        /// <returns>
        /// The schema.
        /// </returns>
        IEnumerable<ColumnInfo> GetSchema(SchemaType schemaType);
    }
}