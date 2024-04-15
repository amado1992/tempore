// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentVariableService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Hosting.Services
{
    using System;

    using Microsoft.Extensions.Logging;

    using Tempore.Hosting.Services.Interfaces;

    public class EnvironmentVariableService : IEnvironmentVariableService
    {
        private readonly ILogger<EnvironmentVariableService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVariableService"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public EnvironmentVariableService(ILogger<EnvironmentVariableService> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public string? GetValue(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);

            // Note: never log the actual values for security reasons
            this.logger.LogDebug("Getting value for environment variable '{Name}', returned any value: '{HasValue}'", name, !string.IsNullOrWhiteSpace(value));

            return value;
        }

        /// <inheritdoc/>
        public string? GetValue(string name, EnvironmentVariableTarget target)
        {
            var value = Environment.GetEnvironmentVariable(name, target);

            // Note: never log the actual values for security reasons
            this.logger.LogDebug("Getting value for environment variable '{Name}' and target '{Target}', returned any value: '{HasValue}'", name, target, !string.IsNullOrWhiteSpace(value));

            return value;
        }
    }
}