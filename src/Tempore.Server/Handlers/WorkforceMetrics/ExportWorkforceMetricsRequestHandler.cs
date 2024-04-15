// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportWorkforceMetricsRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.WorkforceMetrics;

using System.Net.Mime;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

using Tempore.Common.Extensions;
using Tempore.Logging.Extensions;
using Tempore.Processing;
using Tempore.Processing.Services.Interfaces;
using Tempore.Server.Exceptions;
using Tempore.Server.Requests.WorkforceMetrics;
using Tempore.Server.Services.Interfaces;
using Tempore.Server.Specs;
using Tempore.Server.Specs.WorkforceMetrics;
using Tempore.Storage;
using Tempore.Storage.Entities;

/// <summary>
/// The ExportWorkforceMetricsRequestHandler.
/// </summary>
public class ExportWorkforceMetricsRequestHandler : IRequestHandler<ExportWorkforceMetricsRequest, FileResult>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<GetWorkforceMetricsRequestHandler> logger;

    /// <summary>
    /// The workforce metric collection repository.
    /// </summary>
    private readonly IRepository<WorkforceMetricCollection, ApplicationDbContext> workforceMetricCollectionRepository;

    /// <summary>
    /// The workforce metric daily snapshot repository.
    /// </summary>
    private readonly IRepository<WorkforceMetricDailySnapshot, ApplicationDbContext> workforceMetricDailySnapshotRepository;

    /// <summary>
    /// The workforce metric collection schema provider.
    /// </summary>
    private readonly IWorkforceMetricCollectionSchemaProviderFactory workforceMetricCollectionSchemaProviderFactory;

    /// <summary>
    /// The file content writer factory.
    /// </summary>
    private readonly IFileContentWriterFactory fileContentWriterFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportWorkforceMetricsRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="workforceMetricCollectionRepository">
    /// The workforce metric repository.
    /// </param>
    /// <param name="workforceMetricDailySnapshotRepository">
    /// The workforce metric daily snapshot repository.
    /// </param>
    /// <param name="workforceMetricCollectionSchemaProviderFactory">
    /// The workforce metric collection schema provider factory.
    /// </param>
    /// <param name="fileContentWriterFactory">
    /// The file content writer factory.
    /// </param>
    public ExportWorkforceMetricsRequestHandler(
        ILogger<GetWorkforceMetricsRequestHandler> logger,
        IRepository<WorkforceMetricCollection, ApplicationDbContext> workforceMetricCollectionRepository,
        IRepository<WorkforceMetricDailySnapshot, ApplicationDbContext> workforceMetricDailySnapshotRepository,
        IWorkforceMetricCollectionSchemaProviderFactory workforceMetricCollectionSchemaProviderFactory,
        IFileContentWriterFactory fileContentWriterFactory)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(workforceMetricCollectionSchemaProviderFactory);
        ArgumentNullException.ThrowIfNull(workforceMetricCollectionRepository);
        ArgumentNullException.ThrowIfNull(workforceMetricDailySnapshotRepository);
        ArgumentNullException.ThrowIfNull(fileContentWriterFactory);

        this.logger = logger;
        this.workforceMetricCollectionRepository = workforceMetricCollectionRepository;
        this.workforceMetricDailySnapshotRepository = workforceMetricDailySnapshotRepository;
        this.workforceMetricCollectionSchemaProviderFactory = workforceMetricCollectionSchemaProviderFactory;
        this.fileContentWriterFactory = fileContentWriterFactory;
    }

    /// <inheritdoc />
    public async Task<FileResult> Handle(ExportWorkforceMetricsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var workforceMetricCollection = await this.workforceMetricCollectionRepository.SingleOrDefaultAsync(workforceMetricCollection => workforceMetricCollection.Id == request.WorkforceMetricCollectionId);
        if (workforceMetricCollection is null)
        {
            throw this.logger.LogErrorAndCreateException<NotFoundException>($"Workforce metric collection '{request.WorkforceMetricCollectionId}' not found");
        }

        var workforceMetricCollectionSchemaProvider = this.workforceMetricCollectionSchemaProviderFactory.Create(workforceMetricCollection.Name);

        // TODO: Validate the schema?
        var columnInfos = request.Schema;
        if (columnInfos is null)
        {
            columnInfos = workforceMetricCollectionSchemaProvider.GetSchema(SchemaType.Export).ToList();
        }

        // ALERT: Improve this later by using pagination.
        var options = new PaginationOptions(0, int.MaxValue);
        var cumulativeWorkforceMetricDailySnapshotSpec = new CumulativeWorkforceMetricDailySnapshotSpec(
            request.WorkforceMetricCollectionId,
            request.StartDate,
            request.EndDate,
            options);

        columnInfos = columnInfos
            .Where(info => info.Include)
            .OrderBy(info => info.Index).ToList();

        await using var fileContentWriter = this.fileContentWriterFactory.Create(request.FileFormat);

        // Extract file writer.
        if (request.IncludeHeader)
        {
            await fileContentWriter.WriteLineAsync(columnInfos.Select(info => info.Name));
        }

        var workforceMetricAggregations = await this.workforceMetricDailySnapshotRepository.FindAsync(cumulativeWorkforceMetricDailySnapshotSpec).ToListAsync();
        for (var idx = 0; idx < workforceMetricAggregations.Count; idx++)
        {
            var row = new Dictionary<string, object>();
            var currentWorkforceMetricAggregation = workforceMetricAggregations[idx];

            row["Id"] = currentWorkforceMetricAggregation.ExternalId;
            row["Name"] = currentWorkforceMetricAggregation.EmployeeName;

            WorkforceMetricAggregation nextWorkforceMetricAggregation;
            do
            {
                nextWorkforceMetricAggregation = workforceMetricAggregations[idx];
                row[nextWorkforceMetricAggregation.WorkforceMetricName] = nextWorkforceMetricAggregation.Value;
                idx++;
            }
            while (idx < workforceMetricAggregations.Count
                   && currentWorkforceMetricAggregation.EmployeeId == nextWorkforceMetricAggregation.EmployeeId);

            var values = columnInfos.Select(columnInfo => row.TryGetValue(columnInfo.Name, out var value) ? value : null).ToList();

            await fileContentWriter.WriteAsync(values);

            if (idx < workforceMetricAggregations.Count)
            {
                idx -= 2;
                await fileContentWriter.WriteLineAsync();
            }
        }

        await fileContentWriter.FlushAsync();

        var fileContentResult = new FileContentResult(await fileContentWriter.GetContentAsync(), MediaTypeNames.Application.Octet)
        {
            FileDownloadName = fileContentWriter.GetFileName($"{workforceMetricCollection.Name}-{request.StartDate:O}-{request.EndDate:O}"),
        };

        return fileContentResult;
    }
}