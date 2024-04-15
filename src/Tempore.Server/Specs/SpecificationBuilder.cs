// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecificationBuilder.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs
{
    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    /// <summary>
    /// The specification builder.
    /// </summary>
    public static class SpecificationBuilder
    {
        /// <summary>
        /// Builds an specification.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="TEntity">
        /// The entity.
        /// </typeparam>
        /// <returns>
        /// The specification.
        /// </returns>
        public static ISpecification<TEntity> Build<TEntity>(this Func<IQueryable<TEntity>, IQueryable<TEntity>> query)
        {
            return Specification<TEntity>.New(query);
        }

        /// <summary>
        /// Builds an specification.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="TEntity">
        /// The entity type.
        /// </typeparam>
        /// <typeparam name="TOutput">
        /// The output type.
        /// </typeparam>
        /// <returns>
        /// The specification.
        /// </returns>
        public static ISpecification<TEntity, TOutput> Build<TEntity, TOutput>(this Func<IQueryable<TEntity>, IQueryable<TOutput>> query)
        {
            return Specification<TEntity, TOutput>.New(query);
        }

        private class Specification<TEntity> : ISpecification<TEntity>
        {
            private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> build;

            private Specification(Func<IQueryable<TEntity>, IQueryable<TEntity>> build)
            {
                this.build = build;
            }

            public static ISpecification<TEntity> New(Func<IQueryable<TEntity>, IQueryable<TEntity>> options)
            {
                return new Specification<TEntity>(options);
            }

            public Func<IQueryable<TEntity>, IQueryable<TEntity>> Build()
            {
                return this.build;
            }
        }

        private class Specification<TEntity, TOutput> : ISpecification<TEntity, TOutput>
        {
            private readonly Func<IQueryable<TEntity>, IQueryable<TOutput>> build;

            private Specification(Func<IQueryable<TEntity>, IQueryable<TOutput>> build)
            {
                this.build = build;
            }

            public static ISpecification<TEntity, TOutput> New(Func<IQueryable<TEntity>, IQueryable<TOutput>> options)
            {
                return new Specification<TEntity, TOutput>(options);
            }

            public Func<IQueryable<TEntity>, IQueryable<TOutput>> Build()
            {
                return this.build;
            }
        }
    }
}