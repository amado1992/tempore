// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentByIdSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.Agents
{
    using System;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The agent by id spec.
    /// </summary>
    public class AgentByIdSpec : ISpecification<Agent>
    {
        /// <summary>
        /// The agent id.
        /// </summary>
        private readonly Guid id;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentByIdSpec"/> class.
        /// </summary>
        /// <param name="id">
        /// The agent id.
        /// </param>
        public AgentByIdSpec(Guid id)
        {
            this.id = id;
        }

        /// <inheritdoc />
        public Func<IQueryable<Agent>, IQueryable<Agent>> Build()
        {
            return agents => agents.Where(agent => agent.Id == this.id);
        }
    }
}