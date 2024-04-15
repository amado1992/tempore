// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationCenter.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.App.Services
{
    using System.Collections.Concurrent;
    using System.Collections.Immutable;

    using Tempore.App.Services.EventArgs;
    using Tempore.App.Services.Interfaces;
    using Tempore.Client.Services.Interfaces;

    using Severity = Tempore.Client.Services.Interfaces.Severity;

    /// <summary>
    /// The notification center.
    /// </summary>
    public sealed class NotificationCenter : INotificationReceiver, INotificationCenter
    {
        /// <summary>
        /// The subscriptions.
        /// </summary>
        private readonly ConcurrentDictionary<string, List<Action<NotificationReceivedEventArgs>>> subscriptions = new ConcurrentDictionary<string, List<Action<NotificationReceivedEventArgs>>>();

        /// <summary>
        /// The notification received event.
        /// </summary>
        public event EventHandler<NotificationReceivedEventArgs>? NotificationReceived;

        /// <inheritdoc />
        public void Subscribe(string notificationType, Action<NotificationReceivedEventArgs> action)
        {
            var actions = this.subscriptions.GetOrAdd(notificationType, s => new List<Action<NotificationReceivedEventArgs>>());
            lock (actions)
            {
                actions.Add(action);
            }
        }

        /// <inheritdoc />
        public void Unsubscribe(string notificationType, Action<NotificationReceivedEventArgs> action)
        {
            var actions = this.subscriptions.GetOrAdd(notificationType, s => new List<Action<NotificationReceivedEventArgs>>());
            lock (actions)
            {
                actions.Remove(action);
            }
        }

        /// <inheritdoc/>
        public Task SendAsync(string notificationType, Severity severity, string message)
        {
            return severity switch
            {
                Severity.Normal => this.NormalAsync(notificationType, message),
                Severity.Information => this.InformationAsync(notificationType, message),
                Severity.Success => this.SuccessAsync(notificationType, message),
                Severity.Warning => this.WarningAsync(notificationType, message),
                Severity.Error => this.ErrorAsync(notificationType, message),
                _ => Task.CompletedTask,
            };
        }

        /// <inheritdoc/>
        public Task NormalAsync(string notificationType, string message)
        {
            this.OnNotificationReceived(new NotificationReceivedEventArgs(Severity.Normal, notificationType, message));
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SuccessAsync(string notificationType, string message)
        {
            this.OnNotificationReceived(new NotificationReceivedEventArgs(Severity.Success, notificationType, message));
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task ErrorAsync(string notificationType, string message)
        {
            this.OnNotificationReceived(new NotificationReceivedEventArgs(Severity.Error, notificationType, message));
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task WarningAsync(string notificationType, string message)
        {
            this.OnNotificationReceived(new NotificationReceivedEventArgs(Severity.Warning, notificationType, message));
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task InformationAsync(string notificationType, string message)
        {
            this.OnNotificationReceived(new NotificationReceivedEventArgs(Severity.Information, notificationType, message));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Used to call the notification received event.
        /// </summary>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void OnNotificationReceived(NotificationReceivedEventArgs e)
        {
            this.NotificationReceived?.Invoke(this, e);
            if (this.subscriptions.TryGetValue(e.NotificationType, out var actions))
            {
                foreach (var action in actions.ToImmutableList())
                {
                    action.Invoke(e);
                }
            }
        }
    }
}