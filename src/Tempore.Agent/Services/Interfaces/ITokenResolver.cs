// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITokenResolver.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Services.Interfaces
{
    /// <summary>
    /// The TokenResolver interface.
    /// </summary>
    public interface ITokenResolver
    {
        /// <summary>
        /// The resolve async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<string> ResolveAsync();
    }
}