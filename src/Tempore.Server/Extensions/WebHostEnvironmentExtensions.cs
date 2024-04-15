// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebHostEnvironmentExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Extensions
{
    using System.Reflection;

    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// The web host environment extensions.
    /// </summary>
    public static class WebHostEnvironmentExtensions
    {
        /// <summary>
        /// Determines whether the environment is for swagger generation.
        /// </summary>
        /// <param name="hostEnvironment">
        /// The host environment.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsSwaggerGen(this IWebHostEnvironment hostEnvironment)
        {
            if (Assembly.GetEntryAssembly()?.GetName().Name == "dotnet-swagger")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the environment is for integration test.
        /// </summary>
        /// <param name="hostEnvironment">
        /// The host environment.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsIntegrationTest(this IHostEnvironment hostEnvironment)
        {
            return hostEnvironment.EnvironmentName == TemporeEnvironments.IntegrationTest;
        }

        /// <summary>
        /// Determines whether the environment is for debug.
        /// </summary>
        /// <param name="hostEnvironment">
        /// The host environment.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsDebug(this IHostEnvironment hostEnvironment)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}