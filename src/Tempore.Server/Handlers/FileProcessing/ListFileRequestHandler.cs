// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListFileRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.FileProcessing
{
    using System.Threading;
    using System.Threading.Tasks;

    using Mapster;

    using MediatR;

    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.FileProcessing;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The ListFileRequestHandler class.
    /// </summary>
    public class ListFileRequestHandler : IRequestHandler<ListFileRequest, PagedResponse<DataFileDto>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ListFileRequestHandler> logger;

        /// <summary>
        /// The data file repository.
        /// </summary>
        private readonly IRepository<DataFile, ApplicationDbContext> dataFileRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListFileRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="dataFileRepository">
        /// The data file repository.
        /// </param>
        public ListFileRequestHandler(
            ILogger<ListFileRequestHandler> logger,
            IRepository<DataFile, ApplicationDbContext> dataFileRepository)
        {
            this.logger = logger;
            this.dataFileRepository = dataFileRepository;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<DataFileDto>> Handle(ListFileRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            this.logger.LogInformation("Gets all Files at system ");

            var count = await this.dataFileRepository.CountAsync(_ => true);

            if (count == 0)
            {
                return new PagedResponse<DataFileDto>(count, Array.Empty<DataFileDto>());
            }

            var listFiles = this.dataFileRepository.All();

            var items = listFiles.Skip(request.Skip).Take(request.Take).ToList();

            var config = TypeAdapterConfig<DataFile, DataFileDto>.NewConfig().Ignore(src => src.Data).Ignore(dest => dest.Data).Config;
            var dataFileDto = items.Adapt<List<DataFileDto>>(config);

            return new PagedResponse<DataFileDto>(count, dataFileDto);
        }
    }
}