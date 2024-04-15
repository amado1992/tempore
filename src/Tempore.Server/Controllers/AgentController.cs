// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentController.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Controllers
{
    using MediatR;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Swashbuckle.AspNetCore.Annotations;

    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.Agents;
    using Tempore.Validation.Filters;

    /// <summary>
    /// The agent controller.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AgentController : ControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AgentController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentController"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AgentController(ILogger<AgentController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Register an agent.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="request">
        /// The agent registration request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost("[action]")]
        [Validate]
        [Authorize(Policy = Authorization.Roles.Roles.Agents.Editor)]
        [SwaggerOperation("Register an agent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult<Guid>> RegisterAgentAsync([FromServices] ISender sender, [FromBody] AgentRegistrationRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Gets registered agents async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="request">
        /// The agents request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet("[action]")]
        [SwaggerOperation("Get all agents")]
        [Authorize(Policy = Authorization.Roles.Roles.Agents.Viewer)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<AgentDto>))]
        public async Task<ActionResult<PagedResponse<AgentDto>>> GetAgentsAsync([FromServices] ISender sender, [FromQuery] GetAgentsRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Gets agent by id async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="request">
        /// The devices by agent id request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet("[action]")]
        [SwaggerOperation("Get the agent by id")]
        [Authorize(Policy = Authorization.Roles.Roles.Devices.Viewer)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgentDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AgentDto>> GetAgentByIdAsync([FromServices] ISender sender, [FromQuery] GetAgentByIdRequest request)
        {
            return await sender.Send(request);
        }
    }
}