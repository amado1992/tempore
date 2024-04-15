// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeycloakInitializationService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using global::Keycloak.Net.Models.Clients;
    using global::Keycloak.Net.Models.ClientScopes;
    using global::Keycloak.Net.Models.ProtocolMappers;
    using global::Keycloak.Net.Models.Roles;
    using global::Keycloak.Net.Models.Users;

    using Tempore.Authorization.Extensions;
    using Tempore.Authorization.Groups;
    using Tempore.Authorization.Roles;
    using Tempore.Common.Extensions;
    using Tempore.Infrastructure.Keycloak.Models;
    using Tempore.Infrastructure.Keycloak.Services;
    using Tempore.Infrastructure.Keycloak.Services.Interfaces;
    using Tempore.Keycloak;
    using Tempore.Keycloak.Extensions;
    using Tempore.Keycloak.Services;
    using Tempore.Server.Extensions;
    using Tempore.Server.Services.Interfaces;

    using Group = global::Keycloak.Net.Models.Groups.Group;

    /// <summary>
    /// The keycloak initialization service.
    /// </summary>
    public class KeycloakInitializationService : IKeycloakInitializationService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<KeycloakInitializationService> logger;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The environment.
        /// </summary>
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// The keycloak client factory.
        /// </summary>
        private readonly IKeycloakClientFactory keycloakClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeycloakInitializationService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="environment">
        /// The environment.
        /// </param>
        /// <param name="keycloakClientFactory">
        /// The keycloak Client Factory.
        /// </param>
        public KeycloakInitializationService(
            ILogger<KeycloakInitializationService> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IKeycloakClientFactory keycloakClientFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(environment);
            ArgumentNullException.ThrowIfNull(keycloakClientFactory);

            this.logger = logger;
            this.configuration = configuration;
            this.environment = environment;
            this.keycloakClientFactory = keycloakClientFactory;
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// The wait async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task WaitAsync()
        {
            while (!this.IsInitialized)
            {
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Initializes the keycloak.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InitializeAsync()
        {
            this.logger.LogInformation("Initializing Keycloak");

            if (this.environment.IsSwaggerGen())
            {
                return;
            }

            try
            {
                await this.InitializeTemporeClientsAsync();
                await this.EnableEventsAsync();

                this.logger.LogInformation("Keycloak Initialized");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error initializing Keycloak");
            }

            this.IsInitialized = true;
        }

        private async Task InitializeTemporeClientsAsync()
        {
            this.logger.LogInformation("Initializing Tempore Clients");

            await this.CreateTemporeApiClientAsync();
            await this.CreateClientScopeAsync();

            await this.CreateTemporeAppClientAsync();
            await this.CreateTemporeAgentClientAsync();

            await this.UpdateOptionalClientsScopeAsync();
            await this.CreateTemporeRolesAsync();

            await this.UpdateAgentServiceAccountRolesAsync();

            await this.CreateTemporeAppAdminUserAsync();

            this.logger.LogInformation("Tempore Clients Initialized");
        }

        private async Task UpdateAgentServiceAccountRolesAsync()
        {
            this.logger.LogInformation("Updating agent service account roles");

            var (keycloakClient, realmName) = await this.keycloakClientFactory.CreateAsync();

            var serviceAccountTemporeAgentUser =
                (await keycloakClient.GetUsersAsync(realmName, username: "service-account-tempore-agent")).First();

            var role = await keycloakClient.GetRoleByNameAsync(
                           realmName,
                           TemporeClients.Api.Id,
                           Roles.Agent.TemporeAgent);
            await keycloakClient.AddClientRoleMappingsToUserAsync(
                realmName,
                serviceAccountTemporeAgentUser.Id,
                TemporeClients.Api.Id,
                new[] { role });

            this.logger.LogInformation("Updated agent service account roles");
        }

        private async Task UpdateOptionalClientsScopeAsync()
        {
            this.logger.LogInformation("Updating optional clients scope");

            var (keycloakClient, realmName) = await this.keycloakClientFactory.CreateAsync();

            var temporeClients = new List<Client>
                          {
                              await keycloakClient.GetClientAsync(realmName, TemporeClients.Client.Id),
                              await keycloakClient.GetClientAsync(realmName, TemporeClients.Agent.Id),
                          };

            var temporeClientScope = await keycloakClient.GetClientScopeAsync(realmName, TemporeClients.Api.Id);

            foreach (var temporeClient in temporeClients)
            {
                this.logger.LogInformation("Updating client scopes for client {ClientName}", temporeClient.Name);

                await keycloakClient.UpdateOptionalClientScopeAsync(realmName, temporeClient.Id, temporeClientScope.Id);

                this.logger.LogInformation("Updated optional client scopes for client {ClientName}", temporeClient.Name);
            }

            this.logger.LogInformation("Updated optional clients scope");
        }

        private async Task CreateClientScopeAsync()
        {
            this.logger.LogInformation("Creating client scope");

            var (keycloakClient, realmName) = await this.keycloakClientFactory.CreateAsync();
            var temporeApiClient = await keycloakClient.GetClientAsync(realmName, TemporeClients.Api.Id);

            var clientScope = new ClientScope
            {
                Id = temporeApiClient.Id,
                Name = temporeApiClient.Id,
                Protocol = "openid-connect",
            };

            var protocolMapper = new ProtocolMapper
            {
                Name = $"Audience of {temporeApiClient.Name}",
                Protocol = "openid-connect",
                _ProtocolMapper = "oidc-audience-mapper",
            };

            var protocolMapperConfig = new KeycloakAudienceConfig();
            protocolMapper.Config = protocolMapperConfig;
            protocolMapperConfig.IdTokenClaim = "false";
            protocolMapperConfig.AddToAccessToken = "true";
            protocolMapperConfig.IncludedClientAudience = temporeApiClient.Id;
            clientScope.ProtocolMappers = new List<ProtocolMapper> { protocolMapper };

            ClientScope? storedClientScope = null;
            try
            {
                storedClientScope = await keycloakClient.GetClientScopeAsync(realmName, clientScope.Id);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "Error requesting a client scope {ClientScope}.", clientScope.Id);
            }

            if (storedClientScope is null && await keycloakClient.CreateClientScopeAsync(realmName, clientScope))
            {
                storedClientScope = await keycloakClient.GetClientScopeAsync(realmName, clientScope.Id);
            }
        }

        private async Task CreateTemporeApiClientAsync()
        {
            this.logger.LogInformation("Creating '{ClientName}' client", TemporeClients.Api.Name);

            var ingressUrl = this.configuration.GetSection("IdentityServer")["AppIngress"];
            var (keycloakClient, realmName) = await this.keycloakClientFactory.CreateAsync();

            // TODO: Review later if redirect is required.
            var temporeApiClient = new Client
            {
                Id = TemporeClients.Api.Id,
                Name = TemporeClients.Api.Name,
                BaseUrl = ingressUrl,
                RedirectUris = new List<string> { new Uri(ingressUrl).GetRootUrl() },
                WebOrigins = new List<string> { ingressUrl },
            };

            Client? storedClient = null;
            try
            {
                storedClient = await keycloakClient.GetClientAsync(realmName, temporeApiClient.Id);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(
                    ex,
                    "Error requesting client {ClientId}, this is normal if the client is not yet created",
                    temporeApiClient.Id);
            }

            if (storedClient is null)
            {
                this.logger.LogInformation("Creating client {Client}", temporeApiClient.Id);

                await keycloakClient.CreateClientAsync(realmName, temporeApiClient);

                this.logger.LogInformation("Created client {Client}", temporeApiClient.Id);
            }
            else
            {
                this.logger.LogInformation("Updating client {Client}", temporeApiClient.Id);

                storedClient.Name = temporeApiClient.Name;
                storedClient.ProtocolMappers = temporeApiClient.ProtocolMappers;
                storedClient.BaseUrl = temporeApiClient.BaseUrl;
                storedClient.WebOrigins = temporeApiClient.WebOrigins;
                storedClient.RedirectUris = temporeApiClient.RedirectUris;

                await keycloakClient.UpdateClientAsync(realmName, storedClient.Id, storedClient);

                this.logger.LogInformation("Updated client {Client}", temporeApiClient.Id);
            }

            this.logger.LogInformation("Created '{ClientName}' client", TemporeClients.Api.Name);
        }

        private async Task CreateTemporeAgentClientAsync()
        {
            try
            {
                var (keycloakClient, realmName) = await this.keycloakClientFactory.CreateAsync();

                this.logger.LogInformation("Creating '{ClientName}' client", TemporeClients.Agent.Name);

                var roleKeycloakClientConfig = new KeycloakClientRoleMappingConfig
                {
                    MultiValued = "true",
                    CustomIdTokenClaim = "true",
                    CustomAccessTokenClaim = "true",
                    CustomUserInfoTokenClaim = "true",
                    UserModelClientRoleMappingClientId = TemporeClients.Api.Id,
                    CustomClaimName = "roles",
                    JsonTypeLabel = "String",
                };

                var clientRoleMapper = new ClientProtocolMapper
                {
                    Protocol = "openid-connect",
                    ProtocolMapper = "oidc-usermodel-client-role-mapper",
                    Name = "Client role mapping",
                    Config = roleKeycloakClientConfig,
                };

                var groupKeycloakClientConfig = new KeycloakGroupMappingConfig
                {
                    FullPath = "true",
                    CustomIdTokenClaim = "true",
                    CustomAccessTokenClaim = "true",
                    CustomUserInfoTokenClaim = "true",
                    CustomClaimName = "groups",
                };

                var groupMapper = new ClientProtocolMapper
                {
                    Protocol = "openid-connect",
                    ProtocolMapper = "oidc-group-membership-mapper",
                    Name = "Group mapping",
                    Config = groupKeycloakClientConfig,
                };

                // TODO: Review later if redirect is required.
                var temporeAgentClient = new Client
                {
                    Id = TemporeClients.Agent.Id,
                    Name = TemporeClients.Agent.Name,
                    ImplicitFlowEnabled = false,
                    StandardFlowEnabled = true,
                    ServiceAccountsEnabled = true,
                    ClientAuthenticatorType = "client-secret",
                    PublicClient = false,
                    RedirectUris = Array.Empty<string>(),
                    ProtocolMappers = new List<ClientProtocolMapper>
                                                               {
                                                                   clientRoleMapper, groupMapper,
                                                               },
                    Attributes = new Dictionary<string, object>
                                                          {
                                                              { KeycloakClientAttributes.PkceCodeChallengeMethod, "S256" },
                                                          },
                };

                if (this.environment.IsIntegrationTest() || this.environment.IsDebug())
                {
                    temporeAgentClient.DirectAccessGrantsEnabled = true;
                }

                Client? storedClient = null;
                try
                {
                    storedClient = await keycloakClient.GetClientAsync(realmName, temporeAgentClient.Id);
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(
                        ex,
                        "Error requesting client {ClientId}, this is normal if the client is not yet created",
                        temporeAgentClient.Id);
                }

                if (storedClient is null)
                {
                    this.logger.LogInformation("Creating client {Client}", temporeAgentClient.Id);

                    await keycloakClient.CreateClientAsync(realmName, temporeAgentClient);

                    this.logger.LogInformation("Created client {Client}", temporeAgentClient.Id);
                }
                else
                {
                    this.logger.LogInformation("Updating client {Client}", temporeAgentClient.Id);

                    storedClient.Name = temporeAgentClient.Name;
                    storedClient.ProtocolMappers = temporeAgentClient.ProtocolMappers;
                    storedClient.BaseUrl = temporeAgentClient.BaseUrl;
                    storedClient.WebOrigins = temporeAgentClient.WebOrigins;
                    storedClient.RedirectUris = temporeAgentClient.RedirectUris;
                    storedClient.ServiceAccountsEnabled = temporeAgentClient.ServiceAccountsEnabled;
                    storedClient.ClientAuthenticatorType = temporeAgentClient.ClientAuthenticatorType;
                    if (this.environment.IsIntegrationTest() || this.environment.IsDebug())
                    {
                        storedClient.DirectAccessGrantsEnabled = temporeAgentClient.DirectAccessGrantsEnabled;
                    }

                    storedClient.ImplicitFlowEnabled = temporeAgentClient.ImplicitFlowEnabled;
                    storedClient.StandardFlowEnabled = temporeAgentClient.StandardFlowEnabled;
                    storedClient.PublicClient = temporeAgentClient.PublicClient;
                    if (temporeAgentClient.Attributes.ContainsKey(KeycloakClientAttributes.PkceCodeChallengeMethod))
                    {
                        storedClient.Attributes[KeycloakClientAttributes.PkceCodeChallengeMethod] =
                            temporeAgentClient.Attributes[KeycloakClientAttributes.PkceCodeChallengeMethod];
                    }

                    await keycloakClient.UpdateClientAsync(realmName, storedClient.Id, storedClient);

                    this.logger.LogInformation("Updated client {Client}", temporeAgentClient.Id);
                }

                this.logger.LogInformation("Created '{ClientName}' client", TemporeClients.Agent.Name);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error creating '{ClientName}' client", TemporeClients.Agent.Name);
            }
        }

        private async Task<Client> CreateTemporeAppClientAsync()
        {
            var ingressUrl = this.configuration.GetSection("IdentityServer")["AppIngress"];
            var (keycloakClient, realmName) = await this.keycloakClientFactory.CreateAsync();

            this.logger.LogInformation("Creating '{ClientName}' client", TemporeClients.Client.Name);

            var roleKeycloakClientConfig = new KeycloakClientRoleMappingConfig
            {
                MultiValued = "true",
                CustomIdTokenClaim = "true",
                CustomAccessTokenClaim = "true",
                CustomUserInfoTokenClaim = "true",
                UserModelClientRoleMappingClientId = TemporeClients.Api.Id,
                CustomClaimName = "roles",
                JsonTypeLabel = "String",
            };

            var clientRoleMapper = new ClientProtocolMapper
            {
                Protocol = "openid-connect",
                ProtocolMapper = "oidc-usermodel-client-role-mapper",
                Name = "Client role mapping",
                Config = roleKeycloakClientConfig,
            };

            var groupKeycloakClientConfig = new KeycloakGroupMappingConfig
            {
                FullPath = "true",
                CustomIdTokenClaim = "true",
                CustomAccessTokenClaim = "true",
                CustomUserInfoTokenClaim = "true",
                CustomClaimName = "groups",
            };

            var groupMapper = new ClientProtocolMapper
            {
                Protocol = "openid-connect",
                ProtocolMapper = "oidc-group-membership-mapper",
                Name = "Group mapping",
                Config = groupKeycloakClientConfig,
            };

            var redirectUri = new Uri(ingressUrl).GetRootUrl();
            var temporeAppClient = new Client
            {
                Id = TemporeClients.Client.Id,
                Name = TemporeClients.Client.Name,
                ImplicitFlowEnabled = false,
                StandardFlowEnabled = true,
                PublicClient = true,
                BaseUrl = ingressUrl,
                RedirectUris = new List<string> { redirectUri },
                WebOrigins = new List<string> { ingressUrl },
                ProtocolMappers = new List<ClientProtocolMapper> { clientRoleMapper, groupMapper },
                Attributes = new Dictionary<string, object>
                                                    {
                                                        { KeycloakClientAttributes.PkceCodeChallengeMethod, "S256" },
                                                        { KeycloakClientAttributes.PostLogoutRedirectUris, redirectUri },
                                                    },
            };

            if (this.environment.IsIntegrationTest() || this.environment.IsDebug())
            {
                // TODO: This could be required also for production?
                temporeAppClient.DirectAccessGrantsEnabled = true;
            }

            Client? storedClient = null;
            try
            {
                storedClient = await keycloakClient.GetClientAsync(realmName, temporeAppClient.Id);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(
                    ex,
                    "Error requesting client {Client}, this is normal if the client is not yet created",
                    temporeAppClient.Id);
            }

            if (storedClient is null)
            {
                this.logger.LogInformation("Creating client {Client}", temporeAppClient.Id);

                await keycloakClient.CreateClientAsync(realmName, temporeAppClient);

                this.logger.LogInformation("Created client {Client}", temporeAppClient.Id);
            }
            else
            {
                this.logger.LogInformation("Updating client {Client}", temporeAppClient.Id);

                storedClient.Name = temporeAppClient.Name;
                storedClient.ProtocolMappers = temporeAppClient.ProtocolMappers;
                storedClient.BaseUrl = temporeAppClient.BaseUrl;
                storedClient.WebOrigins = temporeAppClient.WebOrigins;
                storedClient.RedirectUris = temporeAppClient.RedirectUris;
                storedClient.ImplicitFlowEnabled = temporeAppClient.ImplicitFlowEnabled;

                if (this.environment.IsIntegrationTest() || this.environment.IsDebug())
                {
                    storedClient.DirectAccessGrantsEnabled = temporeAppClient.DirectAccessGrantsEnabled;
                }

                storedClient.PublicClient = temporeAppClient.PublicClient;
                storedClient.DirectAccessGrantsEnabled = temporeAppClient.DirectAccessGrantsEnabled;
                if (temporeAppClient.Attributes.ContainsKey(KeycloakClientAttributes.PkceCodeChallengeMethod))
                {
                    storedClient.Attributes[KeycloakClientAttributes.PkceCodeChallengeMethod] =
                        temporeAppClient.Attributes[KeycloakClientAttributes.PkceCodeChallengeMethod];
                }

                if (temporeAppClient.Attributes.ContainsKey(KeycloakClientAttributes.PostLogoutRedirectUris))
                {
                    storedClient.Attributes[KeycloakClientAttributes.PostLogoutRedirectUris] =
                        temporeAppClient.Attributes[KeycloakClientAttributes.PostLogoutRedirectUris];
                }

                await keycloakClient.UpdateClientAsync(realmName, storedClient.Id, storedClient);

                this.logger.LogInformation("Updated client {Client}", temporeAppClient.Id);
            }

            this.logger.LogInformation("Created '{ClientName}' client", TemporeClients.Client.Name);

            return temporeAppClient;
        }

        private async Task EnableEventsAsync()
        {
            this.logger.LogInformation("Enabling events");
            var (keycloakClient, realmName) = await this.keycloakClientFactory.CreateAsync();

            var eventsConfig = await keycloakClient.GetRealmEventsProviderConfigurationAsync(realmName);

            if (!(eventsConfig.EventsEnabled ?? false))
            {
                this.logger.LogInformation("Enabling events");

                eventsConfig.EventsEnabled = true;
                eventsConfig.EventsExpiration = 2592000L; // == 30 days
            }

            if (!(eventsConfig.AdminEventsEnabled ?? false))
            {
                this.logger.LogInformation("Enabling admin events");

                eventsConfig.AdminEventsEnabled = true;
            }

            await keycloakClient.UpdateRealmEventsProviderConfigurationAsync(realmName, eventsConfig);
        }

        private async Task CreateTemporeRolesAsync()
        {
            this.logger.LogInformation("Creating required roles");

            var (keycloakClient, realmName) = await this.keycloakClientFactory.CreateAsync();
            var temporeApiClient = await keycloakClient.GetClientAsync(realmName, TemporeClients.Api.Id);

            foreach (var policyInfo in typeof(Authorization.Policies.Policies).SortedRoleBasedPolicies())
            {
                Role? role = null;

                try
                {
                    role = await keycloakClient.GetRoleByNameAsync(realmName, temporeApiClient.Id, policyInfo.Name);
                }
                catch (Exception)
                {
                    // ignored
                }

                var currentPolicyCompositeRoles = new List<Role>();
                if (policyInfo.Roles.Any())
                {
                    var storedRoles = (await keycloakClient.GetRolesAsync(realmName, temporeApiClient.Id)).ToList();
                    foreach (var currentPolicyRoleName in policyInfo.Roles)
                    {
                        var currentPolicyRole = storedRoles.FirstOrDefault(r => r.Name == currentPolicyRoleName);
                        if (currentPolicyRole is not null)
                        {
                            currentPolicyCompositeRoles.Add(currentPolicyRole);
                        }
                    }
                }

                if (role is null)
                {
                    this.logger.LogInformation("Creating role '{RoleName}'", policyInfo.Name);

                    if (await keycloakClient.CreateRoleAsync(
                            realmName,
                            temporeApiClient.Id,
                            new Role
                            {
                                Name = policyInfo.Name,
                                Description = policyInfo.Description,
                                Composite = currentPolicyCompositeRoles.Any(),
                            }))
                    {
                        role = await keycloakClient.GetRoleByNameAsync(realmName, temporeApiClient.Id, policyInfo.Name);
                    }
                }
                else
                {
                    this.logger.LogInformation("Updating role '{RoleName}'", policyInfo.Name);

                    role.Description = policyInfo.Description;

                    await keycloakClient.UpdateRoleByNameAsync(realmName, temporeApiClient.Id, role.Name, role);
                }

                if (currentPolicyCompositeRoles.Any())
                {
                    await keycloakClient.RemoveCompositesFromRoleAsync(realmName, temporeApiClient.Id, policyInfo.Name, currentPolicyCompositeRoles);
                    await keycloakClient.AddCompositesToRoleAsync(realmName, temporeApiClient.Id, policyInfo.Name, currentPolicyCompositeRoles);
                }

                if (role is not null && !string.IsNullOrWhiteSpace(policyInfo.GroupName))
                {
                    this.logger.LogInformation("Creating group '{GroupName}'", policyInfo.GroupName);

                    var policyGroup = (await keycloakClient.GetGroupHierarchyAsync(realmName)).FirstOrDefault(g => g.Name == policyInfo.GroupName);
                    if (policyGroup is null && await keycloakClient.CreateGroupAsync(realmName, new Group { Name = policyInfo.GroupName }))
                    {
                        policyGroup =
                            (await keycloakClient.GetGroupHierarchyAsync(realmName)).FirstOrDefault(
                                g => g.Name == policyInfo.GroupName);
                    }

                    if (policyGroup is not null)
                    {
                        this.logger.LogInformation(
                            "Adding role '{RoleName}' to  group '{GroupName}'",
                            role.Name,
                            policyInfo.GroupName);

                        await keycloakClient.AddClientRoleMappingsToGroupAsync(
                            realmName,
                            policyGroup.Id,
                            temporeApiClient.Id,
                            new List<Role> { role });
                    }
                }
            }

            this.logger.LogInformation("Created required roles");
        }

        private async Task CreateTemporeAppAdminUserAsync()
        {
            this.logger.LogInformation("Creating Tempore app admin user");

            var appUsername = this.configuration.GetSection("IdentityServer")["AppUsername"];
            var appPassword = this.configuration.GetSection("IdentityServer")["AppPassword"];

            if (string.IsNullOrEmpty(appUsername))
            {
                this.logger.LogWarning("The Tempore username is not specified. Skipping Tempore app user admin configuration.");

                return;
            }

            var (keycloakClient, realmName) = await this.keycloakClientFactory.CreateAsync();
            var temporeApiClient = await keycloakClient.GetClientAsync(realmName, TemporeClients.Api.Id);

            this.logger.LogInformation("Searching for existing {UserName} user", appUsername);

            var temporeAppUser = (await keycloakClient.GetUsersAsync(realmName, username: appUsername)).FirstOrDefault(user => user.UserName == appUsername);
            if (temporeAppUser is null)
            {
                this.logger.LogInformation("Existing user not found, creating {UserName} user", appUsername);

                var requiredActions = new List<string>();

                if (!this.environment.IsIntegrationTest() && !this.environment.IsDebug())
                {
                    requiredActions.Add(KeycloakUserRequiredActions.UpdatePassword);
                }

                var user = new User
                {
                    UserName = appUsername,
                    Enabled = true,
                    RequiredActions = requiredActions.AsReadOnly(),
                };

                if (await keycloakClient.CreateUserAsync(realmName, user))
                {
                    temporeAppUser = (await keycloakClient.GetUsersAsync(realmName, username: appUsername)).First(u => u.UserName == appUsername);
                    if (string.IsNullOrEmpty(appPassword))
                    {
                        appPassword = Guid.NewGuid().ToString();
                        requiredActions.Add(KeycloakUserRequiredActions.UpdatePassword);
                    }

                    var temporary = requiredActions.Contains(KeycloakUserRequiredActions.UpdatePassword);
                    var credentials = new Credentials
                    {
                        Value = appPassword,
                        Temporary = temporary,
                    };

                    await keycloakClient.ResetUserPasswordAsync(realmName, temporeAppUser.Id, credentials);
                }
            }

            if (temporeAppUser is null)
            {
                this.logger.LogInformation("User '{UserName} not found'. Skipped role configuration. ", appUsername);
                return;
            }

            this.logger.LogInformation("Updating roles for '{Role}' user", appUsername);

            var temporeAdministratorRole =
                (await keycloakClient.GetRolesAsync(realmName, temporeApiClient.Id, search: Roles.System.Administrator))
                .FirstOrDefault(role => role.Name == Roles.System.Administrator);

            if (temporeAdministratorRole is not null)
            {
                var storedGroups = (await keycloakClient.GetGroupHierarchyAsync(realmName)).ToList();
                var temporeAdministratorsGroup = storedGroups.FirstOrDefault(g => g.Name == Groups.TemporeAdministrators);
                if (temporeAdministratorsGroup is not null)
                {
                    await keycloakClient.UpdateUserGroupAsync(
                        realmName,
                        temporeAppUser.Id,
                        temporeAdministratorsGroup.Id,
                        temporeAdministratorsGroup);
                }
            }

            this.logger.LogInformation("Created administrator Tempore user '{UserName}'", appUsername);
        }
    }
}