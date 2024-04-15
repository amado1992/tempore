// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeeUnlinkHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Employees
{
    using System.Threading;
    using System.Threading.Tasks;

    using MediatR;

    using Microsoft.Extensions.Logging;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests.Employees;
    using Tempore.Server.Specs.EmployeeFromDevice;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The employee from device request handler.
    /// </summary>
    public class EmployeeUnlinkHandler : IRequestHandler<EmployeeUnlinkRequest>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<EmployeeUnlinkHandler> logger;

        private readonly IUnitOfWork<ApplicationDbContext> unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeUnlinkHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="unitOfWork">
        /// The unit Of Work.
        /// </param>
        public EmployeeUnlinkHandler(ILogger<EmployeeUnlinkHandler> logger, IUnitOfWork<ApplicationDbContext> unitOfWork)
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
        /// The cancellation token.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Handle(EmployeeUnlinkRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var employeeFromDeviceRepository = this.unitOfWork.GetRepository<EmployeeFromDevice>();

            using var transaction = this.unitOfWork.BeginTransaction();
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

                    employeeFromDevice.EmployeeId = null;

                    employeeFromDeviceRepository.Update(employeeFromDevice);
                    await employeeFromDeviceRepository.SaveChangesAsync();
                }

                await transaction.CommitAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error unlinking employees");

                await transaction.RollbackAsync(CancellationToken.None);

                throw;
            }
        }
    }
}