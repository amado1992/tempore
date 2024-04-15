namespace Tempore.Tests.Tempore.Agent.Services
{
    extern alias TemporeAgent;

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Tempore.Client;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;
    using StoneAssemblies.Hikvision.Extensions;
    using StoneAssemblies.Hikvision.Services;

    using TemporeAgent::Tempore.Agent.Services;

    using Xunit;

    public partial class AgentReceiverFacts
    {
        public class The_UploadEmployeesTimestampsAsync_Method
        {
            [Fact]
            [Trait(Traits.Category, Category.Development)]
            public async Task Works_As_Expected_Async()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddHikvision();

                var configurationData = new Dictionary<string, string>
                {
                    ["Devices:0:Name"] = "Main Door",
                    ["Devices:0:IpAddress"] = "192.168.1.10",
                    ["Devices:0:Username"] = "admin",
                    ["Devices:0:Password"] = "h1kv1s1on123",
                    ["Devices:0:FirstDateOnline"] = "18/1/2023",
                };

                var configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(configurationData).Build();

                using var buildServiceProvider = serviceCollection.BuildServiceProvider();
                var agentReceiver = new AgentReceiver(
                    NullLogger<AgentReceiver>.Instance,
                    configurationRoot,
                    new Mock<IAgentClient>().Object,
                    new Mock<IDeviceClient>().Object,
                    new Mock<IEmployeeClient>().Object,
                    new Mock<ITimestampClient>().Object,
                    new ConfigurationBasedDeviceInfoRepository(
                        NullLogger<ConfigurationBasedDeviceInfoRepository>.Instance,
                        configurationRoot),
                    new Mock<IUnitOfWork<ApplicationDbContext>>().Object,
                    new HikvisionDeviceConnectionFactory(buildServiceProvider));

                await agentReceiver.UploadEmployeesTimestampsAsync(
                    Guid.NewGuid(),
                    new DateTimeOffset(2023, 9, 4, 22, 0, 0, TimeSpan.FromHours(-5)),
                    new DateTimeOffset(2023, 9, 5, 1, 0, 0, TimeSpan.FromHours(-5)));
            }
        }
    }
}