namespace Tempore.Tests.Tempore.Agent.Invocables
{
    extern alias TemporeAgent;

    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Tempore.Client;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;
    using StoneAssemblies.Hikvision.Extensions;
    using StoneAssemblies.Hikvision.Models;
    using StoneAssemblies.Hikvision.Services;
    using StoneAssemblies.Hikvision.Services.Interfaces;

    using TemporeAgent::Tempore.Agent;
    using TemporeAgent::Tempore.Agent.Entities;
    using TemporeAgent::Tempore.Agent.Invocables;
    using TemporeAgent::Tempore.Agent.Services;
    using TemporeAgent::Tempore.Agent.Services.Interfaces;

    using Xunit;

    using DeviceInfo = TemporeAgent::Tempore.Agent.Configurations.DeviceInfo;

    public class UploadTimestampsInvocableFacts
    {
        public class The_Invoke_Method
        {
            public static IEnumerable<object[]> Events()
            {
                yield return new object[]
                {
                    new List<AcsEventInfo>
                                    {
                                        new AcsEventInfo
                                        {
                                            EmployeeNoString = "0001",
                                            Time = DateTime.Now.AddSeconds(-5).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                        },
                                        new AcsEventInfo
                                        {
                                            EmployeeNoString = "0002",
                                            Time = DateTime.Now.AddSeconds(-4).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                        },
                                        new AcsEventInfo
                                        {
                                            EmployeeNoString = "0003",
                                            Time = DateTime.Now.AddSeconds(-3).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                        },
                                        new AcsEventInfo
                                        {
                                            EmployeeNoString = "0004",
                                            Time = DateTime.Now.AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                        },
                                    },
                };

                yield return new object[]
                {
                    new List<AcsEventInfo>
                                    {
                                        new AcsEventInfo
                                        {
                                            EmployeeNoString = "0001",
                                            Time = DateTime.Now.AddSeconds(-2).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                        },
                                        new AcsEventInfo
                                        {
                                            EmployeeNoString = "0002",
                                            Time = DateTime.Now.AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                        },
                                    },
                };

                yield return new object[]
                {
                    new List<AcsEventInfo>
                                    {
                                        new AcsEventInfo
                                        {
                                            EmployeeNoString = "0001",
                                            Time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                        },
                                    },
                };
            }

            [Fact]
            [Trait(Traits.Category, Category.Development)]
            public async Task Works_As_Expected_Async()
            {
                var deviceRepositoryMock = new Mock<IRepository<Device, ApplicationDbContext>>();

                var device = new Device
                {
                    Id = Guid.NewGuid(),
                    Name = Fixtures.TestEnvironment.Device_00.Name,
                };

                var devices = new List<Device>
                              {
                                  device,
                              };

                deviceRepositoryMock.Setup(repository => repository.All())
                    .Returns(new EnumerableQuery<Device>(devices));

                var deviceInfoRepositoryMock = new Mock<IDeviceInfoRepository>();
                var deviceInfo = new DeviceInfo
                {
                    Name = device.Name,
                    Username = Fixtures.TestEnvironment.Device_00.Username,
                    Password = Fixtures.TestEnvironment.Device_00.Password,
                    IpAddress = Fixtures.TestEnvironment.Device_00.IpAddress,
                };

                deviceInfoRepositoryMock.Setup(repository => repository.Devices).Returns(
                    () => new List<DeviceInfo>
                          {
                              deviceInfo,
                          });

                var hikvisionDeviceConnectionFactoryMock = new Mock<IHikvisionDeviceConnectionFactory>();
                var hikvisionDeviceConnection = new Mock<IHikvisionDeviceConnection>();
                hikvisionDeviceConnectionFactoryMock.Setup(
                    factory => factory.Create(
                        deviceInfo.Url,
                        deviceInfo.Username,
                        deviceInfo.Password)).Returns(hikvisionDeviceConnection.Object);

                var httpMessageHandler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential(deviceInfo.Username, deviceInfo.Password),
                };

