// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInvocationContextAccessor.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services.Interfaces
{
    using Tempore.Server.Invokables.Interfaces;

    public interface IInvocationContextAccessor
    {
        IInvocationContext Create();

        IInvocationContext<TRequest> Create<TRequest>(TRequest request);
    }
}