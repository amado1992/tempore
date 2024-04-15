namespace Tempore.Tests.Tempore.Server.Extensions
{
    extern alias TemporeServer;

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Tests.Infraestructure;

    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json.Linq;

    using TemporeServer::Tempore.Server.Configuration;
    using TemporeServer::Tempore.Server.Extensions;

    using Xunit;

    public class JObjectExtensionsFacts
    {
        public class The_SyncBlazorHostedConfigurationAsync_Method
        {
            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Throws_ArgumentExcetion_When_IdentityServer_Authority_Is_Empty_Or_Wrong_Formmated()
            {
                var appSettings = new JObject();
                var configurationBuilder = new ConfigurationBuilder();
                var configuration = configurationBuilder.AddInMemoryCollection().Build();
                Assert.Throws<ArgumentException>(() => appSettings.SynchronizeIdentityServerSection(configuration));
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Throws_ArgumentExcetion_When_IdentityServer_AppIngress_Is_Empty_Or_Wrong_Formmated()
            {
                var appSettings = new JObject();
                var configurationBuilder = new ConfigurationBuilder();
                var keyValuePairs = new Dictionary<string, string>
                {
                    ["IdentityServer:Authority"] = "http://localhost:5003/auth/realms/master",
                };

                var configuration = configurationBuilder.AddInMemoryCollection(keyValuePairs).Build();
                Assert.Throws<ArgumentException>(() => appSettings.SynchronizeIdentityServerSection(configuration));
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Succeeds_When_IdentityServer_Authority_And_AppIngress_Are_Specified()
            {
                var appSettings = new JObject();
                var configurationBuilder = new ConfigurationBuilder();
                var data = new Dictionary<string, string>
                {
                    ["IdentityServer:Authority"] = "http://localhost:5003/auth/realms/master",
                    ["IdentityServer:AppIngress"] = "http://localhost:5003/",
                };

                var configuration = configurationBuilder.AddInMemoryCollection(data).Build();
                appSettings.SynchronizeIdentityServerSection(configuration);
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Succeeds_When_IdentityServer_Authority_And_AppIngress_Using_Default_Values()
            {
                var appSettings = new JObject();
                var configurationBuilder = new ConfigurationBuilder();
                var data = new Dictionary<string, string>
                {
                    ["IdentityServer:Authority"] = Fixtures.TestEnvironment.Keycloak.Autority,
                    ["IdentityServer:AppIngress"] = Fixtures.TestEnvironment.Tempore.Ingress,
                };

                var configuration = configurationBuilder.AddInMemoryCollection(data).Build();
                appSettings.SynchronizeIdentityServerSection(configuration);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer
                    .ResponseType]?.Value<string>()?.Should().Be(DefaultConfigurationValues.IdentityServer.ResponseType);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer.Scope]
                    ?.Value<string>()?.Should().Be(DefaultConfigurationValues.IdentityServer.Scope);
                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer.ClientId]
                    ?.Value<string>()?.Should().Be(DefaultConfigurationValues.IdentityServer.ClientId);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer
                        .AutomaticSilentRenew]?.Value<string>()?.Should()
                    .Be(DefaultConfigurationValues.IdentityServer.AutomaticSilentRenew);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer
                        .FilterProtocolClaims]?.Value<string>()?.Should()
                    .Be(DefaultConfigurationValues.IdentityServer.FilterProtocolClaims);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer
                    .LoadUserInfo]?.Value<string>()?.Should().Be(DefaultConfigurationValues.IdentityServer.LoadUserInfo);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer.Authority]
                    ?.Value<string>()?.Should().Be(Fixtures.TestEnvironment.Keycloak.Autority);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer.AppIngress]
                    ?.Value<string>()?.Should().Be(Fixtures.TestEnvironment.Tempore.Ingress);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer.RedirectUri]?.Value<string>()?.Should()
                    .Be(Fixtures.TestEnvironment.Tempore.Ingress);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer
                    .PostLogoutRedirectUri]?.Value<string>()?.Should().Be(Fixtures.TestEnvironment.Tempore.Ingress);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer
                    .AccessTokenExpiringNotificationTimeInSeconds]?.Value<string>()?.Should().Be(
                    DefaultConfigurationValues.IdentityServer.AccessTokenExpiringNotificationTimeInSeconds);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer
                    .TimeForUserInactivityAutomaticSignOut]?.Value<string>()?.Should().Be(
                    DefaultConfigurationValues.IdentityServer.TimeForUserInactivityAutomaticSignOut);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer
                    .TimeForUserInactivityNotification]?.Value<string>()?.Should().Be(
                    DefaultConfigurationValues.IdentityServer.TimeForUserInactivityNotification);
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Sets_RedirectUri_If_Specified()
            {
                var appSettings = new JObject();
                var configurationBuilder = new ConfigurationBuilder();
                var redirectUriExpectedValue = "http://localhost-redirect";

                var data = new Dictionary<string, string>
                {
                    ["IdentityServer:Authority"] = Fixtures.TestEnvironment.Keycloak.Autority,
                    ["IdentityServer:AppIngress"] = Fixtures.TestEnvironment.Tempore.Ingress,
                    ["IdentityServer:RedirectUri"] = redirectUriExpectedValue,
                };

                var configuration = configurationBuilder.AddInMemoryCollection(data).Build();
                appSettings.SynchronizeIdentityServerSection(configuration);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer.RedirectUri]?.Value<string>().Should().Be(redirectUriExpectedValue);
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Sets_PostLogoutRedirectUri_If_Specified()
            {
                var appSettings = new JObject();
                var configurationBuilder = new ConfigurationBuilder();
                var postLogoutUriExpectedValue = "http://localhost-postlogout";

                var data = new Dictionary<string, string>
                {
                    ["IdentityServer:Authority"] = Fixtures.TestEnvironment.Keycloak.Autority,
                    ["IdentityServer:AppIngress"] = Fixtures.TestEnvironment.Tempore.Ingress,
                    ["IdentityServer:PostLogoutRedirectUri"] = postLogoutUriExpectedValue,
                };

                var configuration = configurationBuilder.AddInMemoryCollection(data).Build();
                appSettings.SynchronizeIdentityServerSection(configuration);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer.PostLogoutRedirectUri]?.Value<string>().Should().Be(postLogoutUriExpectedValue);
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Sets_AccessTokenExpiringNotificationTimeInSeconds_If_Specified()
            {
                var appSettings = new JObject();
                var configurationBuilder = new ConfigurationBuilder();

                var accessTokenExpiringNotificationTimeInSecondsExpectedValue = $"{new Random().Next(100)}";

                var data = new Dictionary<string, string>
                {
                    ["IdentityServer:Authority"] = Fixtures.TestEnvironment.Keycloak.Autority,
                    ["IdentityServer:AppIngress"] = Fixtures.TestEnvironment.Tempore.Ingress,
                    ["IdentityServer:AccessTokenExpiringNotificationTimeInSeconds"] = accessTokenExpiringNotificationTimeInSecondsExpectedValue,
                };

                var configuration = configurationBuilder.AddInMemoryCollection(data).Build();
                appSettings.SynchronizeIdentityServerSection(configuration);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer.AccessTokenExpiringNotificationTimeInSeconds]?.Value<string>().Should().Be(accessTokenExpiringNotificationTimeInSecondsExpectedValue);
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Sets_TimeForUserInactivityAutomaticSignOut_If_Specified()
            {
                var appSettings = new JObject();
                var configurationBuilder = new ConfigurationBuilder();

                var timeForUserInactivityAutomaticSignOutExpectedValue = $"{new Random().Next(100)}";

                var data = new Dictionary<string, string>
                {
                    ["IdentityServer:Authority"] = Fixtures.TestEnvironment.Keycloak.Autority,
                    ["IdentityServer:AppIngress"] = Fixtures.TestEnvironment.Tempore.Ingress,
                    ["IdentityServer:TimeForUserInactivityAutomaticSignOut"] = timeForUserInactivityAutomaticSignOutExpectedValue,
                };

                var configuration = configurationBuilder.AddInMemoryCollection(data).Build();
                appSettings.SynchronizeIdentityServerSection(configuration);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer.TimeForUserInactivityAutomaticSignOut]?.Value<string>().Should().Be(timeForUserInactivityAutomaticSignOutExpectedValue);
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Sets_TimeForUserInactivityNotification_If_Specified()
            {
                var appSettings = new JObject();
                var configurationBuilder = new ConfigurationBuilder();

                var timeForUserInactivityNotificationExpectedValue = $"{new Random().Next(100)}";

                var data = new Dictionary<string, string>
                {
                    ["IdentityServer:Authority"] = Fixtures.TestEnvironment.Keycloak.Autority,
                    ["IdentityServer:AppIngress"] = Fixtures.TestEnvironment.Tempore.Ingress,
                    ["IdentityServer:TimeForUserInactivityNotification"] = timeForUserInactivityNotificationExpectedValue,
                };

                var configuration = configurationBuilder.AddInMemoryCollection(data).Build();
                appSettings.SynchronizeIdentityServerSection(configuration);

                appSettings[ConfigurationSections.IdentityServer.Name]?[ConfigurationSections.IdentityServer.TimeForUserInactivityNotification]?.Value<string>().Should().Be(timeForUserInactivityNotificationExpectedValue);
            }
        }
    }
}