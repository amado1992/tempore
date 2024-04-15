// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetFileDataByIdRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.FileProcessing
{
    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Processing.Services.Interfaces;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.FileProcessing;
    using Tempore.Server.Services.Interfaces;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The get file content by id request handler.
    /// </summary>
    public class GetFileDataByIdRequestHandler : IRequestHandler<GetFileDataByIdRequest, PagedResponse<Dictionary<string, string>>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetFileDataByIdRequestHandler> logger;

        /// <summary>
        /// The data file repository.
        /// </summary>
        private readonly IRepository<DataFile, ApplicationDbContext> dataFileRepository;

        /// <summary>
        /// The file processing service factory.
        /// </summary>
        private readonly IFileProcessingServiceFactory fileProcessingServiceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFileDataByIdRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="dataFileRepository">
        /// The data file repository.
        /// </param>
        /// <param name="fileProcessingServiceFactory">
        /// The file processing service factory.
        /// </param>
        public GetFileDataByIdRequestHandler(
            ILogger<GetFileDataByIdRequestHandler> logger,
            IRepository<DataFile, ApplicationDbContext> dataFileRepository,
            IFileProcessingServiceFactory fileProcessingServiceFactory)
        {
            this.logger = logger;
            this.dataFileRepository = dataFileRepository;
            this.fileProcessingServiceFactory = fileProcessingServiceFactory;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<PagedResponse<Dictionary<string, string>>> Handle(GetFileDataByIdRequest request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Processing file {ID}", request.FileId);
            var file = await this.dataFileRepository.SingleOrDefaultAsync(dataFile => dataFile.Id == request.FileId);
            if (file is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Data file with id {request.FileId} not found");
            }

            var fileProcessingService = this.fileProcessingServiceFactory.Create(file.FileType);
            var (count, items) = await fileProcessingService.GetFileDataAsync(file, request.Skip, request.Take);
            return new PagedResponse<Dictionary<string, string>>(count, items);
        }
    }
}