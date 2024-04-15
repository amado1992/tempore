// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddEmployeeFromDeviceTimestampRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Timestamp
{
    using EntityFramework.Exceptions.Common;

    using Flurl.Util;

    using MediatR;

    using Microsoft.AspNetCore.Mvc.TagHelpers;
    using Microsoft.EntityFrameworkCore;

    using Npgsql;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests.Employees;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The add employee from device timestamp request handler.
    /// </summary>
    public class AddEmployeeFromDeviceTimestampRequestHandler : IRequestHandler<AddEmployeeFromDeviceTimestampRequest, Guid>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AddEmployeeFromDeviceTimestampRequestHandler> logger;

        /// <summary>
        /// The device repository.
        /// </summary>
        private readonly IRepository<EmployeeFromDevice, ApplicationDbContext> employeeFromDeviceRepository;

        /// <summary>
        /// The timestamp repository.
        /// </summary>
        private readonly IRepository<Timestamp, ApplicationDbContext> timestampRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEmployeeFromDeviceTimestampRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="databaseExceptionProcessor">
        /// The database exception processor.
        /// </param>
        /// <param name="employeeFromDeviceRepository">
        /// The device repository.
        /// </param>
        /// <param name="timestampRepository">
        /// The timestamp repository.
        /// </param>
        public AddEmployeeFromDeviceTimestampRequestHandler(
            ILogger<AddEmployeeFromDeviceTimestampRequestHandler> logger,
            IRepository<EmployeeFromDevice, ApplicationDbContext> employeeFromDeviceRepository,
            IRepository<Timestamp, ApplicationDbContext> timestampRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(employeeFromDeviceRepository);
            ArgumentNullException.ThrowIfNull(timestampRepository);

            this.logger = logger;
            this.employeeFromDeviceRepository = employeeFromDeviceRepository;
            this.timestampRepository = timestampRepository;
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
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<Guid> Handle(AddEmployeeFromDeviceTimestampRequest request, CancellationToken cancellationToken)
        {
            var employeeIdOnDevice = request.Timestamp.EmployeeFromDevice.EmployeeIdOnDevice;
            var deviceId = request.Timestamp.EmployeeFromDevice.DeviceId;

            var employeeFromDevice = await this.employeeFromDeviceRepository.SingleOrDefaultAsync(e => e.EmployeeIdOnDevice == employeeIdOnDevice && e.DeviceId == deviceId);
            if (employeeFromDevice is null)
            {
                throw this.logger.LogErrorAndCreateException<NotFoundException>($"Employee '{employeeIdOnDevice}' from device '{deviceId}' not found");
            }

            try
            {
                var timestamp = new Timestamp
                {
                    EmployeeFromDeviceId = employeeFromDevice.Id,
                    DateTime = request.Timestamp.DateTime,
                };

                this.timestampRepository.Add(timestamp);
                await this.timestampRepository.SaveChangesAsync();

                return timestamp.Id;
            }
            catch (Exception ex)
            {
                if (ex is UniqueConstraintException)
                {
                    throw this.logger.LogErrorAndCreateException<ConflictException>("Error assigning employees to a shift", ex);
                }

                throw;
            }
        }
    }
}