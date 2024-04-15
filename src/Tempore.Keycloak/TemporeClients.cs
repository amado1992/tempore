// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporeClients.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Keycloak
{
    /// <summary>
    /// The clients.
    /// </summary>
    public static class TemporeClients
    {
        /// <summary>
        /// The api.
        /// </summary>
        public static class Api
        {
            /// <summary>
            /// The tempore api client id.
            /// </summary>
            public const string Id = "tempore-api";

            /// <summary>
            /// The tempore api client name.
            /// </summary>
            public const string Name = "Tempore API";
        }

        /// <summary>
        /// The cient.
        /// </summary>
        public static class Client
        {
            /// <summary>
            /// The tempore app client id.
            /// </summary>
            public const string Id = "tempore-client";

            /// <summary>
            /// The tempore app client name.
            /// </summary>
            public const string Name = "Tempore";
        }

        /// <summary>
        /// The agent.
        /// </summary>
        public static class Agent
        {
            /// <summary>
            /// The tempore agent id.
            /// </summary>
            public const string Id = "tempore-agent";

            /// <summary>
            /// The tempore agent name.
            /// </summary>
            public const string Name = "Tempore Agent";
        }

        /// <summary>
        /// The mobile.
        /// </summary>
        public static class Mobile
        {
            /// <summary>
            /// The tempore mobile client id.
            /// </summary>
            public const string Id = "tempore-mobile-client";

            /// <summary>
            /// The tempore mobile client name.
            /// </summary>
            public const string Name = "Tempore Mobile";

            /// <summary>
            /// The tempore mobile client redirect uri.
            /// </summary>
            public const string RedirectUri = "tempore://callback";
        }
    }
}