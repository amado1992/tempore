// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentVariablesConfigurationProvider.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Configuration
{
    using System;

    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The environment variables configuration provider.
    /// </summary>
    public class EnvironmentVariablesConfigurationProvider : ConfigurationProvider
    {
        /// <summary>
        /// The prefix.
        /// </summary>
        private readonly string prefix;

        /// <summary>
        /// The section separator.
        /// </summary>
        private readonly string sectionSeparator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVariablesConfigurationProvider"/> class.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="sectionSeparator">
        /// The separator.
        /// </param>
        public EnvironmentVariablesConfigurationProvider(string prefix, string sectionSeparator)
        {
            this.prefix = prefix;
            this.sectionSeparator = sectionSeparator;
        }

        /// <inheritdoc/>
        public override void Load()
        {
            var environmentVariables = Environment.GetEnvironmentVariables();

            foreach (string key in environmentVariables.Keys)
            {
                var trimmedKey = key.Replace($"{this.prefix}{this.sectionSeparator}", string.Empty);

                this.Data[trimmedKey.Replace(this.sectionSeparator, ":")] = (string?)environmentVariables[key];
            }
        }
    }
}