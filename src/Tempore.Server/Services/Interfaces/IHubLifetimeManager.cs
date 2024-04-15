// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHubLifetimeManager.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services.Interfaces
{
    using Microsoft.AspNetCore.SignalR;

    /// <summary>
    /// The HubLifetimeManager interface.
    /// </summary>
    /// <typeparam name="THub">
    /// The hub type.
    /// </typeparam>
    public interface IHubLifetimeManager<THub>
        where THub : Hub
    {
        /// <summary>
        /// Determines whether the connection is alive.
        /// </summary>
        /// <param name="connectionId">
        /// The connection id.
        /// </param>
        /// <returns>
        /// <c>true</c> if the connection is alive, otherwise <c>false</c>.
        /// </returns>
        bool IsAlive(string? connectionId);
    }
}