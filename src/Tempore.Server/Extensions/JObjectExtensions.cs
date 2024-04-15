// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JObjectExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Extensions
{
    using Newtonsoft.Json.Linq;

    using Serilog;

    using Tempore.Server.Configuration;

    /// <summary>
    /// The JObject extensions.
    /// </summary>
    public static class JObjectExtensions
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILogger Logger = Log.ForContext(typeof(JObjectExtensions));

        /// <summary>
        /// Synchronizes the identity server section.
        /// </summary>
        /// <param name="appSettings">
        ///     The app settings.
        /// </param>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <exception cref="ArgumentException">
        /// When an expected required configuration variable is missing or or wrong formatted.
        /// </exception>
        public static void SynchronizeIdentityServerSection(this JObject appSettings, IConfiguration configuration)
        {
            if (!appSettings.TryGetValue(ConfigurationSections.IdentityServer.Name, out var identityServerSection))
            {
                identityServerSection = appSettings[ConfigurationSections.IdentityServer.Name] = new JObject();
            }

            identityServerSection.SetFixedValues();
            identityServerSection.SetValuesFromConfiguration(configuration);
        }
    }
}