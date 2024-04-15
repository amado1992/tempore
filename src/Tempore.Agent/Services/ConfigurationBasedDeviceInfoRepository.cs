// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationBasedDeviceInfoRepository.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Agent.Services
{
    using System.Globalization;

    using Tempore.Agent.Configurations;
    using Tempore.Agent.Services.Interfaces;

    /// <summary>
    /// The configuration device repository.
    /// </summary>
    public class ConfigurationBasedDeviceInfoRepository : IDeviceInfoRepository
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ConfigurationBasedDeviceInfoRepository> logger;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationBasedDeviceInfoRepository"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public ConfigurationBasedDeviceInfoRepository(
            ILogger<ConfigurationBasedDeviceInfoRepository> logger,
            IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(configuration);

            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        /// Gets the devices.
        /// </summary>
        public IEnumerable<DeviceInfo> Devices
        {
            get
            {
                foreach (var section in this.configuration.GetSection("Devices").GetChildren())
                {
                    var deviceInfo = new DeviceInfo
                    {
                        Name = section[nameof(DeviceInfo.Name)],
                        IpAddress = section[nameof(DeviceInfo.IpAddress)],
                        Username = section[nameof(DeviceInfo.Username)],
                        Password = section[nameof(DeviceInfo.Password)],
                    };

                    if (int.TryParse(section[nameof(DeviceInfo.Port)], out var port))
                    {
                        deviceInfo.Port = port;
                    }

                    if (bool.TryParse(section[nameof(DeviceInfo.SynchronizeTime)], out var synchronizeTime))
                    {
                        deviceInfo.SynchronizeTime = synchronizeTime;
                    }

                    if (DateTime.TryParseExact(section[nameof(DeviceInfo.FirstDateOnline)], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var firstDateOnline))
                    {
                        deviceInfo.FirstDateOnline = firstDateOnline;
                    }

                    yield return deviceInfo;
                }
            }
        }
    }
}