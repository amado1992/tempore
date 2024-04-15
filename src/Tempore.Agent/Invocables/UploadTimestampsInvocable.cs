// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTimestampsInvocable.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Invocables
{
    using Coravel.Invocable;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;
    using StoneAssemblies.Hikvision.Services;
    using StoneAssemblies.Hikvision.Services.Interfaces;

    using Tempore.Agent.Entities;
    using Tempore.Agent.Services;
    using Tempore.Agent.Services.Interfaces;
    using Tempore.Client;

    /// <summary>
    /// The upload timestamps invocable.
    /// </summary>
    public class UploadTimestampsInvocable : IInvocable
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UploadTimestampsInvocable> logger;

        /// <summary>
        /// The hikvision device connection factory.
        /// </summary>
        private readonly IHikvisionDeviceConnectionFactory hikvisionDeviceConnectionFactory;

        /// <summary>
        /// The device repository.
        /// </summary>
        private readonly IRepository<Device, ApplicationDbContext> deviceRepository;

        /// <summary>
        /// The device info repository.
        /// </summary>
        private readonly IDeviceInfoRepository deviceInfoRepository;

        /// <summary>
        /// The timestamp client.
        /// </summary>
        private readonly ITimestampClient timestampClient;

        /// <summary>
        /// The employee client.
        /// </summary>
        private readonly IEmployeeClient employeeClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadTimestampsInvocable"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="hikvisionDeviceConnectionFactory">
        /// The device info connection factory.
        /// </param>
        /// <param name="deviceRepository">
        /// The device repository.
        /// </param>
        /// <param name="deviceInfoRepository">
        /// The device info repository.
        /// </param>
        /// <param name="timestampClient">
        /// The timestamp client.
        /// </param>
        /// <param name="employeeClient">
        /// The employee client.
        /// </param>
        public UploadTimestampsInvocable(
            ILogger<UploadTimestampsInvocable> logger,
            IHikvisionDeviceConnectionFactory hikvisionDeviceConnectionFactory,
            IRepository<Device, ApplicationDbContext> deviceRepository,
            IDeviceInfoRepository deviceInfoRepository,
            ITimestampClient timestampClient,
            IEmployeeClient employeeClient)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(hikvisionDeviceConnectionFactory);
            ArgumentNullException.ThrowIfNull(deviceRepository);
            ArgumentNullException.ThrowIfNull(deviceInfoRepository);
            ArgumentNullException.ThrowIfNull(timestampClient);
            ArgumentNullException.ThrowIfNull(employeeClient);

            this.logger = logger;
            this.hikvisionDeviceConnectionFactory = hikvisionDeviceConnectionFactory;
            this.deviceRepository = deviceRepository;
            this.deviceInfoRepository = deviceInfoRepository;
            this.timestampClient = timestampClient;
            this.employeeClient = employeeClient;
        }

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Invoke()
        {
            foreach (var device in this.deviceRepository.All())
            {
                var deviceInfo = this.deviceInfoRepository.Devices.FirstOrDefault(info => info.Name == device.Name);
                if (deviceInfo is null)
                {
                    this.logger.LogWarning("Device {DeviceName} not found in configuration.", device.Name);
                    continue;
                }

                var deviceId = device.Id;

                var deviceConnection = this.hikvisionDeviceConnectionFactory.Create(
                    deviceInfo.Url,
                    deviceInfo.Username!,
                    deviceInfo.Password!);

                var acsEventsClient = deviceConnection.GetClient<IAcsEventsClient>();

                device.LastTransferredTimestampDateTime ??= new DateTimeOffset(deviceInfo.FirstDateOnline);
                await foreach (var eventInfo in acsEventsClient.ListAcsEventsAsync(
                                   device.LastTransferredTimestampDateTime.Value.DateTime,
                                   DateTime.Now,
                                   AccessControlEventTypes.Event,
                                   EventMinorTypes.FingerprintComparePass))
                {
                    var dateTimeOffset = DateTimeOffset.Parse(eventInfo.Time);
                    var employeeIdOnDevice = eventInfo.EmployeeNoString;

                    var retry = false;
                    do
                    {
                        try
                        {
                            this.logger.LogInformation(
                                "Adding timestamp '{DateTime}' to employee '{EmployeeIdOnDevice}' from device '{DeviceId}'",
                                dateTimeOffset,
                                employeeIdOnDevice,
                                deviceId);

                            await this.timestampClient.AddEmployeeFromDeviceTimestampAsync(AddEmployeeFromDeviceTimestampRequest.Create(deviceId, dateTimeOffset, employeeIdOnDevice));

                            retry = false;
                        }
                        catch (ApiException ex) when (ex.StatusCode == StatusCodes.Status404NotFound)
                        {
                            if (retry)
                            {
                                break;
                            }

                            this.logger.LogError(
                                ex,
                                "Error adding timestamp '{DateTime}' to employee '{EmployeeIdOnDevice}' from device '{DeviceId}'. Will add the employee and retry.",
                                dateTimeOffset,
                                employeeIdOnDevice,
                                deviceId);

                            var userInfoClient = deviceConnection.GetClient<IUserInfoClient>();
                            var userInfo = await userInfoClient.ListUserAsync(employeeIdOnDevice)
                                               .FirstOrDefaultAsync();
                            if (userInfo is not null)
                            {
                                this.logger.LogInformation(
                                    "Adding employee '{EmployeeIdOnDevice}' from device '{DeviceId}'",
                                    employeeIdOnDevice,
                                    deviceId);

                                await this.employeeClient.AddEmployeeFromDeviceAsync(AddEmployeeFromDeviceRequest.Create(deviceId, userInfo.EmployeeNo, userInfo.Name));

                                retry = true;
                            }
                        }
                        catch (ApiException ex) when (ex.StatusCode == StatusCodes.Status409Conflict)
                        {
                            this.logger.LogWarning(
                                ex,
                                "Timestamp '{DateTime}' already registered for the employee '{EmployeeIdOnDevice}' from device '{DeviceId}'.",
                                dateTimeOffset,
                                employeeIdOnDevice,
                                deviceId);
                        }
                    }
                    while (retry);

                    // TODO: Review this. When assign this value? This is the better way to track this?
                    device.LastTransferredTimestampDateTime = dateTimeOffset.AddSeconds(1);
                }

                this.deviceRepository.Update(device);
                await this.deviceRepository.SaveChangesAsync();
            }
        }
    }
}