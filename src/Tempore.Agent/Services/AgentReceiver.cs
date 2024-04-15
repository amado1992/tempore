// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentReceiver.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Services
{
    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;
    using StoneAssemblies.Hikvision.Services;
    using StoneAssemblies.Hikvision.Services.Interfaces;

    using Tempore.Agent.Configurations;
    using Tempore.Agent.Entities;
    using Tempore.Agent.Services.Interfaces;
    using Tempore.Client;
    using Tempore.Client.Services.Interfaces;
    using Tempore.Logging.Extensions;

    using ISystemClient = StoneAssemblies.Hikvision.Services.Interfaces.ISystemClient;

    /// <summary>
    /// The agent client.
    /// </summary>
    public class AgentReceiver : IAgentReceiver
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AgentReceiver> logger;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The agent client.
        /// </summary>
        private readonly IAgentClient agentClient;

        /// <summary>
        /// The device client.
        /// </summary>
        private readonly IDeviceClient deviceClient;

        /// <summary>
        /// The employee client.
        /// </summary>
        private readonly IEmployeeClient employeeClient;

        /// <summary>
        /// The timestamp client.
        /// </summary>
        private readonly ITimestampClient timestampClient;

        /// <summary>
        /// The device repository.
        /// </summary>
        private readonly IDeviceInfoRepository deviceInfoInfoRepository;

        /// <summary>
        /// The unit of work.
        /// </summary>
        private readonly IUnitOfWork<ApplicationDbContext> unitOfWork;

        /// <summary>
        /// The device connection factory.
        /// </summary>
        private readonly IHikvisionDeviceConnectionFactory deviceConnectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentReceiver"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="agentClient">
        /// The agent client.
        /// </param>
        /// <param name="deviceClient">
        /// The device client.
        /// </param>
        /// <param name="employeeClient">
        /// The employee client.
        /// </param>
        /// <param name="timestampClient">
        /// The timestamp client.
        /// </param>
        /// <param name="deviceInfoInfoRepository">
        /// The device repository.
        /// </param>
        /// <param name="unitOfWork">
        /// The unit of work.
        /// </param>
        /// <param name="deviceConnectionFactory">
        /// The device connection factory.
        /// </param>
        public AgentReceiver(
            ILogger<AgentReceiver> logger,
            IConfiguration configuration,
            IAgentClient agentClient,
            IDeviceClient deviceClient,
            IEmployeeClient employeeClient,
            ITimestampClient timestampClient,
            IDeviceInfoRepository deviceInfoInfoRepository,
            IUnitOfWork<ApplicationDbContext> unitOfWork,
            IHikvisionDeviceConnectionFactory deviceConnectionFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(agentClient);
            ArgumentNullException.ThrowIfNull(deviceClient);
            ArgumentNullException.ThrowIfNull(employeeClient);
            ArgumentNullException.ThrowIfNull(deviceClient);
            ArgumentNullException.ThrowIfNull(timestampClient);
            ArgumentNullException.ThrowIfNull(deviceInfoInfoRepository);
            ArgumentNullException.ThrowIfNull(unitOfWork);
            ArgumentNullException.ThrowIfNull(deviceConnectionFactory);

            this.logger = logger;
            this.configuration = configuration;
            this.agentClient = agentClient;
            this.deviceClient = deviceClient;
            this.employeeClient = employeeClient;
            this.timestampClient = timestampClient;
            this.deviceInfoInfoRepository = deviceInfoInfoRepository;
            this.unitOfWork = unitOfWork;
            this.deviceConnectionFactory = deviceConnectionFactory;
        }

        /// <summary>
        /// The register async.
        /// </summary>
        /// <param name="connectionId">
        /// The connection id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task RegisterAsync(string connectionId)
        {
            var agentId = await this.RegisterAgentAsync(connectionId);
            await this.SynchronizeConfigurationAsync(agentId);
        }

        /// <inheritdoc />
        public async Task UploadEmployeesAsync(Guid deviceId)
        {
            this.logger.LogInformation("Uploading employees from device {DeviceId}", deviceId);

            var deviceInfo = await this.TryGetDeviceInfoAsync(deviceId);
            if (deviceInfo is null)
            {
                return;
            }

            var hikvisionDeviceConnection = this.deviceConnectionFactory.Create(
                deviceInfo.Url,
                deviceInfo.Username!,
                deviceInfo.Password!);

            var userInfoClient = hikvisionDeviceConnection.GetClient<IUserInfoClient>();
            await foreach (var userInfo in userInfoClient.ListUserAsync())
            {
                var employeeFromDeviceDto = new EmployeeFromDeviceDto
                {
                    DeviceId = deviceId,
                    EmployeeIdOnDevice = userInfo.EmployeeNo,
                    FullName = userInfo.Name,
                };

                try
                {
                    await this.employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(employeeFromDeviceDto));
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, "Error loading employee from devices");
                }
            }
        }

        /// <inheritdoc />
        public async Task UploadEmployeesTimestampsAsync(Guid deviceId, DateTimeOffset from, DateTimeOffset? to)
        {
            var deviceInfo = await this.TryGetDeviceInfoAsync(deviceId);
            if (deviceInfo is null)
            {
                return;
            }

            var hikvisionDeviceConnection = this.deviceConnectionFactory.Create(
                deviceInfo.Url,
                deviceInfo.Username!,
                deviceInfo.Password!);

            var acsEventsClient = hikvisionDeviceConnection.GetClient<IAcsEventsClient>();
            var tasks = new List<Task>();
            await foreach (var eventInfo in acsEventsClient.ListAcsEventsAsync(
                               from.DateTime,
                               to!.Value.DateTime,
                               AccessControlEventTypes.Event,
                               EventMinorTypes.FingerprintComparePass))
            {
                var employeeFromDeviceDto = new EmployeeFromDeviceDto
                {
                    DeviceId = deviceId,
                    EmployeeIdOnDevice = eventInfo.EmployeeNoString,
                };

                var timestampDto = new TimestampDto
                {
                    EmployeeFromDevice = employeeFromDeviceDto,
                    DateTime = DateTimeOffset.Parse(eventInfo.Time),
                };

                tasks.Add(this.timestampClient.AddEmployeeFromDeviceTimestampAsync(AddEmployeeFromDeviceTimestampRequest.Create(timestampDto)));
            }

            await Task.WhenAll(tasks);
        }

        /// <inheritdoc />
        public async Task ReportDeviceStateAsync(Guid deviceId)
        {
            var deviceInfo = await this.TryGetDeviceInfoAsync(deviceId);
            if (deviceInfo is null)
            {
                return;
            }

            var deviceName = deviceInfo.Name;
            var updateDeviceStateRequest = new UpdateDeviceStateRequest
            {
                DeviceId = deviceId,
            };

            try
            {
                var hikvisionDeviceConnection = this.deviceConnectionFactory.Create(
                    deviceInfo.Url,
                    deviceInfo.Username!,
                    deviceInfo.Password!);
                var systemClient = hikvisionDeviceConnection.GetClient<ISystemClient>();

                this.logger.LogInformation("Getting device '{DeviceName}' information", deviceInfo.Name);

                _ = await systemClient.GetDeviceInfoAsync();

                updateDeviceStateRequest.DeviceState = DeviceState.Online;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error retrieving device {DeviceName} information", deviceName);
                updateDeviceStateRequest.DeviceState = DeviceState.Offline;
            }

            try
            {
                this.logger.LogInformation("Updating device '{DeviceName}' state to {DeviceState}", deviceName, updateDeviceStateRequest.DeviceState);

                await this.deviceClient.UpdateDeviceStateAsync(updateDeviceStateRequest);

                this.logger.LogInformation("Updated device '{DeviceName}' state to {DeviceState}", deviceName, updateDeviceStateRequest.DeviceState);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error updating device {DeviceName} state", deviceName);
            }
        }

        private async Task<Guid> RegisterAgentAsync(string connectionId)
        {
            var agentName = this.configuration["AgentName"];
            if (string.IsNullOrWhiteSpace(agentName))
            {
                throw this.logger.LogErrorAndCreateException<ArgumentException>("Missing required 'AgentName' configuration value");
            }

            this.logger.LogInformation("Registering Agent {AgentName}", agentName);
            var agentDto = new AgentRegistrationDto
            {
                Name = agentName,
                ConnectionId = connectionId,
            };

            var devices = new List<DeviceRegistrationDto>();
            foreach (var device in this.deviceInfoInfoRepository.Devices)
            {
                // TODO: Update device.
                var deviceDto = new DeviceRegistrationDto
                {
                    Name = device.Name,
                };

                try
                {
                    var hikvisionDeviceConnection = this.deviceConnectionFactory.Create(
                        device.Url,
                        device.Username!,
                        device.Password!);
                    var systemClient = hikvisionDeviceConnection.GetClient<ISystemClient>();

                    this.logger.LogInformation("Getting device {DeviceName} information", device.Name);

                    var deviceInfo = await systemClient.GetDeviceInfoAsync();

                    deviceDto.DeviceName = deviceInfo.DeviceName;
                    deviceDto.MacAddress = deviceInfo.MacAddress;
                    deviceDto.SerialNumber = deviceInfo.SerialNumber;
                    deviceDto.Model = deviceInfo.Model;
                    deviceDto.State = DeviceState.Online;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error occurred connecting to device {DeviceName}", device.Name);

                    deviceDto.State = DeviceState.Offline;
                }

                devices.Add(deviceDto);
            }

            agentDto.Devices = devices;

            return await this.agentClient.RegisterAgentAsync(AgentRegistrationRequest.Create(agentDto));
        }

        private async Task SynchronizeConfigurationAsync(Guid agentId)
        {
            this.logger.LogInformation("Synchronizing agent registration info");

            var registeredAgent = await this.agentClient.GetAgentByIdAsync(agentId);

            var agentRepository = this.unitOfWork.GetRepository<Agent>();
            var storedAgent = await agentRepository.SingleOrDefaultAsync(agent => agent.Id == registeredAgent.Id);
            if (storedAgent is null)
            {
                var newAgent = new Agent
                {
                    Id = agentId,
                    Name = registeredAgent.Name,
                };

                agentRepository.Add(newAgent);
            }
            else
            {
                storedAgent.Name = registeredAgent.Name;
                agentRepository.Update(storedAgent);
            }

            this.logger.LogInformation("Synchronizing devices registration info");

            var repository = this.unitOfWork.GetRepository<Device>();
            var response = await this.deviceClient.GetDevicesByAgentIdAsync(agentId, 0, int.MaxValue);

            var deviceIds = new List<Guid>();

            foreach (var device in response.Items)
            {
                deviceIds.Add(device.Id);
                var storedDevice = await repository.SingleOrDefaultAsync(d => d.Id == device.Id);
                if (storedDevice is null)
                {
                    var entity = new Device
                    {
                        Id = device.Id,
                        Name = device.Name,
                        AgentId = agentId,
                    };

                    repository.Add(entity);
                }
                else
                {
                    storedDevice.Name = device.Name;
                    storedDevice.AgentId = agentId;
                    repository.Update(storedDevice);
                }
            }

            repository.Delete(device => !deviceIds.Contains(device.Id));
            agentRepository.Delete(agent => agent.Id != agentId);

            await this.unitOfWork.SaveChangesAsync();
        }

        private async Task<DeviceInfo?> TryGetDeviceInfoAsync(Guid deviceId)
        {
            var deviceRepository = this.unitOfWork.GetRepository<Device>();
            var device = await deviceRepository.SingleOrDefaultAsync(d => d.Id == deviceId);
            if (device is null)
            {
                this.logger.LogError("Device Id '{DeviceId}' not found in local database.", deviceId);
                return null;
            }

            var deviceInfo = this.deviceInfoInfoRepository.Devices.FirstOrDefault(info => info.Name == device.Name);
            if (deviceInfo is null)
            {
                this.logger.LogError("Device with Id '{DeviceId}' named '{DeviceName}' not found in configuration.", deviceId, device.Name);
            }

            return deviceInfo;
        }
    }
}