                using var httpClient = new HttpClient(httpMessageHandler)
                {
                    BaseAddress = new Uri(deviceInfo.Url),
                };

                hikvisionDeviceConnection.Setup(connection => connection.GetClient<IAcsEventsClient>())
                    .Returns(new AcsEventsClient(httpClient, new SearchIdGenerationService()));

                var timestampClientMock = new Mock<ITimestampClient>();
                var employeeClientMock = new Mock<IEmployeeClient>();

                var uploadTimestampsInvocable = new UploadTimestampsInvocable(
                    NullLogger<UploadTimestampsInvocable>.Instance,
                    hikvisionDeviceConnectionFactoryMock.Object,
                    deviceRepositoryMock.Object,
                    deviceInfoRepositoryMock.Object,
                    timestampClientMock.Object,
                    employeeClientMock.Object);

                await uploadTimestampsInvocable.Invoke();
            }

            [Theory]
            [MemberData(nameof(Events))]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Calls_AddEmployeeFromDeviceTimestampAsync_Exactly_As_Many_Events_Are_Read_From_The_Device_Async(List<AcsEventInfo> acsEventInfos)
            {
                var deviceRepositoryMock = new Mock<IRepository<Device, ApplicationDbContext>>();

                var device = new Device
                {
                    Id = Guid.NewGuid(),
                    Name = Fixtures.TestEnvironment.Device_00.Name,
                };

                var devices = new List<Device>
                              {
                                  device,
                              };

                deviceRepositoryMock.Setup(repository => repository.All())
                    .Returns(new EnumerableQuery<Device>(devices));

                var deviceInfoRepositoryMock = new Mock<IDeviceInfoRepository>();
                var deviceInfo = new DeviceInfo
                {
                    Name = device.Name,
                    Username = Fixtures.TestEnvironment.Device_00.Username,
                    Password = Fixtures.TestEnvironment.Device_00.Password,
                    IpAddress = Fixtures.TestEnvironment.Device_00.IpAddress,
                };

                deviceInfoRepositoryMock.Setup(repository => repository.Devices).Returns(
                    () => new List<DeviceInfo>
                          {
                              deviceInfo,
                          });

                var hikvisionDeviceConnectionFactoryMock = new Mock<IHikvisionDeviceConnectionFactory>();
                var hikvisionDeviceConnection = new Mock<IHikvisionDeviceConnection>();
                hikvisionDeviceConnectionFactoryMock.Setup(
                    factory => factory.Create(
                        deviceInfo.Url,
                        deviceInfo.Username,
                        deviceInfo.Password)).Returns(hikvisionDeviceConnection.Object);

                var acsEventsClientMock = new Mock<IAcsEventsClient>();
                acsEventsClientMock
                    .Setup(
                        client => client.ListAcsEventsAsync(
                            It.IsAny<DateTime>(),
                            It.IsAny<DateTime>(),
                            AccessControlEventTypes.Event,
                            EventMinorTypes.FingerprintComparePass))
                    .Returns(acsEventInfos.ToAsyncEnumerable);

                hikvisionDeviceConnection.Setup(connection => connection.GetClient<IAcsEventsClient>())
                    .Returns(acsEventsClientMock.Object);

                var timestampClientMock = new Mock<ITimestampClient>();
                var employeeClientMock = new Mock<IEmployeeClient>();

                var uploadTimestampsInvocable = new UploadTimestampsInvocable(
                    NullLogger<UploadTimestampsInvocable>.Instance,
                    hikvisionDeviceConnectionFactoryMock.Object,
                    deviceRepositoryMock.Object,
                    deviceInfoRepositoryMock.Object,
                    timestampClientMock.Object,
                    employeeClientMock.Object);

                await uploadTimestampsInvocable.Invoke();

                timestampClientMock.Verify(client => client.AddEmployeeFromDeviceTimestampAsync(It.IsAny<AddEmployeeFromDeviceTimestampRequest>()), Times.Exactly(acsEventInfos.Count));
            }

