// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAgentsRequest.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Requests.Agents
{
    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;

    /// <summary>
    /// The agents request.
    /// </summary>
    public class GetAgentsRequest : PagedRequest<AgentDto>
    {
    }
}