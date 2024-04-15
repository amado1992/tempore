// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeFromDeviceBasicFilterSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.EmployeeFromDevice
{
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Extensions;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The employee by spec.
    /// </summary>
    public class EmployeeFromDeviceBasicFilterSpec : Specification<EmployeeFromDevice>
    {
        /// <summary>
        /// The search text filter.
        /// </summary>
        private readonly string? searchText;

        /// <summary>
        /// The is linked filter.
        /// </summary>
        private readonly bool? isLinked;

        /// <summary>
        /// The include device agent flag.
        /// </summary>
        private readonly bool includeAgent;

        /// <summary>
        /// The include device flag.
        /// </summary>
        private readonly bool includeDevice;

        /// <summary>
        /// The employee id.
        /// </summary>
        private readonly Guid? employeeId;

        /// <summary>
        /// List of DiviceIds.
        /// </summary>
        private readonly List<Guid>? deviceIds;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeFromDeviceBasicFilterSpec"/> class.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        /// <param name="isLinked">
        /// The is linked.
        /// </param>
        /// <param name="includeDevice">
        /// Indicates whether the result will include the device.
        /// </param>
        /// <param name="includeAgent">
        /// Indicates whether the result will include the agent.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="paginationOptions">
        /// The paginationOptions.
        /// </param>
        /// <param name="deviceIds">
        /// The diviceIds.
        /// </param>>
        public EmployeeFromDeviceBasicFilterSpec(string? searchText, bool? isLinked, bool? includeDevice, bool? includeAgent, Guid? employeeId, List<Guid>? deviceIds, PaginationOptions paginationOptions)
            : base(paginationOptions)
        {
            this.searchText = searchText;
            this.isLinked = isLinked;
            this.employeeId = employeeId;
            this.includeDevice = includeDevice ?? false;
            this.includeAgent = this.includeDevice && (includeAgent ?? false);
            this.deviceIds = deviceIds;
        }

        /// <inheritdoc />
        protected override Func<IQueryable<EmployeeFromDevice>, IQueryable<EmployeeFromDevice>> BuildSpec()
        {
            //// TODO: ILike is postgres specific function. Avoid use this here.
            return entities =>
            {
                return entities
                    .Where(x => EF.Functions.ILike(x.FullName, $"%{this.searchText}%") || EF.Functions.ILike(x.EmployeeIdOnDevice, $"%{this.searchText}%"), !string.IsNullOrWhiteSpace(this.searchText))
                    .Where(x => x.EmployeeId == this.employeeId, this.employeeId is not null)
                    .Where(x => x.IsLinked == this.isLinked, this.isLinked is not null)
                    .Where(x => this.deviceIds!.Contains(x.DeviceId), this.deviceIds is not null && this.deviceIds.Count > 0)
                    .Include(e => e.Device, this.includeDevice)
                    .ThenInclude((Device d) => d.Agent, this.includeAgent);
            };
        }
    }
}