            [Theory]
            [MemberData(nameof(Events))]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Saves_The_Device_With_The_Last_Time_Plus_A_Second_In_The_LastTransferredTimestampDateTime_Property_Async(List<AcsEventInfo> acsEventInfos)
            {
                var deviceRepositoryMock = new Mock<IRepository<Device, ApplicationDbContext>>();

                var device = new Device
                {
                    Id = Guid.NewGuid(),
                    Name = Fixtures.TestEnvironment.Device_00.Name,
                };

                var devices = new List<Device>
                              {
                                  device,
                              };

                deviceRepositoryMock.Setup(repository => repository.All())
                    .Returns(new EnumerableQuery<Device>(devices));

                var deviceInfoRepositoryMock = new Mock<IDeviceInfoRepository>();
                var deviceInfo = new DeviceInfo
                {
                    Name = device.Name,
                    Username = Fixtures.TestEnvironment.Device_00.Username,
                    Password = Fixtures.TestEnvironment.Device_00.Password,
                    IpAddress = Fixtures.TestEnvironment.Device_00.IpAddress,
                };

                deviceInfoRepositoryMock.Setup(repository => repository.Devices).Returns(
                    () => new List<DeviceInfo>
                          {
                              deviceInfo,
                          });

                var hikvisionDeviceConnectionFactoryMock = new Mock<IHikvisionDeviceConnectionFactory>();
                var hikvisionDeviceConnection = new Mock<IHikvisionDeviceConnection>();
                hikvisionDeviceConnectionFactoryMock.Setup(
                    factory => factory.Create(
                        deviceInfo.Url,
                        deviceInfo.Username,
                        deviceInfo.Password)).Returns(hikvisionDeviceConnection.Object);

                var acsEventsClientMock = new Mock<IAcsEventsClient>();
                acsEventsClientMock
                    .Setup(
                        client => client.ListAcsEventsAsync(
                            It.IsAny<DateTime>(),
                            It.IsAny<DateTime>(),
                            AccessControlEventTypes.Event,
                            EventMinorTypes.FingerprintComparePass))
                    .Returns(acsEventInfos.ToAsyncEnumerable);

                hikvisionDeviceConnection.Setup(connection => connection.GetClient<IAcsEventsClient>())
                    .Returns(acsEventsClientMock.Object);

                DateTimeOffset? lastTransferredTimestampDateTime = null;
                deviceRepositoryMock.Setup(repository => repository.Update(device)).Callback(
                    (Device d) =>
                    {
                        lastTransferredTimestampDateTime = d.LastTransferredTimestampDateTime;
                    });

                var timestampClientMock = new Mock<ITimestampClient>();
                var employeeClientMock = new Mock<IEmployeeClient>();

                var uploadTimestampsInvocable = new UploadTimestampsInvocable(
                    NullLogger<UploadTimestampsInvocable>.Instance,
                    hikvisionDeviceConnectionFactoryMock.Object,
                    deviceRepositoryMock.Object,
                    deviceInfoRepositoryMock.Object,
                    timestampClientMock.Object,
                    employeeClientMock.Object);

                await uploadTimestampsInvocable.Invoke();

                lastTransferredTimestampDateTime.Should()
                    .Be(acsEventInfos.Select(info => DateTimeOffset.Parse(info.Time).AddSeconds(1)).Last());
            }

