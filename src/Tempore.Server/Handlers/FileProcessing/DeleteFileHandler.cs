// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteFileHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.FileProcessing
{
    using System.Threading;
    using System.Threading.Tasks;

    using Catel;

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

    public class DeleteFileHandler : IRequestHandler<DeleteFileRequest>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DeleteFileHandler> logger;

        /// <summary>
        /// The data file repository.
        /// </summary>
        private readonly IRepository<DataFile, ApplicationDbContext> dataFileRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteFileHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="dataFileRepository">
        /// The datafile repository.
        /// </param>
        public DeleteFileHandler(ILogger<DeleteFileHandler> logger, IRepository<DataFile, ApplicationDbContext> dataFileRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(dataFileRepository);

            this.logger = logger;
            this.dataFileRepository = dataFileRepository;
        }

        /// <inheritdoc/>
        public async Task Handle(DeleteFileRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            this.logger.LogInformation("Processing file {ID}", request.FileId);
            var file = await this.dataFileRepository.SingleOrDefaultAsync(dataFile => dataFile.Id == request.FileId);

            if (file is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Data file with id {request.FileId} not found");
            }

            this.dataFileRepository.Delete(file);
            await this.dataFileRepository.SaveChangesAsync();
        }
    }
}