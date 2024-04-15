// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Extensions
{
    using System.Reflection;

    public static class ConfigurationExtensions
    {
        public static bool IsIntegrationTest(this IConfiguration configuration)
        {
            return configuration[WebHostDefaults.EnvironmentKey] == TemporeEnvironments.IntegrationTest;
        }

        public static bool IsProduction(this IConfiguration configuration)
        {
            return configuration[WebHostDefaults.EnvironmentKey] == TemporeEnvironments.Production;
        }

        public static bool IsDevelopment(this IConfiguration configuration)
        {
            return configuration[WebHostDefaults.EnvironmentKey] == TemporeEnvironments.Development;
        }

        public static bool IsStaging(this IConfiguration configuration)
        {
            return configuration[WebHostDefaults.EnvironmentKey] == TemporeEnvironments.Staging;
        }

        public static bool IsSwaggerGen(this IConfiguration configuration)
        {
            if (Assembly.GetEntryAssembly()?.GetName().Name == "dotnet-swagger")
            {
                return true;
            }

            return false;
        }
    }
}