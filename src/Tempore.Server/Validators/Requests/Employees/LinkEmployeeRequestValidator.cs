// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkEmployeeRequestValidator.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Validators.Requests.Employees
{
    using FluentValidation;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Server.Requests.Employees;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// Link employee request validator.
    /// </summary>
    public class LinkEmployeeRequestValidator : AbstractValidator<EmployeeLinkRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkEmployeeRequestValidator"/> class.
        /// constructor.
        /// </summary>
        /// <param name="employeeFromDeviceRepository">
        /// The employee from device repository.
        /// </param>
        public LinkEmployeeRequestValidator(IRepository<EmployeeFromDevice, ApplicationDbContext> employeeFromDeviceRepository)
        {
            this.RuleFor(request => request.EmployeeFromDeviceIds)
                .NotEmpty();

            this.RuleForEach(request => request.EmployeeFromDeviceIds)
                .NotEmpty()
                .MustAsync(async (id, token) =>
                {
                    var employee = await employeeFromDeviceRepository.SingleOrDefaultAsync(e => e.Id == id);
                    return !employee?.IsLinked ?? true;
                });
        }
    }
}