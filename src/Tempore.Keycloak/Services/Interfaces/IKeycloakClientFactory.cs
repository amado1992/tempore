// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IKeycloakClientFactory.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Infrastructure.Keycloak.Services.Interfaces
{
    using global::Keycloak.Net;

    public interface IKeycloakClientFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<(KeycloakClient KeycloakClient, string RealmName)> CreateAsync();
    }
}