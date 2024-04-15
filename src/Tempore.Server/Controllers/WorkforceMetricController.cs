// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkforceMetricController.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Controllers
{
    using System.Net.Mime;

    using MediatR;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Swashbuckle.AspNetCore.Annotations;

    using Tempore.Processing;
    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Requests;
    using Tempore.Server.Requests.WorkforceMetrics;
    using Tempore.Validation.Filters;

    /// <summary>
    /// The agent controller.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WorkforceMetricController : ControllerBase
    {
        /// <summary>
        /// Compute workforce metrics async.
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
        [SwaggerOperation("Compute workforce metrics")]
        [Authorize(Policy = Authorization.Roles.Roles.Metrics.Calculator)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult<Guid>> ComputeWorkforceMetricsAsync([FromServices] ISender sender, [FromBody] ComputeWorkforceMetricsRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Determines whether a compute workforce metric process is running.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("[action]")]
        [Authorize(Policy = Authorization.Roles.Roles.Metrics.Viewer)]
        [SwaggerOperation("Is compute workforce metric process running")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult<bool>> IsComputeWorkforceMetricsProcessRunningAsync([FromServices] ISender sender)
        {
            return await sender.Send(IsComputeWorkforceMetricsProcessRunningRequest.Instance);
        }

        /// <summary>
        /// Gets the workforce metric collection async.
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
        [SwaggerOperation("Get workforce metric collections")]
        [Authorize(Policy = Authorization.Roles.Roles.Metrics.Viewer)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<WorkforceMetricCollectionDto>))]
        public async Task<ActionResult<PagedResponse<WorkforceMetricCollectionDto>>> GetWorkforceMetricCollectionsAsync([FromServices] ISender sender, [FromBody] GetWorkforceMetricCollectionsRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Gets the workforce metrics async.
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
        [SwaggerOperation("Get workforce metrics")]
        [Authorize(Policy = Authorization.Roles.Roles.Metrics.Viewer)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<Dictionary<string, object>>))]
        public async Task<ActionResult<PagedResponse<Dictionary<string, object>>>> GetWorkforceMetricsAsync([FromServices] ISender sender, [FromBody] GetWorkforceMetricsRequest request)
        {
            return await sender.Send(request);
        }

        [HttpPost("[action]")]
        [Validate]
        [SwaggerOperation("Export workforce metrics in file")]
        [Authorize(Policy = Authorization.Roles.Roles.Metrics.Exporter)]
        [Produces(MediaTypeNames.Application.Octet, Type = typeof(FileResult))]
        public async Task<FileResult> ExportWorkforceMetricsAsync([FromServices] ISender sender, [FromBody] ExportWorkforceMetricsRequest request)
        {
            return await sender.Send(request);
        }

        [HttpGet("[action]")]
        [Validate]
        [SwaggerOperation("Get workforce metrics schema")]
        [Authorize(Policy = Authorization.Roles.Roles.Metrics.Viewer)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ColumnInfo>))]
        public async Task<ActionResult<List<ColumnInfo>>> GetWorkforceMetricsSchemaAsync([FromServices] ISender sender, [FromQuery] GetWorkforceMetricsSchemaRequest request)
        {
            return await sender.Send(request);
        }
    }
}