// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkEmployeesInvokable.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Invokables.Employees
{
    using System.Threading;
    using System.Threading.Tasks;

    using Coravel.Invocable;

    using MediatR;

    using MethodTimer;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Client.Services.Interfaces;
    using Tempore.Server.Invokables.Interfaces;
    using Tempore.Server.Notifications.Employees;
    using Tempore.Server.Specs.EmployeeFromDevice;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The link employees invokable.
    /// </summary>
    public class LinkEmployeesInvokable : IInvocable, IInvocableWithPayload<IInvocationContext>, ICancellableInvocable
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<LinkEmployeesInvokable> logger;

        /// <summary>
        /// The unit of work.
        /// </summary>
        private readonly IUnitOfWork<ApplicationDbContext> unitOfWork;

        /// <summary>
        /// The publisher.
        /// </summary>
        private readonly IPublisher publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkEmployeesInvokable"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="unitOfWork">
        /// The unit Of work.
        /// </param>
        /// <param name="publisher">
        /// The publisher.
        /// </param>
        public LinkEmployeesInvokable(ILogger<LinkEmployeesInvokable> logger, IUnitOfWork<ApplicationDbContext> unitOfWork, IPublisher publisher)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(unitOfWork);
            ArgumentNullException.ThrowIfNull(publisher);

            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.publisher = publisher;
        }

        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public IInvocationContext Payload { get; set; } = default!;

        /// <summary>
        /// Gets or sets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Time]
        public async Task Invoke()
        {
            try
            {
                var employeeFromDeviceRepository = this.unitOfWork.GetRepository<EmployeeFromDevice>();
                var employeeRepository = this.unitOfWork.GetRepository<Employee>();

                var unlinkedEmployeesFromDevicesSpec = new UnlinkedEmployeesFromDevicesSpec();
                foreach (var employeeFromDevice in await employeeFromDeviceRepository.FindAsync(
                                                       unlinkedEmployeesFromDevicesSpec))
                {
                    this.logger.LogInformation(
                        "Linking employee '{EmployeeFromDeviceId}' from device '{DeviceId}'",
                        employeeFromDevice.EmployeeId,
                        employeeFromDevice.DeviceId);

                    var employee = await employeeRepository.SingleOrDefaultAsync(
                                       employee => employee.ExternalId == employeeFromDevice.EmployeeIdOnDevice);

                    if (employee is null)
                    {
                        this.logger.LogWarning(
                            "Can't link employee '{EmployeeFromDeviceId}' from device '{DeviceId}' with any existing employees.",
                            employeeFromDevice.EmployeeId,
                            employeeFromDevice.DeviceId);
                        continue;
                    }

                    employeeFromDevice.EmployeeId = employee.Id;

                    this.logger.LogInformation(
                        "Linked employee '{EmployeeFromDeviceId}' from device '{DeviceId}' with existing employee {EmployeeId}",
                        employeeFromDevice.EmployeeId,
                        employeeFromDevice.DeviceId,
                        employee.Id);
                }

                await this.unitOfWork.SaveChangesAsync();

                await this.publisher.Publish(new EmployeesLinkProcessCompletedNotification(this.Payload, Severity.Success));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error linking employees");

                await this.publisher.Publish(new EmployeesLinkProcessCompletedNotification(this.Payload, Severity.Error));
            }
        }
    }
}