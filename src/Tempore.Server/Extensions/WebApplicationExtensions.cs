// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApplicationExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Extensions
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Serilog;

    /// <summary>
    /// The web application extensions.
    /// </summary>
    public static class WebApplicationExtensions
    {
        /// <summary>
        /// Synchronizes blazor hosted configuration file.
        /// </summary>
        /// <param name="webApplication">
        /// The web application.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// When the configuration file is not found.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// When an expected required configuration variable is missing or or wrong formatted.
        /// </exception>
        public static async Task SyncBlazorHostedConfigurationAsync(this WebApplication webApplication)
        {
            var logger = Log.ForContext(typeof(WebApplicationExtensions));

            logger.Information("Synchronizing web configuration file with server side configuration variables");

            var webApplicationEnvironment = webApplication.Environment;
            var configuration = webApplication.Configuration;

            foreach (var fileInfo in webApplicationEnvironment.WebRootFileProvider.GetDirectoryContents("/"))
            {
                logger.Information(fileInfo.PhysicalPath);
            }

            var appSettingsFile = webApplicationEnvironment.WebRootFileProvider.GetDirectoryContents("/")
                .FirstOrDefault(info => Path.GetFileName(info.PhysicalPath).Equals("appsettings.json"));

            if (appSettingsFile is null)
            {
                throw new FileNotFoundException("Required configuration file not found appsettings.json");
            }

            var appSettingsContent = await File.ReadAllTextAsync(appSettingsFile.PhysicalPath);
            var appSettings = JObject.Parse(appSettingsContent);

            appSettings.SynchronizeIdentityServerSection(configuration);

            await File.WriteAllTextAsync(
                appSettingsFile.PhysicalPath,
                JsonConvert.SerializeObject(appSettings, Formatting.Indented));
        }
    }
}