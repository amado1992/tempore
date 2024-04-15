// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryConfigurationSource.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Configuration
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The in memory configuration source.
    /// </summary>
    public class InMemoryConfigurationSource : IConfigurationSource
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
        /// Initializes a new instance of the <see cref="InMemoryConfigurationSource"/> class.
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
        public InMemoryConfigurationSource(
            string prefix, string sectionSeparator, IDictionary<string, string> configurationData)
        {
            this.prefix = prefix;
            this.sectionSeparator = sectionSeparator;
            this.configurationData = configurationData;
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <returns>
        /// The <see cref="IConfigurationProvider"/>.
        /// </returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new InMemoryConfigurationProvider(this.prefix, this.sectionSeparator, this.configurationData);
        }
    }
}