// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAgentByIdRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Agents
{
    using MediatR;

    using Tempore.Server.DataTransferObjects;

    /// <summary>
    /// The agent by id request.
    /// </summary>
    public class GetAgentByIdRequest : IRequest<AgentDto>
    {
        /// <summary>
        /// Gets or sets the agent id.
        /// </summary>
        public Guid AgentId { get; set; }
    }
}