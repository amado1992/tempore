// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationHub.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Hubs
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    using Tempore.Client.Services.Interfaces;

    /// <summary>
    /// The notification hub.
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub<INotificationReceiver>, INotificationHub
    {
    }
}