// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInvocationContext.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Invokables.Interfaces
{
    using System.Globalization;

    /// <summary>
    /// The InvocationContext interface.
    /// </summary>
    public interface IInvocationContext
    {
        /// <summary>
        /// Gets the username.
        /// </summary>
        string? Username { get; }

        /// <summary>
        /// Gets the culture info.
        /// </summary>
        CultureInfo CultureInfo { get; }
    }
}