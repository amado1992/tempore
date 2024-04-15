namespace Tempore.Tests.Tempore.Agent.Services
{
    extern alias TemporeAgent;

    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
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

    using TemporeAgent::Tempore.Agent.Entities;
    using TemporeAgent::Tempore.Agent.Services;

    using Xunit;

    public partial class AgentReceiverFacts
    {
        public class The_RegisterAsync_Method
        {
            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Throws_ArgumentException_If_AgentName_Configuration_Value_Is_Missing_Async()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddHikvision();

                var configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build();
                await using var buildServiceProvider = serviceCollection.BuildServiceProvider();
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

                await Assert.ThrowsAsync<ArgumentException>(() => agentReceiver.RegisterAsync(Guid.NewGuid().ToString()));
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Adds_Agent_And_Devices_To_Local_Storage_Async()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddHikvision();

                const string AgentName = "Main Agent";
                var configurationData = new Dictionary<string, string>
                {
                    ["AgentName"] = AgentName,

                    ["Devices:0:Name"] = Fixtures.TestEnvironment.Device_00.Name,
                    ["Devices:0:IpAddress"] = Fixtures.TestEnvironment.Device_00.IpAddress,
                    ["Devices:0:Username"] = Fixtures.TestEnvironment.Device_00.Username,
                    ["Devices:0:Password"] = Fixtures.TestEnvironment.Device_00.Password,

                    ["Devices:1:Name"] = Fixtures.TestEnvironment.Device_01.Name,
                    ["Devices:1:IpAddress"] = Fixtures.TestEnvironment.Device_01.IpAddress,
                    ["Devices:1:Username"] = Fixtures.TestEnvironment.Device_01.Username,
                    ["Devices:1:Password"] = Fixtures.TestEnvironment.Device_01.Password,
                };

                var configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(configurationData).Build();

                var unitOfWorkMock = new Mock<IUnitOfWork<ApplicationDbContext>>();
                var agentRepositoryMock = new Mock<IRepository<Agent, ApplicationDbContext>>();
                var deviceRepositoryMock = new Mock<IRepository<Device, ApplicationDbContext>>();

                unitOfWorkMock.Setup(u => u.GetRepository<Agent>()).Returns(agentRepositoryMock.Object);
                unitOfWorkMock.Setup(u => u.GetRepository<Device>()).Returns(deviceRepositoryMock.Object);

                var agentClientMock = new Mock<IAgentClient>();
                agentClientMock.Setup(client => client.GetAgentByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Guid agentId) => new AgentDto
                {
                    Id = agentId,
                    Name = AgentName,
                });

                var deviceClientMock = new Mock<IDeviceClient>();
                var response = new DeviceDtoPagedResponse
                {
                    Count = 2,
                    Items = new List<DeviceDto>
                              {
                                  Fixtures.TestEnvironment.Device_00.Instance,
                                  Fixtures.TestEnvironment.Device_01.Instance,
                              },
                };

                deviceClientMock
                    .Setup(client => client.GetDevicesByAgentIdAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(response);

                await using (var buildServiceProvider = serviceCollection.BuildServiceProvider())
                {
                    var agentReceiver = new AgentReceiver(
                        NullLogger<AgentReceiver>.Instance,
                        configurationRoot,
                        agentClientMock.Object,
                        deviceClientMock.Object,
                        new Mock<IEmployeeClient>().Object,
                        new Mock<ITimestampClient>().Object,
                        new ConfigurationBasedDeviceInfoRepository(
                            NullLogger<ConfigurationBasedDeviceInfoRepository>.Instance,
                            configurationRoot),
                        unitOfWorkMock.Object,
                        new HikvisionDeviceConnectionFactory(buildServiceProvider));

                    await agentReceiver.RegisterAsync(Guid.NewGuid().ToString());
                }

                agentRepositoryMock.Verify(repository => repository.Add(It.IsAny<Agent>()), Times.Once);
                deviceRepositoryMock.Verify(repository => repository.Add(It.IsAny<Device>()), Times.Exactly(response.Items.Count));
                unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Exactly(1));
            }

            [Fact]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Updates_Agent_And_Devices_To_Local_Storage_Async()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddHikvision();

                const string AgentName = "Main Agent";
                var configurationData = new Dictionary<string, string>
                {
                    ["AgentName"] = AgentName,

                    ["Devices:0:Name"] = Fixtures.TestEnvironment.Device_00.Name,
                    ["Devices:0:IpAddress"] = Fixtures.TestEnvironment.Device_00.IpAddress,
                    ["Devices:0:Username"] = Fixtures.TestEnvironment.Device_00.Username,
                    ["Devices:0:Password"] = Fixtures.TestEnvironment.Device_00.Password,

                    ["Devices:1:Name"] = Fixtures.TestEnvironment.Device_01.Name,
                    ["Devices:1:IpAddress"] = Fixtures.TestEnvironment.Device_01.IpAddress,
                    ["Devices:1:Username"] = Fixtures.TestEnvironment.Device_01.Username,
                    ["Devices:1:Password"] = Fixtures.TestEnvironment.Device_01.Password,
                };

                var configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(configurationData).Build();
                var unitOfWorkMock = new Mock<IUnitOfWork<ApplicationDbContext>>();
                var agentRepositoryMock = new Mock<IRepository<Agent, ApplicationDbContext>>();
                agentRepositoryMock
                    .Setup(repository => repository.SingleOrDefaultAsync(It.IsAny<Expression<Func<Agent, bool>>>()))
                    .ReturnsAsync(new Agent());

                var deviceRepositoryMock = new Mock<IRepository<Device, ApplicationDbContext>>();
                deviceRepositoryMock
                    .Setup(repository => repository.SingleOrDefaultAsync(It.IsAny<Expression<Func<Device, bool>>>()))
                    .ReturnsAsync(new Device());

                unitOfWorkMock.Setup(u => u.GetRepository<Agent>()).Returns(agentRepositoryMock.Object);
                unitOfWorkMock.Setup(u => u.GetRepository<Device>()).Returns(deviceRepositoryMock.Object);

                var agentClientMock = new Mock<IAgentClient>();
                agentClientMock.Setup(client => client.GetAgentByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Guid agentId) => new AgentDto
                {
                    Id = agentId,
                    Name = AgentName,
                });

                var deviceClientMock = new Mock<IDeviceClient>();
                var response = new DeviceDtoPagedResponse
                {
                    Count = 2,
                    Items = new List<DeviceDto>
                              {
                                  Fixtures.TestEnvironment.Device_00.Instance,
                                  Fixtures.TestEnvironment.Device_01.Instance,
                              },
                };

                deviceClientMock
                    .Setup(client => client.GetDevicesByAgentIdAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(response);

                await using (var buildServiceProvider = serviceCollection.BuildServiceProvider())
                {
                    var agentReceiver = new AgentReceiver(
                        NullLogger<AgentReceiver>.Instance,
                        configurationRoot,
                        agentClientMock.Object,
                        deviceClientMock.Object,
                        new Mock<IEmployeeClient>().Object,
                        new Mock<ITimestampClient>().Object,
                        new ConfigurationBasedDeviceInfoRepository(
                            NullLogger<ConfigurationBasedDeviceInfoRepository>.Instance,
                            configurationRoot),
                        unitOfWorkMock.Object,
                        new HikvisionDeviceConnectionFactory(buildServiceProvider));

                    await agentReceiver.RegisterAsync(Guid.NewGuid().ToString());
                }

                agentRepositoryMock.Verify(repository => repository.Update(It.IsAny<Agent>()), Times.Once);
                deviceRepositoryMock.Verify(repository => repository.Update(It.IsAny<Device>()), Times.Exactly(response.Items.Count));
                unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Exactly(1));
            }
        }
    }
}