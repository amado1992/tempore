// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShiftController.cs" company="Port Hope Investment S.A.">
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
    using Tempore.Server.Requests.Shifts;
    using Tempore.Validation.Filters;

    /// <summary>
    /// The shift controller.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ShiftController : ControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ShiftController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftController"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ShiftController(ILogger<ShiftController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Get shifts async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPost("[action]")]
        [Validate]
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Viewer)]
        [SwaggerOperation("Get shifts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<ShiftDto>))]
        public async Task<ActionResult<PagedResponse<ShiftDto>>> GetShiftsAsync([FromServices] ISender sender, [FromBody] GetShiftRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Get shift by id async.
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
        [HttpGet("[action]")]
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Viewer)]
        [SwaggerOperation("Get shift by id")]
        public async Task<ActionResult<ShiftDto>> GetShiftByIdAsync([FromServices] ISender sender, [FromQuery] GetShiftByIdRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Gets shift assignments async.
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
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Viewer)]
        [SwaggerOperation("Get scheduled shifts overview")]
        public async Task<ActionResult<PagedResponse<ScheduledShiftOverviewDto>>> GetScheduledShiftsByShiftIdAsync([FromServices] ISender sender, [FromBody] GetScheduledShiftByShiftIdRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Gets shift assignments async.
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
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Viewer)]
        [SwaggerOperation("Get employees from shift assignment distinct")]
        public async Task<ActionResult<PagedResponse<ScheduledShiftEmployeeDto>>> GetEmployeesFromScheduledShiftAsync([FromServices] ISender sender, [FromBody] GetEmployeesFromScheduledShiftRequest request)
        {
            return await sender.Send(request);
        }
    }
}