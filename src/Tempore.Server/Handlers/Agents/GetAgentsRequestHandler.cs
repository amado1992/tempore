// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAgentsRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Agents
{
    using System.Threading;
    using System.Threading.Tasks;

    using Mapster;

    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.Agents;
    using Tempore.Server.Specs;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The agents request handler.
    /// </summary>
    public class GetAgentsRequestHandler : IRequestHandler<GetAgentsRequest, PagedResponse<AgentDto>>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<GetAgentsRequestHandler> logger;

        /// <summary>
        /// The agent repository.
        /// </summary>
        private readonly IRepository<Agent, ApplicationDbContext> agentRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAgentsRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="agentRepository">
        /// The agent repository.
        /// </param>
        public GetAgentsRequestHandler(ILogger<GetAgentsRequestHandler> logger, IRepository<Agent, ApplicationDbContext> agentRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(agentRepository);

            this.logger = logger;
            this.agentRepository = agentRepository;
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
        public async Task<PagedResponse<AgentDto>> Handle(GetAgentsRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var specification = new Specification<Agent>(new PaginationOptions(request.Skip, request.Take, false));
            var count = await this.agentRepository.CountAsync(specification);
            if (count == 0)
            {
                return new PagedResponse<AgentDto>(0, Array.Empty<AgentDto>());
            }

            specification.PaginationOptions.IsEnable = true;
            var agents = await this.agentRepository.FindAsync(specification);
            var items = agents?.Adapt<List<AgentDto>>();
            return new PagedResponse<AgentDto>(count, items);
        }
    }
}