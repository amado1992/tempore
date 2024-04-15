// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportWorkforceMetricsRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.WorkforceMetrics
{
    using MediatR;

    using Microsoft.AspNetCore.Mvc;

    using Tempore.Processing;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The ExportWorkforceMetricsRequest.
    /// </summary>
    public class ExportWorkforceMetricsRequest : IRequest<FileResult>
    {
        /// <summary>
        /// Gets or sets the workforce metric collection id.
        /// </summary>
        public Guid WorkforceMetricCollectionId { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Gets or sets the file format.
        /// </summary>
        public FileFormat FileFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file must include a header rows.
        /// </summary>
        public bool IncludeHeader { get; set; }

        /// <summary>
        /// Gets or sets a value the schema.
        /// </summary>
        public List<ColumnInfo>? Schema { get; set; }
    }
}