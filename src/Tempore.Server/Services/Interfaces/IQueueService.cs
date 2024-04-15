// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueueService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services.Interfaces
{
    using Coravel.Invocable;

    /// <summary>
    /// The queue service interface.
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// Determines whether a invokable type is already scheduled.
        /// </summary>
        /// <typeparam name="TInvokable">
        /// The invokable type.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the type is already scheduled otherwise <c>false</c>.
        /// </returns>
        bool IsScheduled<TInvokable>()
            where TInvokable : IInvocable;

        /// <summary>
        /// Remove a task.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        void Remove(Guid id);

        /// <summary>
        /// Gets the task type.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns></returns>
        Type? GetTaskType(Guid id);

        /// <summary>
        /// Queue an invocable that, when dequeued, will be instantiated using DI and invoked.
        /// </summary>
        /// <typeparam name="TInvocable">
        /// The invokable type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        Guid QueueInvocable<TInvocable>()
            where TInvocable : IInvocable;

        /// <summary>
        /// Queue an invocable that will be given the payload supplied to this method.
        /// </summary>
        /// <param name="payload">
        /// The payload.
        /// </param>
        /// <typeparam name="TInvocable">
        /// The invokable type.
        /// </typeparam>
        /// <typeparam name="TParams">
        /// The parameter type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        Guid QueueInvocableWithPayload<TInvocable, TParams>(TParams payload)
            where TInvocable : IInvocable, IInvocableWithPayload<TParams>;
    }
}