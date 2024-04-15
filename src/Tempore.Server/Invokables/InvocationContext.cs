// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvocationContext.cs" company="Port Hope Investment S.A.">
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
    public class InvocationContext : IInvocationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationContext"/> class.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture info.
        /// </param>
        public InvocationContext(string? username, CultureInfo cultureInfo)
        {
            this.Username = username;
            this.CultureInfo = cultureInfo;
        }

        /// <summary>
        /// Gets the username.
        /// </summary>
        public string? Username { get; }

        /// <summary>
        /// Gets the culture info.
        /// </summary>
        public CultureInfo CultureInfo { get; }
    }
}