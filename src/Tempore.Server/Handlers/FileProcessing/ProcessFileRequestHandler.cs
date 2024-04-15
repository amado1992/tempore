// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessFileRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.FileProcessing
{
    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Invokables.FileProcessing;
    using Tempore.Server.Invokables.Interfaces;
    using Tempore.Server.Requests.FileProcessing;
    using Tempore.Server.Services.Interfaces;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The process file handler.
    /// </summary>
    public class ProcessFileRequestHandler : IRequestHandler<ProcessFileRequest>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ProcessFileRequestHandler> logger;

        /// <summary>
        /// The queue service.
        /// </summary>
        private readonly IQueueService queueService;

        /// <summary>
        /// The data file repository.
        /// </summary>
        private readonly IRepository<DataFile, ApplicationDbContext> dataFileRepository;

        /// <summary>
        /// The invocation context accessor.
        /// </summary>
        private readonly IInvocationContextAccessor invocationContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessFileRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="queueService">
        /// The queue service.
        /// </param>
        /// <param name="dataFileRepository">
        /// The data file repository.
        /// </param>
        /// <param name="invocationContextAccessor">
        /// The invocation context.
        /// </param>
        public ProcessFileRequestHandler(
            ILogger<ProcessFileRequestHandler> logger,
            IQueueService queueService,
            IRepository<DataFile, ApplicationDbContext> dataFileRepository,
            IInvocationContextAccessor invocationContextAccessor)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(queueService);
            ArgumentNullException.ThrowIfNull(dataFileRepository);
            ArgumentNullException.ThrowIfNull(invocationContextAccessor);

            this.logger = logger;
            this.queueService = queueService;
            this.dataFileRepository = dataFileRepository;
            this.invocationContextAccessor = invocationContextAccessor;
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
        public async Task Handle(ProcessFileRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (this.queueService.IsScheduled<ProcessFileInvokable>())
            {
                throw this.logger.LogErrorAndCreateException<ConflictException>("Process file job is already scheduled");
            }

            var file = await this.dataFileRepository.SingleOrDefaultAsync(dataFile => dataFile.Id == request.FileId);
            if (file is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Data file with id {request.FileId} not found");
            }

            this.queueService.QueueInvocableWithPayload<ProcessFileInvokable, IInvocationContext<ProcessFileRequest>>(this.invocationContextAccessor.Create(request));
        }
    }
}