// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationSections.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Configuration
{
    /// <summary>
    /// The configuration sections.
    /// </summary>
    public static class ConfigurationSections
    {
        /// <summary>
        /// The identity server.
        /// </summary>
        public static class IdentityServer
        {
            /// <summary>
            /// The name.
            /// </summary>
            public const string Name = nameof(IdentityServer);

            /// <summary>
            /// The response type.
            /// </summary>
            public const string ResponseType = nameof(ResponseType);

            /// <summary>
            /// The scope.
            /// </summary>
            public const string Scope = nameof(Scope);

            /// <summary>
            /// The client id.
            /// </summary>
            public const string ClientId = nameof(ClientId);

            /// <summary>
            /// The automatic silent renew.
            /// </summary>
            public const string AutomaticSilentRenew = nameof(AutomaticSilentRenew);

            /// <summary>
            /// The filter protocol claims.
            /// </summary>
            public const string FilterProtocolClaims = nameof(FilterProtocolClaims);

            /// <summary>
            /// The load user info.
            /// </summary>
            public const string LoadUserInfo = nameof(LoadUserInfo);

            /// <summary>
            /// The authority.
            /// </summary>
            public const string Authority = nameof(Authority);

            /// <summary>
            /// The app ingress.
            /// </summary>
            public const string AppIngress = nameof(AppIngress);

            /// <summary>
            /// The redirect uri.
            /// </summary>
            public const string RedirectUri = nameof(RedirectUri);

            /// <summary>
            /// The post logout redirect uri.
            /// </summary>
            public const string PostLogoutRedirectUri = nameof(PostLogoutRedirectUri);

            /// <summary>
            /// The access token expiring notification time in seconds.
            /// </summary>
            public const string AccessTokenExpiringNotificationTimeInSeconds = nameof(AccessTokenExpiringNotificationTimeInSeconds);

            /// <summary>
            /// The time for user inactivity automatic sign out.
            /// </summary>
            public const string TimeForUserInactivityAutomaticSignOut = nameof(TimeForUserInactivityAutomaticSignOut);

            /// <summary>
            /// The time for user inactivity notification.
            /// </summary>
            public const string TimeForUserInactivityNotification = nameof(TimeForUserInactivityNotification);
        }
    }
}