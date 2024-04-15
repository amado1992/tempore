// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShiftByIdSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.Shifts
{
    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The shift by id spec.
    /// </summary>
    public class ShiftByIdSpec : ISpecification<Shift>
    {
        /// <summary>
        /// The employee id.
        /// </summary>
        private readonly Guid id;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftByIdSpec"/> class.
        /// </summary>
        /// <param name="id">
        /// The employee id.
        /// </param>
        public ShiftByIdSpec(Guid id)
        {
            this.id = id;
        }

        /// <inheritdoc />
        public Func<IQueryable<Shift>, IQueryable<Shift>> Build()
        {
            return shifts => shifts
            .Where(shift => shift.Id == this.id)
            .Include(e => e.Days.OrderBy(element => element.Index))
            .ThenInclude(e => e.Timetable);
        }
    }
}