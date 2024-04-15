// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryConfigurationProvider.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Configuration
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The in memory configuration provider.
    /// </summary>
    public class InMemoryConfigurationProvider : ConfigurationProvider
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
        /// The configuration data.
        /// </summary>
        private readonly IDictionary<string, string> configurationData;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryConfigurationProvider"/> class.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="sectionSeparator">
        /// The section separator.
        /// </param>
        /// <param name="configurationData">
        /// The configuration data.
        /// </param>
        public InMemoryConfigurationProvider(
            string prefix, string sectionSeparator, IDictionary<string, string> configurationData)
        {
            this.prefix = prefix;
            this.sectionSeparator = sectionSeparator;
            this.configurationData = configurationData;
        }

        /// <summary>
        /// The load.
        /// </summary>
        public override void Load()
        {
            foreach (string key in this.configurationData.Keys)
            {
                var trimmedKey = key.Replace($"{this.prefix}{this.sectionSeparator}", string.Empty);

                this.Data[trimmedKey.Replace(this.sectionSeparator, ":")] = (string?)this.configurationData[key];
            }
        }
    }
}