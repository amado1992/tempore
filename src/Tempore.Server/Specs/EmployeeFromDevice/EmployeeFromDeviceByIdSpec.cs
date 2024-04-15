// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeFromDeviceByIdSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.EmployeeFromDevice
{
    using System;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The employee from device by id spec.
    /// </summary>
    public class EmployeeFromDeviceByIdSpec : ISpecification<EmployeeFromDevice>
    {
        /// <summary>
        /// The employee from device id.
        /// </summary>
        private readonly Guid id;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeFromDeviceByIdSpec"/> class.
        /// </summary>
        /// <param name="id">
        /// The employee from device id.
        /// </param>
        public EmployeeFromDeviceByIdSpec(Guid id)
        {
            this.id = id;
        }

        /// <inheritdoc />
        public Func<IQueryable<EmployeeFromDevice>, IQueryable<EmployeeFromDevice>> Build()
        {
            return employees => employees.Where(employee => employee.Id == this.id);
        }
    }
}