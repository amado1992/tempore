// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemController.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Swashbuckle.AspNetCore.Annotations;

    /// <summary>
    /// The system controller.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SystemController : ControllerBase
    {
        /// <summary>
        /// Gets the version async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet("[action]")]
        [SwaggerOperation("Gets the version.")]
        public Task<string> GetVersionAsync()
        {
            return Task.FromResult("1.0");
        }
    }
}