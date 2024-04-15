// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWorkforceMetricsSchemaRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.WorkforceMetrics
{
    using MediatR;

    using Tempore.Processing;

    /// <summary>
    /// The GetWorkforceMetricsSchemaRequest.
    /// </summary>
    public class GetWorkforceMetricsSchemaRequest : IRequest<List<ColumnInfo>>
    {
        /// <summary>
        /// Gets or sets the workforce metric collection id.
        /// </summary>
        public Guid WorkforceMetricCollectionId { get; set; }

        /// <summary>
        /// Gets or sets the schema type.
        /// </summary>
        public SchemaType SchemaType { get; set; } = SchemaType.Display;
    }
}