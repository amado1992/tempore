// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeByIdSpec.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Specs.Employees
{
    using Microsoft.EntityFrameworkCore;

    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Storage.Entities;

    /// <summary>
    /// The employee by id spec.
    /// </summary>
    public class EmployeeByIdSpec : ISpecification<Employee>
    {
        /// <summary>
        /// The employee id.
        /// </summary>
        private readonly Guid id;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeByIdSpec"/> class.
        /// </summary>
        /// <param name="id">
        /// The employee id.
        /// </param>
        public EmployeeByIdSpec(Guid id)
        {
            this.id = id;
        }

        /// <inheritdoc />
        public Func<IQueryable<Employee>, IQueryable<Employee>> Build()
        {
            return employees => employees.Where(employee => employee.Id == this.id);
        }
    }
}