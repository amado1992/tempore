// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Specification.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs
{
    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    /// <summary>
    /// The spec.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The entity type.
    /// </typeparam>
    public class Specification<TEntity> : ISpecification<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Specification{TEntity}"/> class.
        /// </summary>
        /// <param name="paginationOptions">
        /// The pagination options.
        /// </param>
        public Specification(PaginationOptions paginationOptions)
        {
            this.PaginationOptions = paginationOptions;
        }

        /// <summary>
        /// Gets the pagination options.
        /// </summary>
        public PaginationOptions PaginationOptions { get; }

        /// <inheritdoc />
        Func<IQueryable<TEntity>, IQueryable<TEntity>> ISpecification<TEntity>.Build()
        {
            return entities =>
            {
                entities = this.BuildSpec()(entities);
                if (this.PaginationOptions.IsEnable)
                {
                    entities = entities.Skip(this.PaginationOptions.Skip).Take(this.PaginationOptions.Take);
                }

                return entities;
            };
        }

        /// <summary>
        /// Called when the specification is building.
        /// </summary>
        /// <returns>
        /// The function that represent the specification.
        /// </returns>
        protected virtual Func<IQueryable<TEntity>, IQueryable<TEntity>> BuildSpec()
        {
            return entities => entities;
        }
    }

    /// <summary>
    /// The spec.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The entity type.
    /// </typeparam>
    /// <typeparam name="TOutput">
    /// The output.
    /// </typeparam>
    public abstract class Specification<TEntity, TOutput> : ISpecification<TEntity, TOutput>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Specification{TEntity, TOutput}"/> class.
        /// </summary>
        /// <param name="paginationOptions">
        /// The pagination options.
        /// </param>
        public Specification(PaginationOptions paginationOptions)
        {
            this.PaginationOptions = paginationOptions;
        }

        /// <summary>
        /// Gets the pagination options.
        /// </summary>
        public PaginationOptions PaginationOptions { get; }

        /// <inheritdoc />
        Func<IQueryable<TEntity>, IQueryable<TOutput>> ISpecification<TEntity, TOutput>.Build()
        {
            return entities =>
            {
                var outputs = this.BuildSpec()(entities);
                if (this.PaginationOptions.IsEnable)
                {
                    outputs = outputs.Skip(this.PaginationOptions.Skip).Take(this.PaginationOptions.Take);
                }

                return outputs;
            };
        }

        /// <summary>
        /// Called when the specification is building.
        /// </summary>
        /// <returns>
        /// The function that represent the specification.
        /// </returns>
        protected abstract Func<IQueryable<TEntity>, IQueryable<TOutput>> BuildSpec();
    }
}