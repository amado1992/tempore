// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeController.cs" company="Port Hope Investment S.A.">
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
    using Tempore.Server.Requests.Employees;
    using Tempore.Validation.Filters;

    /// <summary>
    /// The employee controller.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class EmployeeController : ControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AgentController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeController"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public EmployeeController(ILogger<AgentController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// The add employee from device async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="request">
        /// The employee from device request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost("[action]")]
        [Validate]
        [SwaggerOperation("Adds employee from a device")]
        [Authorize(Policy = Authorization.Roles.Roles.Employees.Editor)]
        public async Task<ActionResult<Guid>> AddEmployeeFromDeviceAsync([FromServices] ISender sender, [FromBody] AddEmployeeFromDeviceRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Gets employees from devices async.
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
        [Authorize(Policy = Authorization.Roles.Roles.Employees.Viewer)]
        [SwaggerOperation("Get all employee from devices")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<EmployeeFromDeviceDto>))]
        public async Task<ActionResult<PagedResponse<EmployeeFromDeviceDto>>> GetEmployeesFromDevicesAsync([FromServices] ISender sender, [FromBody] GetEmployeesFromDevicesRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Gets employees async.
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
        [Authorize(Policy = Authorization.Roles.Roles.Employees.Viewer)]
        [SwaggerOperation("Get all employees")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<EmployeeDto>))]
        public async Task<ActionResult<PagedResponse<EmployeeDto>>> GetEmployeesAsync([FromServices] ISender sender, [FromBody] GetEmployeesRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Link employee async.
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
        [Authorize(Policy = Authorization.Roles.Roles.Employees.Linker)]
        [SwaggerOperation("Employee link")]
        public async Task<IActionResult> LinkEmployeeAsync([FromServices] ISender sender, [FromBody] EmployeeLinkRequest request)
        {
            await sender.Send(request);
            return this.Ok();
        }

        /// <summary>
        /// Link employee unlink async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        [HttpPost("[action]")]
        [Validate]
        [Authorize(Policy = Authorization.Roles.Roles.Employees.Linker)]
        [SwaggerOperation("Employee unlink")]
        public async Task<IActionResult> UnlinkEmployeeAsync([FromServices] ISender sender, [FromBody] EmployeeUnlinkRequest request)
        {
            await sender.Send(request);

            return this.Ok();
        }

        /// <summary>
        /// The link employees async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost("[action]")]
        [Authorize(Policy = Authorization.Roles.Roles.Employees.Linker)]
        [SwaggerOperation("Automatically link employees")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult<Guid>> LinkEmployeesAsync([FromServices] ISender sender)
        {
            return await sender.Send(LinkEmployeesRequest.Instance);
        }

        /// <summary>
        /// The is link employees process running async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet("[action]")]
        [Authorize(Policy = Authorization.Roles.Roles.Employees.Linker)]
        [SwaggerOperation("Is link employees running")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult<bool>> IsLinkEmployeesProcessRunningAsync([FromServices] ISender sender)
        {
            return await sender.Send(IsLinkEmployeesProcessRunningRequest.Instance);
        }

        /// <summary>
        /// Get employee by id async.
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
        [Authorize(Policy = Authorization.Roles.Roles.Employees.Viewer)]
        [SwaggerOperation("Get employee by id")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeByIdAsync([FromServices] ISender sender, [FromQuery] GetEmployeeByIdRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Assigns employees to scheduled shift.
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
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Manager)]
        [SwaggerOperation("Assign employees to scheduled shift")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult<Guid>> AssignEmployeesToScheduledShiftAsync([FromServices] ISender sender, AssignEmployeesToScheduledShiftRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Gets employee from device by id async.
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
        [Authorize(Policy = Authorization.Roles.Roles.Employees.Viewer)]
        [SwaggerOperation("Get employee from device by id")]
        public async Task<ActionResult<EmployeeFromDeviceDto>> GetEmployeeFromDeviceByIdAsync([FromServices] ISender sender, [FromQuery] GetEmployeeFromDeviceByIdRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// The un-assign employees from scheduled shift to async.
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
        [Authorize(Policy = Authorization.Roles.Roles.Shifts.Manager)]
        [SwaggerOperation("Un-assign employees from scheduled shift to async")]
        public async Task<IActionResult> UnAssignShiftToEmployeesAsync([FromServices] ISender sender, UnAssignShiftToEmployeesRequest request)
        {
            await sender.Send(request);
            return this.Ok();
        }
    }
}