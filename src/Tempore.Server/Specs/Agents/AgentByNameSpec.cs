// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentByNameSpec.cs" company="Port Hope Investment S.A.">
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
    public class AgentByNameSpec : ISpecification<Agent>
    {
        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentByNameSpec"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public AgentByNameSpec(string name)
        {
            this.name = name;
        }

        /// <inheritdoc />
        Func<IQueryable<Agent>, IQueryable<Agent>> ISpecification<Agent>.Build()
        {
            return agents => agents
                .Include(agent => agent.Devices)
                .Where(agent => agent.Name == this.name);
        }
    }
}