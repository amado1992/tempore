// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimestampController.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Controllers
{
    using MediatR;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Swashbuckle.AspNetCore.Annotations;

    using Tempore.Server.Requests.Employees;
    using Tempore.Validation.Filters;

    /// <summary>
    /// The agent controller.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TimestampController : ControllerBase
    {
        /// <summary>
        /// Add employee from device timestamp async.
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
        [SwaggerOperation("Adds employee from a device timestamp")]
        [Authorize(Policy = Authorization.Roles.Roles.Timestamps.Editor)]
        public async Task<ActionResult<Guid>> AddEmployeeFromDeviceTimestampAsync(
            [FromServices] ISender sender, [FromBody] AddEmployeeFromDeviceTimestampRequest request)
        {
            return await sender.Send(request);
        }
    }
}