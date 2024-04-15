// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubLifetimeManager.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using System.Reflection;

    using Microsoft.AspNetCore.SignalR;

    using Tempore.Server.Extensions;
    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The HubLifetimeManager class.
    /// </summary>
    /// <typeparam name="THub">
    /// The hub type.
    /// </typeparam>
    public class HubLifetimeManager<THub> : IHubLifetimeManager<THub>
        where THub : Hub
    {
        /// <summary>
        /// The hub lifetime manager.
        /// </summary>
        private readonly Microsoft.AspNetCore.SignalR.HubLifetimeManager<THub> hubLifetimeManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="HubLifetimeManager{THub}"/> class.
        /// </summary>
        /// <param name="hubLifetimeManager">
        /// The hub lifetime manager.
        /// </param>
        public HubLifetimeManager(Microsoft.AspNetCore.SignalR.HubLifetimeManager<THub> hubLifetimeManager)
        {
            ArgumentNullException.ThrowIfNull(hubLifetimeManager);

            this.hubLifetimeManager = hubLifetimeManager;
        }

        /// <summary>
        /// Determines whether the connection is alive.
        /// </summary>
        /// <param name="connectionId">
        /// The connection id.
        /// </param>
        /// <returns>
        /// <c>true</c> if the connection is alive, otherwise <c>false</c>.
        /// </returns>
        public bool IsAlive(string? connectionId)
        {
            return this.hubLifetimeManager.IsAlive(connectionId);
        }
    }
}