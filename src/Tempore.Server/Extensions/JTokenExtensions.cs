// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JTokenExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Extensions
{
    using Newtonsoft.Json.Linq;

    using Serilog;

    using Tempore.Server.Configuration;

    /// <summary>
    /// The JToken extensions.
    /// </summary>
    public static class JTokenExtensions
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILogger Logger = Log.ForContext(typeof(JObjectExtensions));

        /// <summary>
        /// Set values from configuration.
        /// </summary>
        /// <param name="identityServerSection">
        /// The identity server section.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public static void SetValuesFromConfiguration(this JToken identityServerSection, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(identityServerSection);
            ArgumentNullException.ThrowIfNull(configuration);

            identityServerSection.SetAuthorityFromConfiguration(configuration);
            var appIngress = identityServerSection.SetAndGetAppIngressFromConfiguration(configuration);
            var redirectUri = identityServerSection.SetAndGetRedirectUriFromConfiguration(configuration, appIngress);
            identityServerSection.SetPostLogoutRedirectUri(configuration, redirectUri);
            identityServerSection.SetAccessTokenExpiringNotificationTimeInSecondsFromConfiguration(configuration);
            identityServerSection.SetTimeForUserInactivityAutomaticSignOutFromConfiguration(configuration);
            identityServerSection.SetTimeForUserInactivityNotificationFromConfiguration(configuration);
        }

        /// <summary>
        /// Sets fixed values.
        /// </summary>
        /// <param name="identityServerSection">
        /// The identity server section.
        /// </param>
        public static void SetFixedValues(this JToken identityServerSection)
        {
            ArgumentNullException.ThrowIfNull(identityServerSection);

            identityServerSection[ConfigurationSections.IdentityServer.ResponseType] = DefaultConfigurationValues.IdentityServer.ResponseType;
            identityServerSection[ConfigurationSections.IdentityServer.Scope] = DefaultConfigurationValues.IdentityServer.Scope;
            identityServerSection[ConfigurationSections.IdentityServer.ClientId] = DefaultConfigurationValues.IdentityServer.ClientId;
            identityServerSection[ConfigurationSections.IdentityServer.AutomaticSilentRenew] = DefaultConfigurationValues.IdentityServer.AutomaticSilentRenew;
            identityServerSection[ConfigurationSections.IdentityServer.FilterProtocolClaims] = DefaultConfigurationValues.IdentityServer.FilterProtocolClaims;
            identityServerSection[ConfigurationSections.IdentityServer.LoadUserInfo] = DefaultConfigurationValues.IdentityServer.LoadUserInfo;
        }

        /// <summary>
        /// Updates authority from configuration.
        /// </summary>
        /// <param name="identityServerSection">
        /// The identity server section.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The expected IdentityServer Authority configuration value is empty or wrong formatted.
        /// </exception>
        private static void SetAuthorityFromConfiguration(this JToken identityServerSection, IConfiguration configuration)
        {
            var authority = configuration.GetSection(ConfigurationSections.IdentityServer.Name)?[ConfigurationSections.IdentityServer.Authority];
            if (string.IsNullOrWhiteSpace(authority) || !Uri.TryCreate(authority, UriKind.Absolute, out _))
            {
                throw new ArgumentException(
                    "The expected IdentityServer Authority configuration value is empty or wrong formatted.");
            }

            Logger.Information("Updating identity server authority to '{Authority}'", authority);
            identityServerSection[ConfigurationSections.IdentityServer.Authority] = authority;
        }

        /// <summary>
        /// Set and get app ingress from configuration.
        /// </summary>
        /// <param name="identityServerSection">
        /// The identity server section.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// When the expected IdentityServer AppIngress configuration value is empty or wrong formatted.
        /// </exception>
        private static string SetAndGetAppIngressFromConfiguration(this JToken identityServerSection, IConfiguration configuration)
        {
            var appIngress = configuration.GetSection(ConfigurationSections.IdentityServer.Name)?[ConfigurationSections.IdentityServer.AppIngress]?.TrimEnd(' ', '/');
            if (string.IsNullOrWhiteSpace(appIngress) || !Uri.TryCreate(appIngress, UriKind.Absolute, out _))
            {
                throw new ArgumentException(
                    "The expected IdentityServer AppIngress configuration value is empty or wrong formatted.");
            }

            Logger.Information("Updating redirect url '{AppIngress}'", appIngress);
            identityServerSection[ConfigurationSections.IdentityServer.AppIngress] = appIngress;
            return appIngress;
        }

        /// <summary>
        /// Set and get redirect uri from configuration.
        /// </summary>
        /// <param name="identityServerSection">
        /// The identity server section.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The redirect uri.
        /// </returns>
        private static string SetAndGetRedirectUriFromConfiguration(this JToken identityServerSection, IConfiguration configuration, string defaultValue)
        {
            var redirectUri = configuration.GetSection(ConfigurationSections.IdentityServer.Name)?[ConfigurationSections.IdentityServer.RedirectUri]?.TrimEnd(' ', '/');
            if (string.IsNullOrWhiteSpace(redirectUri) || !Uri.TryCreate(defaultValue, UriKind.Absolute, out _))
            {
                Logger.Warning(
                    "The expected IdentityServer RedirectUri configuration value is empty or wrong formatted. IdentityServer AppIngress will be used as RedirectUri instead");
                redirectUri = defaultValue;
            }

            Logger.Information("Updating redirect url '{RedirectUrl}'", redirectUri);
            identityServerSection[ConfigurationSections.IdentityServer.RedirectUri] = redirectUri;
            return redirectUri;
        }

        /// <summary>
        /// Sets post logout redirect uri.
        /// </summary>
        /// <param name="identityServerSection">
        /// The identity server section.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        private static void SetPostLogoutRedirectUri(this JToken identityServerSection, IConfiguration configuration, string defaultValue)
        {
            var postLogoutRedirectUri =
                configuration.GetSection(ConfigurationSections.IdentityServer.Name)?[ConfigurationSections.IdentityServer.PostLogoutRedirectUri]?.TrimEnd(' ', '/');
            if (string.IsNullOrWhiteSpace(postLogoutRedirectUri) || !Uri.TryCreate(postLogoutRedirectUri, UriKind.Absolute, out _))
            {
                Logger.Warning(
                    "The expected IdentityServer PostLogoutRedirectUri configuration value is empty or wrong formatted. The value IdentityServer RedirectUri or AppIngress will be used as PostLogoutRedirectUri instead.");
                postLogoutRedirectUri = defaultValue;
            }

            Logger.Information("Updating post logout redirect '{RedirectUrl}'", postLogoutRedirectUri);
            identityServerSection[ConfigurationSections.IdentityServer.PostLogoutRedirectUri] = postLogoutRedirectUri;
        }

        /// <summary>
        /// Sets access token expiring notification time in seconds from configuration.
        /// </summary>
        /// <param name="identityServerSection">
        /// The identity server section.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        private static void SetAccessTokenExpiringNotificationTimeInSecondsFromConfiguration(
            this JToken identityServerSection, IConfiguration configuration)
        {
            var accessTokenExpiringNotificationTimeInSeconds =
                configuration.GetSection(ConfigurationSections.IdentityServer.Name)?[ConfigurationSections
                    .IdentityServer.AccessTokenExpiringNotificationTimeInSeconds];
            if (string.IsNullOrWhiteSpace(accessTokenExpiringNotificationTimeInSeconds) || !double.TryParse(
                    accessTokenExpiringNotificationTimeInSeconds,
                    out _))
            {
                accessTokenExpiringNotificationTimeInSeconds = DefaultConfigurationValues.IdentityServer
                    .AccessTokenExpiringNotificationTimeInSeconds;
                Logger.Warning(
                    "The expected IdentityServer AccessTokenExpiringNotificationTimeInSeconds configuration value is empty or wrong formatted. Will be use {AccessTokenExpiringNotificationTimeInSeconds} seconds",
                    accessTokenExpiringNotificationTimeInSeconds);
            }

            Logger.Information(
                "Updating access token expiring notification time in seconds '{AccessTokenExpiringNotificationTimeInSeconds}'",
                accessTokenExpiringNotificationTimeInSeconds);
            identityServerSection[ConfigurationSections.IdentityServer.AccessTokenExpiringNotificationTimeInSeconds] =
                accessTokenExpiringNotificationTimeInSeconds;
        }

        /// <summary>
        /// Sets time for user inactivity automatic sign out from configuration.
        /// </summary>
        /// <param name="identityServerSection">
        /// The identity server section.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        private static void SetTimeForUserInactivityAutomaticSignOutFromConfiguration(
            this JToken identityServerSection, IConfiguration configuration)
        {
            var timeForUserInactivityAutomaticSignOut =
                configuration.GetSection(ConfigurationSections.IdentityServer.Name)?[ConfigurationSections.IdentityServer.TimeForUserInactivityAutomaticSignOut];
            if (string.IsNullOrWhiteSpace(timeForUserInactivityAutomaticSignOut)
                || !double.TryParse(timeForUserInactivityAutomaticSignOut, out _))
            {
                timeForUserInactivityAutomaticSignOut = DefaultConfigurationValues.IdentityServer.TimeForUserInactivityAutomaticSignOut;
                Logger.Warning(
                    "The expected IdentityServer TimeForUserInactivityAutomaticSignOut configuration value is empty or wrong formatted. Will be use {TimeForUserInactivityAutomaticSignOut} milliseconds",
                    timeForUserInactivityAutomaticSignOut);
            }

            Logger.Information(
                "Updating TimeForUserInactivityAutomaticSignOut '{TimeForUserInactivityAutomaticSignOut}'",
                timeForUserInactivityAutomaticSignOut);
            identityServerSection[ConfigurationSections.IdentityServer.TimeForUserInactivityAutomaticSignOut] = timeForUserInactivityAutomaticSignOut;
        }

        /// <summary>
        /// Sets time for user inactivity notification from configuration.
        /// </summary>
        /// <param name="identityServerSection">
        /// The identity server section.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        private static void SetTimeForUserInactivityNotificationFromConfiguration(
            this JToken identityServerSection, IConfiguration configuration)
        {
            var timeForUserInactivityNotification =
                configuration.GetSection(ConfigurationSections.IdentityServer.Name)?[ConfigurationSections
                    .IdentityServer.TimeForUserInactivityNotification];
            if (string.IsNullOrWhiteSpace(timeForUserInactivityNotification)
                || !double.TryParse(timeForUserInactivityNotification, out _))
            {
                timeForUserInactivityNotification =
                    DefaultConfigurationValues.IdentityServer.TimeForUserInactivityNotification;
                Logger.Warning(
                    "The expected IdentityServer TimeForUserInactivityNotification configuration value is empty or wrong formatted. Will be use {TimeForUserInactivityNotification} milliseconds",
                    timeForUserInactivityNotification);
            }

            Logger.Information(
                "Updating TimeForUserInactivityNotification '{TimeForUserInactivityNotification}'",
                timeForUserInactivityNotification);
            identityServerSection[ConfigurationSections.IdentityServer.TimeForUserInactivityNotification] =
                timeForUserInactivityNotification;
        }
    }
}