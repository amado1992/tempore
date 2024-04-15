// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileProcessingController.cs" company="Port Hope Investment S.A.">
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
    using Tempore.Server.Requests.FileProcessing;
    using Tempore.Validation.Filters;

    /// <summary>
    /// The import file controller.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FileProcessingController : ControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FileProcessingController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProcessingController"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public FileProcessingController(ILogger<FileProcessingController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// The upload file async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="request">
        /// The upload file request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost("[action]")]
        [Validate]
        [Authorize(Policy = Authorization.Roles.Roles.Files.Uploader)]
        [SwaggerOperation("Upload file.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<ActionResult<Guid>> UploadFileAsync([FromServices] ISender sender, [FromForm] UploadFileRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// The get file content by id async.
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
        [Authorize(Policy = Authorization.Roles.Roles.Files.ContentViewer)]
        [SwaggerOperation("Gets file content")]
        public async Task<ActionResult<PagedResponse<Dictionary<string, string>>>> GetFileDataByIdAsync([FromServices] ISender sender, [FromQuery] GetFileDataByIdRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("[action]")]
        [Authorize(Policy = Authorization.Roles.Roles.Files.ContentViewer)]
        [SwaggerOperation("Gets Schema file")]
        public async Task<ActionResult<List<string>>> GetFileSchemaAsync([FromServices] ISender sender, [FromQuery] GetFileSchemaRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// The process async.
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
        [Authorize(Policy = Authorization.Roles.Roles.Files.ContentApprover)]
        [SwaggerOperation("Process File.")]
        public async Task ProcessAsync([FromServices] ISender sender, [FromQuery] ProcessFileRequest request)
        {
            await sender.Send(request);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("[action]")]
        [Authorize(Policy = Authorization.Roles.Roles.Files.Editor)]
        [SwaggerOperation("Delete File Exist.")]
        public async Task DeleteFileExistAsync([FromServices] ISender sender, [FromQuery] DeleteFileRequest request)
        {
            await sender.Send(request);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("[action]")]
        [Authorize(Policy = Authorization.Roles.Roles.Files.Viewer)]
        [SwaggerOperation("Get List Files.")]
        public async Task<ActionResult<PagedResponse<DataFileDto>>> GetListFilesAsync([FromServices] ISender sender, [FromQuery] ListFileRequest request)
        {
            return await sender.Send(request);
        }

        /// <summary>
        /// Determines whether a processing file process is running.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("[action]")]
        [Authorize(Policy = Authorization.Roles.Roles.Files.ContentApprover)]
        [SwaggerOperation("Is processing file process running")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult<bool>> IsProcessingFileProcessRunningAsync([FromServices] ISender sender)
        {
            return await sender.Send(IsProcessingFileProcessRunningRequest.Instance);
        }
    }
}