// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnlinkedEmployeesFromDevicesSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.EmployeeFromDevice
{
    using System;
    using System.Linq;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The unlinked employees from devices spec.
    /// </summary>
    public class UnlinkedEmployeesFromDevicesSpec : ISpecification<EmployeeFromDevice>
    {
        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="Func{TResult}"/>.
        /// </returns>
        public Func<IQueryable<EmployeeFromDevice>, IQueryable<EmployeeFromDevice>> Build()
        {
            return employeeFromDevice => employeeFromDevice.Where(e => !e.IsLinked);
        }
    }
}