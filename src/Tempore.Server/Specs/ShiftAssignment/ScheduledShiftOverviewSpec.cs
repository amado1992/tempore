// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledShiftOverviewSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.ShiftAssignment
{
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Extensions;

    using Tempore.Server.Requests.Shifts.SearchParams;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The scheduled shift assignment with employees count spec.
    /// </summary>
    public class ScheduledShiftOverviewSpec : Specification<ScheduledShift, ScheduledShiftOverview>
    {
        /// <summary>
        /// The shift id.
        /// </summary>
        private readonly Guid shiftId;

        /// <summary>
        /// The search params.
        /// </summary>
        private readonly ScheduledShiftSearchParams? searchParams;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledShiftOverviewSpec"/> class.
        /// </summary>
        /// <param name="shiftId">
        /// The shiftId.
        /// </param>
        /// <param name="searchParams">
        /// The search params.
        /// </param>
        /// <param name="paginationOptions">
        /// The pagination options.
        /// </param>
        public ScheduledShiftOverviewSpec(
            Guid shiftId,
            ScheduledShiftSearchParams? searchParams,
            PaginationOptions paginationOptions)
            : base(paginationOptions)
        {
            this.shiftId = shiftId;
            this.searchParams = searchParams;
        }

        /// <summary>
        /// The build spec.
        /// </summary>
        /// <returns>
        /// The <see cref="Func{IQueryable, IQueryable}"/>.
        /// </returns>
        protected override Func<IQueryable<ScheduledShift>, IQueryable<ScheduledShiftOverview>> BuildSpec()
        {
            return scheduledShifts => scheduledShifts
                .Include(scheduledShift => scheduledShift.Employees)
                .Where(scheduledShift => scheduledShift.ShiftId == this.shiftId)
                .Where(scheduledShift => scheduledShift.Id == this.searchParams!.Id, this.searchParams?.Id is not null)
                .Where(scheduledShift => scheduledShift.StartDate == this.searchParams!.StartDate, this.searchParams?.StartDate is not null)
                .Where(scheduledShift => scheduledShift.ExpireDate == this.searchParams!.ExpireDate, this.searchParams?.ExpireDate is not null)
                .Where(scheduledShift => scheduledShift.EffectiveWorkingTime == this.searchParams!.EffectiveWorkingTime, this.searchParams?.EffectiveWorkingTime is not null)
                .Select(scheduledShift => new ScheduledShiftOverview
                {
                    Id = scheduledShift.Id,
                    ShiftId = this.shiftId,
                    StartDate = scheduledShift.StartDate,
                    ExpireDate = scheduledShift.ExpireDate,
                    EffectiveWorkingTime = scheduledShift.EffectiveWorkingTime,
                    EmployeesCount = scheduledShift.Employees.Count,
                });
        }
    }
}