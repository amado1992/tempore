// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsLinkEmployeesProcessRunningRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Employees
{
    using System.Threading;
    using System.Threading.Tasks;

    using MediatR;

    using Tempore.Server.Invokables.Employees;
    using Tempore.Server.Requests.Employees;
    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The is link employees process running request handler.
    /// </summary>
    public class IsLinkEmployeesProcessRunningRequestHandler : IRequestHandler<IsLinkEmployeesProcessRunningRequest, bool>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<IsLinkEmployeesProcessRunningRequestHandler> logger;

        /// <summary>
        /// The queue service.
        /// </summary>
        private readonly IQueueService queueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsLinkEmployeesProcessRunningRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="queueService">
        /// The queue service.
        /// </param>
        public IsLinkEmployeesProcessRunningRequestHandler(ILogger<IsLinkEmployeesProcessRunningRequestHandler> logger, IQueueService queueService)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(queueService);

            this.logger = logger;
            this.queueService = queueService;
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
        public Task<bool> Handle(IsLinkEmployeesProcessRunningRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return Task.FromResult(this.queueService.IsScheduled<LinkEmployeesInvokable>());
        }
    }
}