            [Theory]
            [MemberData(nameof(Events))]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Calls_AddEmployeeFromDeviceAsync_When_AddEmployeeFromDeviceTimestampAsync_Throw_WebApiException_With_Status404NotFound_Async(List<AcsEventInfo> acsEventInfos)
            {
                var deviceRepositoryMock = new Mock<IRepository<Device, ApplicationDbContext>>();

                var device = new Device
                {
                    Id = Guid.NewGuid(),
                    Name = Fixtures.TestEnvironment.Device_00.Name,
                };

                var devices = new List<Device>
                              {
                                  device,
                              };

                deviceRepositoryMock.Setup(repository => repository.All())
                    .Returns(new EnumerableQuery<Device>(devices));

                var deviceInfoRepositoryMock = new Mock<IDeviceInfoRepository>();
                var deviceInfo = new DeviceInfo
                {
                    Name = device.Name,
                    Username = Fixtures.TestEnvironment.Device_00.Username,
                    Password = Fixtures.TestEnvironment.Device_00.Password,
                    IpAddress = Fixtures.TestEnvironment.Device_00.IpAddress,
                };

                deviceInfoRepositoryMock.Setup(repository => repository.Devices).Returns(
                    () => new List<DeviceInfo>
                          {
                              deviceInfo,
                          });

                var hikvisionDeviceConnectionFactoryMock = new Mock<IHikvisionDeviceConnectionFactory>();
                var hikvisionDeviceConnection = new Mock<IHikvisionDeviceConnection>();
                hikvisionDeviceConnectionFactoryMock.Setup(
                    factory => factory.Create(
                        deviceInfo.Url,
                        deviceInfo.Username,
                        deviceInfo.Password)).Returns(hikvisionDeviceConnection.Object);

                var acsEventsClientMock = new Mock<IAcsEventsClient>();
                acsEventsClientMock
                    .Setup(
                        client => client.ListAcsEventsAsync(
                            It.IsAny<DateTime>(),
                            It.IsAny<DateTime>(),
                            AccessControlEventTypes.Event,
                            EventMinorTypes.FingerprintComparePass))
                    .Returns(acsEventInfos.ToAsyncEnumerable);

                hikvisionDeviceConnection.Setup(connection => connection.GetClient<IAcsEventsClient>())
                    .Returns(acsEventsClientMock.Object);

                var userInfoClientMock = new Mock<IUserInfoClient>();
                userInfoClientMock.Setup(client => client.ListUserAsync(It.IsAny<string>()))
                    .Returns(
                        (string[] s) => new List<UserInfo>
                                        {
                                            new UserInfo
                                            {
                                                EmployeeNo = s.First(),
                                                Name = "Jane Doe",
                                            },
                                        }.ToAsyncEnumerable());

                hikvisionDeviceConnection.Setup(connection => connection.GetClient<IUserInfoClient>())
                    .Returns(userInfoClientMock.Object);

                var idx = 0;
                var timestampClientMock = new Mock<ITimestampClient>();
                timestampClientMock.Setup(
                    client => client.AddEmployeeFromDeviceTimestampAsync(
                        It.IsAny<AddEmployeeFromDeviceTimestampRequest>())).Callback(
                    () =>
                    {
                        idx++;
                        if (idx % 2 != 0)
                        {
                            throw new ApiException(
                                "Error",
                                StatusCodes.Status404NotFound,
                                string.Empty,
                                ImmutableDictionary<string, IEnumerable<string>>.Empty,
                                null);
                        }
                    });

                var employeeClientMock = new Mock<IEmployeeClient>();

                var uploadTimestampsInvocable = new UploadTimestampsInvocable(
                    NullLogger<UploadTimestampsInvocable>.Instance,
                    hikvisionDeviceConnectionFactoryMock.Object,
                    deviceRepositoryMock.Object,
                    deviceInfoRepositoryMock.Object,
                    timestampClientMock.Object,
                    employeeClientMock.Object);

                await uploadTimestampsInvocable.Invoke();

                employeeClientMock.Verify(client => client.AddEmployeeFromDeviceAsync(It.IsAny<AddEmployeeFromDeviceRequest>()), Times.Exactly(acsEventInfos.Count));
            }

