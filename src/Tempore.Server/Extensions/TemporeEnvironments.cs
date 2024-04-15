// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporeEnvironments.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Extensions
{
    using Microsoft.Extensions.Hosting;

    public static class TemporeEnvironments
    {
        public const string IntegrationTest = "IntegrationTest";

        public static readonly string Development = Environments.Development;

        public static readonly string Production = Environments.Production;

        public static readonly string Staging = Environments.Staging;
    }
}