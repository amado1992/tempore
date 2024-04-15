// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationBuilderExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Configuration.Extensions
{
    using Microsoft.Extensions.Configuration;

    using Serilog;

    using Tempore.Configuration;

    /// <summary>
    /// The configuration builder extensions.
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        private const string ConfigurationsDirectory = "Configurations";

        /// <summary>
        /// Adds environment variables with prefix and section separator.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="sectionSeparator">
        /// The section separator.
        /// </param>
        /// <returns>
        /// The <see cref="IConfigurationBuilder"/>.
        /// </returns>
        public static IConfigurationBuilder AddEnvironmentVariablesWithPrefixAndSectionSeparator(
            this IConfigurationBuilder builder, string prefix, string sectionSeparator)
        {
            return builder.Add(new EnvironmentVariablesConfigurationSource(prefix, sectionSeparator));
        }

        /// <summary>
        /// Adds environment variables with prefix and section separator.
        /// </summary>
        /// <param name="builder">
        ///     The builder.
        /// </param>
        /// <param name="prefix">
        ///     The prefix.
        /// </param>
        /// <param name="sectionSeparator">
        ///     The section separator.
        /// </param>
        /// <param name="configurationData">
        ///     The configuration data.
        /// </param>
        /// <returns>
        /// The <see cref="IConfigurationBuilder"/>.
        /// </returns>
        public static IConfigurationBuilder AddInMemoryWithPrefixSectionSeparator(
            this IConfigurationBuilder builder,
            string prefix,
            string sectionSeparator,
            IDictionary<string, string> configurationData)
        {
            return builder.Add(new InMemoryConfigurationSource(prefix, sectionSeparator, configurationData));
        }

        /// <summary>
        /// Adds json file configurations.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="environmentName">
        /// The environment name.
        /// </param>
        /// <returns>
        /// The <see cref="IConfigurationBuilder"/>.
        /// </returns>
        public static IConfigurationBuilder AddJsonFiles(this IConfigurationBuilder builder, string environmentName)
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            builder.SetBasePath(currentDirectory);
            var appSettingsFiles = new[] { "appsettings.json", $"appsettings.{environmentName}.json" };
            foreach (var appSettingsFile in appSettingsFiles)
            {
                var appSettingsFilePath = Path.Combine(currentDirectory!, appSettingsFile);

                Log.Information("Loading configuration file {FileName}", appSettingsFilePath);

                if (File.Exists(appSettingsFilePath))
                {
                    builder.AddJsonFile(appSettingsFilePath, false, true);
                }
            }

            var configurationsDirectoryPath = Path.Combine(currentDirectory, ConfigurationsDirectory);
            if (Directory.Exists(configurationsDirectoryPath))
            {
                foreach (var configurationFilePath in Directory.GetFiles(configurationsDirectoryPath, "*.json"))
                {
                    Log.Information("Loading configuration file {FileName}", configurationFilePath);
                    builder.AddJsonFile(configurationFilePath, false, true);
                }
            }

            return builder;
        }
    }
}