            [Theory]
            [MemberData(nameof(Events))]
            [Trait(Traits.Category, Category.Unit)]
            public async Task Success_Even_When_AddEmployeeFromDeviceTimestampAsync_Throw_WebApiException_With_Status409Conflict_Async(List<AcsEventInfo> acsEventInfos)
            {
                var deviceRepositoryMock = new Mock<IRepository<Device, ApplicationDbContext>>();

                var device = new Device
                {
                    Id = Guid.NewGuid(),
                    Name = Fixtures.TestEnvironment.Device_00.Name,
                };

                var devices = new List<Device>
                              {
                                  device,
                              };

                deviceRepositoryMock.Setup(repository => repository.All())
                    .Returns(new EnumerableQuery<Device>(devices));

                var deviceInfoRepositoryMock = new Mock<IDeviceInfoRepository>();
                var deviceInfo = new DeviceInfo
                {
                    Name = device.Name,
                    Username = Fixtures.TestEnvironment.Device_00.Username,
                    Password = Fixtures.TestEnvironment.Device_00.Password,
                    IpAddress = Fixtures.TestEnvironment.Device_00.IpAddress,
                };

                deviceInfoRepositoryMock.Setup(repository => repository.Devices).Returns(
                    () => new List<DeviceInfo>
                          {
                              deviceInfo,
                          });

                var hikvisionDeviceConnectionFactoryMock = new Mock<IHikvisionDeviceConnectionFactory>();
                var hikvisionDeviceConnection = new Mock<IHikvisionDeviceConnection>();
                hikvisionDeviceConnectionFactoryMock.Setup(
                    factory => factory.Create(
                        deviceInfo.Url,
                        deviceInfo.Username,
                        deviceInfo.Password)).Returns(hikvisionDeviceConnection.Object);

                var acsEventsClientMock = new Mock<IAcsEventsClient>();
                acsEventsClientMock
                    .Setup(
                        client => client.ListAcsEventsAsync(
                            It.IsAny<DateTime>(),
                            It.IsAny<DateTime>(),
                            AccessControlEventTypes.Event,
                            EventMinorTypes.FingerprintComparePass))
                    .Returns(acsEventInfos.ToAsyncEnumerable);

                hikvisionDeviceConnection.Setup(connection => connection.GetClient<IAcsEventsClient>())
                    .Returns(acsEventsClientMock.Object);

                var userInfoClientMock = new Mock<IUserInfoClient>();
                userInfoClientMock.Setup(client => client.ListUserAsync(It.IsAny<string>()))
                    .Returns(
                        (string[] s) => new List<UserInfo>
                                        {
                                            new UserInfo
                                            {
                                                EmployeeNo = s.First(),
                                                Name = "Jane Doe",
                                            },
                                        }.ToAsyncEnumerable());

                hikvisionDeviceConnection.Setup(connection => connection.GetClient<IUserInfoClient>())
                    .Returns(userInfoClientMock.Object);

                var idx = 0;
                var timestampClientMock = new Mock<ITimestampClient>();
                timestampClientMock.Setup(
                    client => client.AddEmployeeFromDeviceTimestampAsync(
                        It.IsAny<AddEmployeeFromDeviceTimestampRequest>())).Callback(
                    () =>
                    {
                        throw new ApiException(
                            "Error",
                            StatusCodes.Status409Conflict,
                            string.Empty,
                            ImmutableDictionary<string, IEnumerable<string>>.Empty,
                            null);
                    });

                var employeeClientMock = new Mock<IEmployeeClient>();

                var uploadTimestampsInvocable = new UploadTimestampsInvocable(
                    NullLogger<UploadTimestampsInvocable>.Instance,
                    hikvisionDeviceConnectionFactoryMock.Object,
                    deviceRepositoryMock.Object,
                    deviceInfoRepositoryMock.Object,
                    timestampClientMock.Object,
                    employeeClientMock.Object);

                await uploadTimestampsInvocable.Invoke();
            }
        }
    }
}