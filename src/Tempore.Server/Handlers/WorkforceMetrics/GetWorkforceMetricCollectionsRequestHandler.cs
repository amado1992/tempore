// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWorkforceMetricCollectionsRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.WorkforceMetrics
{
    using Mapster;

    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.WorkforceMetrics;
    using Tempore.Server.Specs;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The compute workforce metrics request handler.
    /// </summary>
    public class GetWorkforceMetricCollectionsRequestHandler : IRequestHandler<GetWorkforceMetricCollectionsRequest, PagedResponse<WorkforceMetricCollectionDto>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetWorkforceMetricCollectionsRequestHandler> logger;

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository<WorkforceMetricCollection> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkforceMetricCollectionsRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="repository">
        /// The workforce metric collection repository.
        /// </param>
        public GetWorkforceMetricCollectionsRequestHandler(ILogger<GetWorkforceMetricCollectionsRequestHandler> logger, IRepository<WorkforceMetricCollection, ApplicationDbContext> repository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(repository);

            this.logger = logger;
            this.repository = repository;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<WorkforceMetricCollectionDto>> Handle(GetWorkforceMetricCollectionsRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var paginationOptions = new PaginationOptions(request.Skip, request.Take, false);
            var specification = new Specification<WorkforceMetricCollection>(paginationOptions);
            var count = await this.repository.CountAsync(specification);
            if (count == 0)
            {
                return new PagedResponse<WorkforceMetricCollectionDto>(0, Array.Empty<WorkforceMetricCollectionDto>());
            }

            specification.PaginationOptions.IsEnable = true;
            var workforceMetricCollections = await this.repository.FindAsync(specification);

            var items = workforceMetricCollections.Adapt<List<WorkforceMetricCollectionDto>>();
            return new PagedResponse<WorkforceMetricCollectionDto>(count, items);
        }
    }
}