// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadFileRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.FileProcessing
{
    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.Requests.FileProcessing;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The upload file request handler.
    /// </summary>
    public class UploadFileRequestHandler : IRequestHandler<UploadFileRequest, Guid>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UploadFileRequestHandler> logger;

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository<DataFile, ApplicationDbContext> dataFileRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="dataFileRepository">
        /// The repository.
        /// </param>
        public UploadFileRequestHandler(ILogger<UploadFileRequestHandler> logger, IRepository<DataFile, ApplicationDbContext> dataFileRepository)
        {
            this.logger = logger;
            this.dataFileRepository = dataFileRepository;
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
        public async Task<Guid> Handle(UploadFileRequest request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Processing file {FileName}", request.File.FileName);

            using var memoryStream = new MemoryStream();
            await using var openReadStream = request.File.OpenReadStream();
            await openReadStream.CopyToAsync(memoryStream, CancellationToken.None);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var dataFile = new DataFile
            {
                FileType = request.FileType,
                FileName = request.File.FileName,
                Data = memoryStream.ToArray(),
                ProcessingState = FileProcessingState.NotProcessed,
            };

            this.dataFileRepository.Add(dataFile);
            await this.dataFileRepository.SaveChangesAsync();

            return dataFile.Id;
        }
    }
}