// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentRegistrationRequestHandler.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Handlers.Agents
{
    using System.Threading;

    using EntityFramework.Exceptions.Common;

    using Mapster;

    using MediatR;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;
    using StoneAssemblies.EntityFrameworkCore.Specifications.Interfaces;

    using Tempore.Logging.Extensions;
    using Tempore.Server.DataTransferObjects;
    using Tempore.Server.Exceptions;
    using Tempore.Server.Requests.Agents;
    using Tempore.Server.Specs.Agents;
    using Tempore.Server.Specs.Devices;
    using Tempore.Storage;
    using Tempore.Storage.Entities;

    /// <summary>
    /// The agent registration request handler.
    /// </summary>
    public class AgentRegistrationRequestHandler : IRequestHandler<AgentRegistrationRequest, Guid>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AgentRegistrationRequestHandler> logger;

        /// <summary>
        /// The unit of work.
        /// </summary>
        private readonly IUnitOfWork<ApplicationDbContext> unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentRegistrationRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="unitOfWork">
        /// The unit of work.
        /// </param>
        public AgentRegistrationRequestHandler(
            ILogger<AgentRegistrationRequestHandler> logger,
            IUnitOfWork<ApplicationDbContext> unitOfWork)
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
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<Guid> Handle(AgentRegistrationRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            this.logger.LogInformation("Registering agent {AgentName}", request.Agent!.Name);

            await using var transaction = this.unitOfWork.BeginTransaction();

            try
            {
                var agentRepository = this.unitOfWork.GetRepository<Agent>();
                var agentName = request.Agent.Name;
                var agentByNameSpec = new AgentByNameSpec(agentName);
                var agent = await agentRepository.SingleOrDefaultAsync(agentByNameSpec);

                var typeAdapterConfig = TypeAdapterConfig<AgentRegistrationDto, Agent>
                    .NewConfig()
                    .Ignore(a => a.Id)
                    .Ignore(a => a.Devices!)
                    .Config;

                if (agent is null)
                {
                    agent = request.Agent.Adapt<Agent>(typeAdapterConfig);
                    agentRepository.Add(agent);
                }
                else
                {
                    request.Agent.Adapt(agent, typeAdapterConfig);
                    agentRepository.Update(agent);
                }

                await agentRepository.SaveChangesAsync();
                var agentId = agent.Id;

                if (agent.Devices?.Count > 0)
                {
                    this.logger.LogInformation("Deleting missing devices of '{AgentName}'", request.Agent.Name);
                    var deviceRepository = this.unitOfWork.GetRepository<Device>();

                    foreach (var agentDevice in agent.Devices)
                    {
                        if (request.Agent.Devices.Exists(dto => dto.Name == agentDevice.Name) && (string.IsNullOrWhiteSpace(agentDevice.MacAddress) || string.IsNullOrWhiteSpace(agentDevice.SerialNumber)))
                        {
                            this.logger.LogInformation(
                                "Deleting partial registered device '{DeviceName}' of '{AgentName}'",
                                agentDevice.Name,
                                request.Agent.Name);

                            deviceRepository.Delete(agentDevice);
                        }
                        else if (!request.Agent.Devices.Exists(
                                     d => d.Name == agentDevice.Name
                                          || (!string.IsNullOrWhiteSpace(d.SerialNumber) && d.SerialNumber == agentDevice.SerialNumber)
                                          || (!string.IsNullOrWhiteSpace(d.MacAddress) && d.MacAddress == agentDevice.MacAddress)))
                        {
                            this.logger.LogInformation(
                                "Deleting missing device '{DeviceName}' of '{AgentName}'",
                                agentDevice.Name,
                                request.Agent.Name);

                            deviceRepository.Delete(agentDevice);
                        }
                    }

                    await deviceRepository.SaveChangesAsync();
                }

                this.logger.LogInformation("Adding or updating devices of '{AgentName}'", request.Agent.Name);
                foreach (var agentDevice in request.Agent.Devices)
                {
                    if (!string.IsNullOrWhiteSpace(agentDevice.MacAddress) || !string.IsNullOrWhiteSpace(agentDevice.SerialNumber))
                    {
                        var deviceSpec = new DeviceByMacOrSerialNumberSpec(agentDevice.MacAddress, agentDevice.SerialNumber);
                        await this.TryAddOrUpdateDeviceAsync(agentId, agentDevice, deviceSpec);
                    }
                    else
                    {
                        var deviceSpec = new DeviceByNameSpec(agentDevice.Name);
                        await this.TryAddOrUpdateDeviceAsync(agentId, agentDevice, deviceSpec);
                    }
                }

                await transaction.CommitAsync(CancellationToken.None);

                this.logger.LogInformation("Registered agent '{AgentName}'", request.Agent.Name);

                return agentId;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error registering agent and devices. Rolling back.");

                await transaction.RollbackAsync(CancellationToken.None);

                if (ex is UniqueConstraintException)
                {
                    throw this.logger.LogErrorAndCreateException<ConflictException>(
                        "Error registering agent and devices",
                        ex);
                }

                throw;
            }
        }

        /// <summary>
        /// The try add or update device async.
        /// </summary>
        /// <param name="agentId">
        /// The agent id.
        /// </param>
        /// <param name="agentDevice">
        ///     The agent device.
        /// </param>
        /// <param name="deviceSpec">
        ///     The device spec.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task TryAddOrUpdateDeviceAsync(
            Guid agentId, DeviceRegistrationDto agentDevice, ISpecification<Device> deviceSpec)
        {
            var deviceRepository = this.unitOfWork.GetRepository<Device>();

            var device = await deviceRepository.SingleOrDefaultAsync(deviceSpec);

            var deviceAdapterConfig = TypeAdapterConfig<DeviceRegistrationDto, Device>.NewConfig()
                .Ignore(d => d.Id)
                .IgnoreNullValues(true)
                .Config;

            if (device is null)
            {
                device = agentDevice.Adapt<Device>(deviceAdapterConfig);
                deviceRepository.Add(device);
            }
            else
            {
                agentDevice.Adapt(device, deviceAdapterConfig);
            }

            device.AgentId = agentId;
            deviceRepository.Update(device);

            await deviceRepository.SaveChangesAsync();
        }
    }
}