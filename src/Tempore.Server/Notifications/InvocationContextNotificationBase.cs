// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvocationContextNotificationBase.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Notifications
{
    using MediatR;

    using Tempore.Client.Services.Interfaces;
    using Tempore.Server.Invokables.Interfaces;

    /// <summary>
    /// The invocation context notification base.
    /// </summary>
    public abstract class InvocationContextNotificationBase : INotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationContextNotificationBase"/> class.
        /// </summary>
        /// <param name="invocationContext">
        /// The invocation context.
        /// </param>
        /// <param name="severity">
        /// The severity.
        /// </param>
        protected InvocationContextNotificationBase(IInvocationContext invocationContext, Severity severity)
        {
            this.InvocationContext = invocationContext;
            this.Severity = severity;
        }

        /// <summary>
        /// Gets the invocation context.
        /// </summary>
        public IInvocationContext InvocationContext { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        public Severity Severity { get; }
    }
}