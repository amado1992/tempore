// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetFileSchemaRequestHandler.cs" company="Port Hope Investment S.A.">
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
    using Tempore.Server.Requests.FileProcessing;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The get file schema request handler.
    /// </summary>
    public class GetFileSchemaRequestHandler : IRequestHandler<GetFileSchemaRequest, List<string>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetFileSchemaRequestHandler> logger;

        /// <summary>
        /// The data file repository.
        /// </summary>
        private readonly IRepository<DataFile, ApplicationDbContext> dataFileRepository;

        /// <summary>
        /// The file processing service factory.
        /// </summary>
        private readonly IFileProcessingServiceFactory fileProcessingServiceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFileSchemaRequestHandler"/> class.
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
        public GetFileSchemaRequestHandler(
            ILogger<GetFileSchemaRequestHandler> logger,
            IRepository<DataFile, ApplicationDbContext> dataFileRepository,
            IFileProcessingServiceFactory fileProcessingServiceFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(dataFileRepository);
            ArgumentNullException.ThrowIfNull(fileProcessingServiceFactory);

            this.logger = logger;
            this.dataFileRepository = dataFileRepository;
            this.fileProcessingServiceFactory = fileProcessingServiceFactory;
        }

        /// <inheritdoc/>
        public async Task<List<string>> Handle(GetFileSchemaRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            this.logger.LogInformation("Processing file {ID}", request.FileId);
            var file = await this.dataFileRepository.SingleOrDefaultAsync(dataFile => dataFile.Id == request.FileId);
            if (file is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Data file with id {request.FileId} not found");
            }

            var fileProcessingService = this.fileProcessingServiceFactory.Create(file.FileType);
            return await fileProcessingService.GetFileSchemeAsync(file);
        }
    }
}