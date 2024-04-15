// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeLinkHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Employees
{
    using System.Threading;
    using System.Threading.Tasks;

    using EntityFramework.Exceptions.Common;

    using MediatR;

    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests.Employees;
    using Tempore.Server.Specs.EmployeeFromDevice;
    using Tempore.Server.Specs.Employees;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The employee from device request handler.
    /// </summary>
    public class EmployeeLinkHandler : IRequestHandler<EmployeeLinkRequest>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<EmployeeLinkHandler> logger;

        /// <summary>
        /// The unit of work.
        /// </summary>
        private readonly IUnitOfWork<ApplicationDbContext> unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeLinkHandler"/> class.
        /// </summary>
        public EmployeeLinkHandler(ILogger<EmployeeLinkHandler> logger, IUnitOfWork<ApplicationDbContext> unitOfWork)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(unitOfWork);

            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellationToken.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Handle(EmployeeLinkRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var employeeFromDeviceRepository = this.unitOfWork.GetRepository<EmployeeFromDevice>();
            var employeeRepository = this.unitOfWork.GetRepository<Employee>();

            await using var transaction = this.unitOfWork.BeginTransaction();
            try
            {
                foreach (var employeeFromDeviceId in request.EmployeeFromDeviceIds)
                {
                    var specificationEmployeeFromDevice = new EmployeeFromDeviceByIdSpec(employeeFromDeviceId);
                    var employeeFromDevice = await employeeFromDeviceRepository.SingleOrDefaultAsync(specificationEmployeeFromDevice);

                    if (employeeFromDevice is null)
                    {
                        throw this.logger.LogErrorAndCreateException<NotFoundException>($"Employee from device with Id '{employeeFromDeviceId}' not found");
                    }

                    var specificationEmployee = new EmployeeByIdSpec(request.EmployeeId);
                    var employee = await employeeRepository.SingleOrDefaultAsync(specificationEmployee);

                    if (employee is null)
                    {
                        throw this.logger.LogErrorAndCreateException<NotFoundException>($"Employee with Id '{request.EmployeeId}' not found");
                    }

                    employeeFromDevice.EmployeeId = request.EmployeeId;

                    employeeFromDeviceRepository.Update(employeeFromDevice);
                    await employeeFromDeviceRepository.SaveChangesAsync();
                }

                await transaction.CommitAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error linking employees");

                await transaction.RollbackAsync(CancellationToken.None);

                throw;
            }
        }
    }
}