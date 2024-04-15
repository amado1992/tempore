// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInvocationContext.generic.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Invokables.Interfaces
{
    /// <summary>
    /// The InvocationContext interface.
    /// </summary>
    public interface IInvocationContext<TRequest> : IInvocationContext
    {
        /// <summary>
        /// Gets the request.
        /// </summary>
        TRequest Request { get; }
    }
}