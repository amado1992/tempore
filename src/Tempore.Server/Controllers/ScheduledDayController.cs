// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledDayController.cs" company="Port Hope Investment S.A.">
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
    using Tempore.Server.Requests.ScheduledDay;
    using Tempore.Validation.Filters;

    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ScheduledDayController : ControllerBase
    {
        private readonly ILogger<ScheduledDayController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledDayController"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ScheduledDayController(ILogger<ScheduledDayController> logger)
        {
            this.logger = logger;
        }

        [HttpPost("[action]")]
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Manager)]
        [SwaggerOperation("Schedule days")]
        public async Task ScheduleDaysAsync([FromServices] ISender sender, [FromBody] ScheduleDaysRequest request)
        {
            await sender.Send(request);
        }

        [HttpGet("[action]")]
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Manager)]
        [SwaggerOperation("Is schedule days process running")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult<bool>> IsScheduleDaysProcessRunningAsync([FromServices] ISender sender)
        {
            return await sender.Send(IsScheduleDaysProcessRunningRequest.Instance);
        }

        [HttpPost("[action]")]
        [Validate]
        [SwaggerOperation("Adds workforce metric conflict resolution")]
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Manager)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult<Guid>> AddWorkforceMetricConflictResolutionAsync(
            [FromServices] ISender sender, [FromBody] AddWorkforceMetricConflictResolutionRequest request)
        {
            return await sender.Send(request);
        }

        [HttpPut("[action]")]
        [Validate]
        [SwaggerOperation("Update workforce metric conflict resolution")]
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Manager)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult<Guid>> UpdateWorkforceMetricConflictResolutionAsync(
            [FromServices] ISender sender, [FromBody] UpdateWorkforceMetricConflictResolutionRequest request)
        {
            return await sender.Send(request);
        }

        [HttpDelete("[action]")]
        [Validate]
        [SwaggerOperation("Delete workforce metric conflict resolution")]
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Manager)]
        public async Task<IActionResult> DeleteWorkforceMetricConflictResolutionAsync(
            [FromServices] ISender sender, [FromQuery] DeleteWorkforceMetricConflictResolutionRequest request)
        {
            await sender.Send(request);
            return this.Ok();
        }

        [HttpPost("[action]")]
        [Validate]
        [SwaggerOperation("Gets workforce metric conflict resolution from scheduled day id")]
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Manager)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<WorkforceMetricConflictResolutionDto>))]
        public async Task<ActionResult<PagedResponse<WorkforceMetricConflictResolutionDto>>> GetWorkforceMetricConflictResolutionAsync([FromServices] ISender sender, [FromBody] GetWorkforceMetricConflictResolutionsRequest request)
        {
            return await sender.Send(request);
        }
    }
}