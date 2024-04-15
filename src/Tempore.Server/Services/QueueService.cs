// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueService.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Services
{
    using Coravel.Invocable;
    using Coravel.Queuing.Interfaces;

    using Tempore.Server.Services.Interfaces;

    /// <summary>
    /// The queue service tracker.
    /// </summary>
    public class QueueService : IQueueService
    {
        /// <summary>
        /// The queue.
        /// </summary>
        private readonly IQueue queue;

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// The locker.
        /// </summary>
        private readonly object locker = new();

        /// <summary>
        /// The task type index.
        /// </summary>
        private readonly Dictionary<Guid, Type> taskTypeIndex = new();

        /// <summary>
        /// The task per type index.
        /// </summary>
        private readonly Dictionary<Type, HashSet<Guid>> taskPerTypeIndex = new();

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<QueueService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="queue">
        /// The queue.
        /// </param>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        public QueueService(ILogger<QueueService> logger, IQueue queue, IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(queue);
            ArgumentNullException.ThrowIfNull(serviceProvider);

            this.logger = logger;
            this.queue = queue;
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public Type? GetTaskType(Guid id)
        {
            lock (this.locker)
            {
                if (this.taskTypeIndex.TryGetValue(id, out var invokableType))
                {
                    return invokableType;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public Guid QueueInvocable<TInvocable>()
            where TInvocable : IInvocable
        {
            return this.Add<TInvocable>(() => this.queue.QueueInvocable<TInvocable>());
        }

        /// <inheritdoc />
        public Guid QueueInvocableWithPayload<TInvocable, TParams>(TParams payload)
            where TInvocable : IInvocable, IInvocableWithPayload<TParams>
        {
            return this.Add<TInvocable>(() => this.queue.QueueInvocableWithPayload<TInvocable, TParams>(payload));
        }

        /// <inheritdoc />
        public void Remove(Guid id)
        {
            lock (this.locker)
            {
                if (this.taskTypeIndex.TryGetValue(id, out var invokableType) && this.taskPerTypeIndex.TryGetValue(invokableType, out var hashSet))
                {
                    hashSet.Remove(id);
                }

                this.taskTypeIndex.Remove(id);
            }
        }

        /// <inheritdoc />
        public bool IsScheduled<TInvokable>()
            where TInvokable : IInvocable
        {
            lock (this.locker)
            {
                return this.taskPerTypeIndex.TryGetValue(typeof(TInvokable), out var hashSet) && hashSet.Count > 0;
            }
        }

        /// <summary>
        /// Adds a task.
        /// </summary>
        /// <param name="taskIdFunc">
        /// The task id Func.
        /// </param>
        /// <typeparam name="TInvokable">
        /// The invokable type.
        /// </typeparam>
        private Guid Add<TInvokable>(Func<Guid> taskIdFunc)
            where TInvokable : IInvocable
        {
            Guid taskId;
            lock (this.locker)
            {
                var invokableType = typeof(TInvokable);

                taskId = taskIdFunc();
                this.taskTypeIndex[taskId] = invokableType;
                if (!this.taskPerTypeIndex.ContainsKey(invokableType))
                {
                    this.taskPerTypeIndex[invokableType] = new HashSet<Guid>();
                }

                this.taskPerTypeIndex[invokableType].Add(taskId);
            }

            return taskId;
        }
    }
}