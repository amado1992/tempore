// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubLifetimeManagerExtensions.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Extensions
{
    using System.Reflection;

    using Microsoft.AspNetCore.SignalR;

    /// <summary>
    /// The HubLifetimeManagerExtensions class.
    /// </summary>
    public static class HubLifetimeManagerExtensions
    {
        /// <summary>
        /// Determines whether the connection is alive.
        /// </summary>
        /// <typeparam name="THub">
        /// The hub type.
        /// </typeparam>
        /// <param name="hubLifetimeManager">
        /// The hub life time manager.
        /// </param>
        /// <param name="connectionId">
        /// The connection id.
        /// </param>
        /// <returns>
        /// <c>true</c> if the connection is alive, otherwise <c>false</c>.
        /// </returns>
        public static bool IsAlive<THub>(this HubLifetimeManager<THub> hubLifetimeManager, string? connectionId)
            where THub : Hub
        {
            var field = hubLifetimeManager.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(info => info.Name == "_connections");
            if (field?.GetValue(hubLifetimeManager) is not HubConnectionStore hubConnectionStore)
            {
                throw new NotSupportedException();
            }

            if (string.IsNullOrWhiteSpace(connectionId))
            {
                return false;
            }

            return hubConnectionStore[connectionId] is not null;
        }
    }
}