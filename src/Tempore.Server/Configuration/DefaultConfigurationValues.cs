// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultConfigurationValues.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Configuration
{
    /// <summary>
    /// The default configuration values.
    /// </summary>
    public static class DefaultConfigurationValues
    {
        /// <summary>
        /// The identity server.
        /// </summary>
        public static class IdentityServer
        {
            /// <summary>
            /// The scope.
            /// </summary>
            public const string Scope = "openid profile tempore-api";

            /// <summary>
            /// The response type.
            /// </summary>
            public const string ResponseType = "code";

            /// <summary>
            /// The client id.
            /// </summary>
            public const string ClientId = "tempore-client";

            /// <summary>
            /// The automatic silent renew.
            /// </summary>
            public const string AutomaticSilentRenew = "true";

            /// <summary>
            /// The filter protocol claims.
            /// </summary>
            public const string FilterProtocolClaims = "true";

            /// <summary>
            /// The load user info.
            /// </summary>
            public const string LoadUserInfo = "true";

            /// <summary>
            /// The time for user inactivity automatic sign out.
            /// </summary>
            public const string TimeForUserInactivityAutomaticSignOut = "3000000";

            /// <summary>
            /// The access token expiring notification time in seconds.
            /// </summary>
            public const string AccessTokenExpiringNotificationTimeInSeconds = "10";

            /// <summary>
            /// The time for user inactivity notification.
            /// </summary>
            public const string TimeForUserInactivityNotification = "1000000";
        }
    }
}