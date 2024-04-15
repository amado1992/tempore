// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkEmployeesRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Employees
{
    using MediatR;

    using Tempore.Logging.Extensions;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Invokables.Employees;
    using Tempore.Server.Invokables.Interfaces;
    using Tempore.Server.Requests.Employees;
    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The link employees notification handler.
    /// </summary>
    public class LinkEmployeesRequestHandler : IRequestHandler<LinkEmployeesRequest, Guid>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<LinkEmployeesRequestHandler> logger;

        /// <summary>
        /// The queue task registry.
        /// </summary>
        private readonly IQueueService queueService;

        /// <summary>
        /// The invocation context accessor.
        /// </summary>
        private readonly IInvocationContextAccessor invocationContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkEmployeesRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="queueService">
        /// The queue service.
        /// </param>
        /// <param name="invocationContextAccessor">
        /// The invocation context accessor.
        /// </param>
        public LinkEmployeesRequestHandler(ILogger<LinkEmployeesRequestHandler> logger, IQueueService queueService, IInvocationContextAccessor invocationContextAccessor)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(queueService);
            ArgumentNullException.ThrowIfNull(invocationContextAccessor);

            this.logger = logger;
            this.queueService = queueService;
            this.invocationContextAccessor = invocationContextAccessor;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="request">
        /// The notification.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<Guid> Handle(LinkEmployeesRequest request, CancellationToken cancellationToken)
        {
            if (this.queueService.IsScheduled<LinkEmployeesInvokable>())
            {
                throw this.logger.LogErrorAndCreateException<ConflictException>("Link employees job is already scheduled");
            }

            var taskId = this.queueService.QueueInvocableWithPayload<LinkEmployeesInvokable, IInvocationContext>(this.invocationContextAccessor.Create());
            return Task.FromResult(taskId);
        }
    }
}