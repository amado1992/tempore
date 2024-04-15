// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvocationContext.generic.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Invokables
{
    using System.Globalization;

    using Tempore.Server.Invokables.Interfaces;

    /// <summary>
    /// The invocation context.
    /// </summary>
    /// <typeparam name="TRequest">
    /// The request type.
    /// </typeparam>
    public class InvocationContext<TRequest> : InvocationContext, IInvocationContext<TRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationContext{TRequest}"/> class.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture info.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        public InvocationContext(string? username, CultureInfo cultureInfo, TRequest request)
            : base(username, cultureInfo)
        {
            this.Request = request;
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        public TRequest Request { get; }
    }
}