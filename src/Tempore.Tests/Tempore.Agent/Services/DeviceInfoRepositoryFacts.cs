namespace Tempore.Tests.Tempore.Agent.Services
{
    extern alias TemporeAgent;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using FluentAssertions;

    using global::Tempore.Tests.Infraestructure;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;

    using TemporeAgent::Tempore.Agent.Services;

    using Xunit;

    public class DeviceInfoRepositoryFacts
    {
        public class The_Device_Property
        {
            [Theory]
            [Trait(Traits.Category, Category.Unit)]
            [InlineData("18/01/2023")]
            [InlineData("30/06/2024")]
            [InlineData("31/01/2025")]
            [InlineData("06/10/2025")]
            public void Binds_The_Configuration_Value_FirstDateOnline_From_Device_Propertly(string dateTime)
            {
                var configurationData = new Dictionary<string, string>
                {
                    ["Devices:0:Name"] = Fixtures.TestEnvironment.Device_00.Name,
                    ["Devices:0:IpAddress"] = Fixtures.TestEnvironment.Device_00.IpAddress,
                    ["Devices:0:Username"] = Fixtures.TestEnvironment.Device_00.Username,
                    ["Devices:0:Password"] = Fixtures.TestEnvironment.Device_00.Password,
                    ["Devices:0:FirstDateOnline"] = dateTime,
                };

                var configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(configurationData).Build();

                var deviceInfoRepository = new ConfigurationBasedDeviceInfoRepository(NullLogger<ConfigurationBasedDeviceInfoRepository>.Instance, configurationRoot);
                var deviceInfo = deviceInfoRepository.Devices.First();

                deviceInfo.Should().NotBeNull();
                deviceInfo.FirstDateOnline.Should().Be(DateTime.ParseExact(dateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture));
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Sets_The_First_Day_Of_The_Past_Year_When_FirstDateOnline_Is_Not_Specified_For_The_Device()
            {
                var configurationData = new Dictionary<string, string>
                {
                    ["Devices:0:Name"] = Fixtures.TestEnvironment.Device_00.Name,
                    ["Devices:0:IpAddress"] = Fixtures.TestEnvironment.Device_00.IpAddress,
                    ["Devices:0:Username"] = Fixtures.TestEnvironment.Device_00.Username,
                    ["Devices:0:Password"] = Fixtures.TestEnvironment.Device_00.Password,
                };

                var configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(configurationData).Build();

                var deviceInfoRepository = new ConfigurationBasedDeviceInfoRepository(
                    NullLogger<ConfigurationBasedDeviceInfoRepository>.Instance,
                    configurationRoot);
                var deviceInfo = deviceInfoRepository.Devices.First();

                deviceInfo.Should().NotBeNull();
                deviceInfo.FirstDateOnline.Should().Be(new DateTime(DateTime.Now.Year - 1, 1, 1));
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Deserializes_FirstDateOnline_Correctly_Date_In_The_ddMMyyyy_Format()
            {
                var configurationData = new Dictionary<string, string>
                {
                    ["Devices:0:Name"] = Fixtures.TestEnvironment.Device_00.Name,
                    ["Devices:0:IpAddress"] = Fixtures.TestEnvironment.Device_00.IpAddress,
                    ["Devices:0:Username"] = Fixtures.TestEnvironment.Device_00.Username,
                    ["Devices:0:Password"] = Fixtures.TestEnvironment.Device_00.Password,
                    ["Devices:0:FirstDateOnline"] = "23/12/2023",
                };

                var configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(configurationData).Build();

                var deviceInfoRepository = new ConfigurationBasedDeviceInfoRepository(
                    NullLogger<ConfigurationBasedDeviceInfoRepository>.Instance,
                    configurationRoot);
                var deviceInfo = deviceInfoRepository.Devices.First();

                deviceInfo.Should().NotBeNull();
                deviceInfo.FirstDateOnline.Should().Be(new DateTime(2023, 12, 23));
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public void Sets_Default_Value_For_FirstDateOnline_When_Is_Not_In_The_ddMMyyyy_Format()
            {
                var configurationData = new Dictionary<string, string>
                {
                    ["Devices:0:Name"] = Fixtures.TestEnvironment.Device_00.Name,
                    ["Devices:0:IpAddress"] = Fixtures.TestEnvironment.Device_00.IpAddress,
                    ["Devices:0:Username"] = Fixtures.TestEnvironment.Device_00.Username,
                    ["Devices:0:Password"] = Fixtures.TestEnvironment.Device_00.Password,
                    ["Devices:0:FirstDateOnline"] = "12/23/2023",
                };

                var configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(configurationData).Build();

                var deviceInfoRepository = new ConfigurationBasedDeviceInfoRepository(
                    NullLogger<ConfigurationBasedDeviceInfoRepository>.Instance,
                    configurationRoot);
                var deviceInfo = deviceInfoRepository.Devices.First();

                deviceInfo.Should().NotBeNull();
                deviceInfo.FirstDateOnline.Should().Be(new DateTime(DateTime.Now.Year - 1, 1, 1));
            }
        }
    }
}