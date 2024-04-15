// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceController.cs" company="Port Hope Investment S.A.">
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
    using Tempore.Server.Requests.Devices;

    /// <summary>
    /// The device controller.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DeviceController : ControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DeviceController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceController"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DeviceController(ILogger<DeviceController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets registered agents async.
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
        [SwaggerOperation("Get all devices from an agent")]
        [Authorize(Policy = Authorization.Roles.Roles.Devices.Viewer)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<DeviceDto>))]
        public async Task<ActionResult<PagedResponse<DeviceDto>>> GetDevicesByAgentIdAsync([FromServices] ISender sender, [FromQuery] DevicesByAgentIdRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Updates the device state async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="request">
        /// The update device state request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost("[action]")]
        [SwaggerOperation("Updates the device state")]
        [Authorize(Policy = Authorization.Roles.Roles.Devices.Editor)]
        public async Task<IActionResult> UpdateDeviceStateAsync([FromServices] ISender sender, [FromBody] UpdateDeviceStateRequest request)
        {
            await sender.Send(request);
            return this.Ok();
        }

        /// <summary>
        /// Gets all devices async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet("[action]")]
        [SwaggerOperation("Get all devices")]
        [Authorize(Policy = Authorization.Roles.Roles.Devices.Viewer)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DeviceDto>))]
        public async Task<ActionResult<List<DeviceDto>>> GetAllDevicesAsync([FromServices] ISender sender)
        {
            return await sender.Send(GetDevicesRequest.Instance);
        }
    }
}