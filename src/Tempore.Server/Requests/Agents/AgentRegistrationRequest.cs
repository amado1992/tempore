// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentRegistrationRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Agents
{
    using MediatR;

    using Tempore.Server.DataTransferObjects;

    /// <summary>
    /// The agent registration request.
    /// </summary>
    public class AgentRegistrationRequest : IRequest<Guid>
    {
        /// <summary>
        /// Gets or sets the agent.
        /// </summary>
        public AgentRegistrationDto? Agent { get; set; }
    }
}