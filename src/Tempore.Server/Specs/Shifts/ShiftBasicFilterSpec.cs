// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShiftBasicFilterSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.Shifts
{
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Extensions;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The shift by spec.
    /// </summary>
    public class ShiftBasicFilterSpec : Specification<Shift>
    {
        /// <summary>
        /// The search text filter.
        /// </summary>
        private readonly string? searchText;

        /// <summary>
        /// The start date filter.
        /// </summary>
        private readonly DateOnly? startDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftBasicFilterSpec"/> class.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        /// <param name="startDate">
        /// The is start date.
        /// </param>
        /// <param name="paginationOptions">
        /// The pagination Options.
        /// </param>
        public ShiftBasicFilterSpec(string? searchText, DateOnly? startDate, PaginationOptions paginationOptions)
            : base(paginationOptions)
        {
            this.searchText = searchText;
            this.startDate = startDate;
        }

        /// <inheritdoc />
        protected override Func<IQueryable<Shift>, IQueryable<Shift>> BuildSpec()
        {
            //// TODO: ILike is postgres specific function. Avoid use this here.
            return entities =>
            {
                return entities
                    .Where(x => EF.Functions.ILike(x.Name, $"%{this.searchText}%"), !string.IsNullOrWhiteSpace(this.searchText));
            };
        }
    }
}