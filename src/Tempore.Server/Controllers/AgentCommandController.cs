// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentCommandController.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Controllers
{
    using MediatR;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Swashbuckle.AspNetCore.Annotations;

    using Tempore.Server.Requests.DeviceCommands;
    using Tempore.Validation.Filters;

    /// <summary>
    /// The device command controller.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AgentCommandController : ControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AgentCommandController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentCommandController"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AgentCommandController(ILogger<AgentCommandController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// The send command to push employees to device async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="request">
        /// The upload employees request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost("[action]")]
        [Validate]
        [SwaggerOperation("Order agent to upload employees from devices")]
        [Authorize(Policy = Authorization.Roles.Roles.Agents.Operator)]
        public async Task<IActionResult> UploadEmployeesFromDevicesAsync([FromServices] ISender sender, [FromBody] UploadEmployeesFromDevicesRequest request)
        {
            await sender.Send(request);

            return this.Ok();
        }

        /// <summary>
        /// Execute upload employees timestamps async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost("[action]")]
        [Validate]
        [SwaggerOperation("Order agent to upload timestamps of employees from devices")]
        [Authorize(Policy = Authorization.Roles.Roles.Agents.Operator)]
        public async Task<IActionResult> UploadTimestampsFromDevicesAsync([FromServices] ISender sender, [FromBody] UploadTimestampsFromDevicesRequest request)
        {
            await sender.Send(request);

            return this.Ok();
        }
    }
}