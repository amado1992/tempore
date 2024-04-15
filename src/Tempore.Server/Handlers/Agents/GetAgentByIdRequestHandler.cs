// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAgentByIdRequestHandler.cs" company="Port Hope Investment S.A.">
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

    using Tempore.Logging.Extensions;
    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests.Agents;
    using Tempore.Server.Specs.Agents;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The agent by id request handler.
    /// </summary>
    public class GetAgentByIdRequestHandler : IRequestHandler<GetAgentByIdRequest, AgentDto>
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
        /// Initializes a new instance of the <see cref="GetAgentByIdRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="agentRepository">
        /// The agent repository.
        /// </param>
        public GetAgentByIdRequestHandler(ILogger<GetAgentsRequestHandler> logger, IRepository<Agent, ApplicationDbContext> agentRepository)
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
        public async Task<AgentDto> Handle(GetAgentByIdRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var specification = new AgentByIdSpec(request.AgentId);
            var agent = await this.agentRepository.SingleOrDefaultAsync(specification);
            if (agent is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Agent with Id '{0}' not found");
            }

            return agent.Adapt<AgentDto>();
        }
    }
}