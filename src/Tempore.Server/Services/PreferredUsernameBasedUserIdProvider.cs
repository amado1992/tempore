// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreferredUsernameBasedUserIdProvider.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using System.Collections.Concurrent;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Win32;

    using Tempore.Server.Extensions;

    /// <summary>
    /// The preferred username based user id provider.
    /// </summary>
    public class PreferredUsernameBasedUserIdProvider : IUserIdProvider
    {
        /// <summary>
        /// Gets the user id.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User.GetPreferredUsername()!;
        }
    }
}