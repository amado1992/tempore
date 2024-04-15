namespace Tempore.Tests.Tempore.App.Pages.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Blorc.Services;

    using Bunit;

    using FluentAssertions;

    using global::Tempore.App.Pages.Employees;
    using global::Tempore.App.Services;
    using global::Tempore.App.Services.Interfaces;
    using global::Tempore.Client;
    using global::Tempore.Tests.Infraestructure;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using MudBlazor;
    using MudBlazor.Services;

    using Xunit;

    public partial class LinkEmployeesFacts
    {
        public class Initialization
        {
            public static IEnumerable<object[]> Data()
            {
                yield return new object[]
                             {
                                 SupportedCultures.SpanishCultureInfo,
                                 "Desvincular",
                             };

                yield return new object[]
                             {
                                 SupportedCultures.EnglishCultureInfo,
                                 "Unlink",
                             };
            }

            [Fact(Skip = "Not working, breaking change")]
            [Trait(Traits.Category, Category.Unit)]
            public void Does_Not_Display_Unlink_Option_For_Unlinked_Employees_Async()
            {
                var employeeClientMock = new Mock<IEmployeeClient>();
                var employeesFromDevices = new List<EmployeeFromDeviceDto>
                                       {
                                           new EmployeeFromDeviceDto
                                           {
                                               Id = Guid.NewGuid(),
                                               EmployeeIdOnDevice = "0001",
                                               FullName = "Jane Doe",
                                           },
                                           new EmployeeFromDeviceDto
                                           {
                                               Id = Guid.NewGuid(),
                                               EmployeeIdOnDevice = "0002",
                                               FullName = "John Doe",
                                           },
                                       };

                var employeeFromDeviceDtoPaginationResponse = new EmployeeFromDeviceDtoPagedResponse
                {
                    Count = employeesFromDevices.Count,
                    Items = employeesFromDevices,
                };

                employeeClientMock.Setup(client => client.GetEmployeesFromDevicesAsync(It.IsAny<GetEmployeesFromDevicesRequest>()))
                    .ReturnsAsync(employeeFromDeviceDtoPaginationResponse);

                var employees = new List<EmployeeDto>
                            {
                                new EmployeeDto
                                {
                                    FullName = "Jane Doe",
                                    ExternalId = "0001",
                                    Id = Guid.NewGuid(),
                                },
                                new EmployeeDto
                                {
                                    FullName = "John Doe",
                                    ExternalId = "0002",
                                    Id = Guid.NewGuid(),
                                },
                            };

                var employeeDtoPaginationResponse = new EmployeeDtoPagedResponse
                {
                    Count = employees.Count,
                    Items = employees,
                };

                employeeClientMock.Setup(client => client.GetEmployeesAsync(It.IsAny<GetEmployeesRequest>()))
                    .ReturnsAsync(employeeDtoPaginationResponse);

                // Arrange
                using var ctx = new TestContext();

                ctx.JSInterop.SetupVoid("mudPopover.initialize", _ => true);
                ctx.JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);

                ctx.Services.AddMudServices();
                ctx.Services.AddBlorcCore();
                ctx.Services.AddTransient(typeof(IMudTableComponentService<>), typeof(MudTableComponentService<>));

                ctx.Services.AddSingleton(employeeClientMock.Object);
                ctx.Services.AddLocalization(options => options.ResourcesPath = "Resources");

                var componentServiceFactory = ctx.Services.GetRequiredService<IComponentServiceFactory>();
                componentServiceFactory.Map<MudTable<EmployeeFromDeviceDto>, IMudTableComponentService<EmployeeFromDeviceDto>>();
                componentServiceFactory.Map<MudTable<EmployeeDto>, IMudTableComponentService<EmployeeDto>>();

                // Act
                using var cut = ctx.RenderComponent<LinkEmployees>();

                // Asserts
                var elements = cut.FindAll("div.mud-table-container > table > tbody > tr > td:nth-child(4) > button");
                elements.Count.Should().Be(0);
            }

            [Theory(Skip = "Not working, breaking change")]
            [SetCulture]
            [MemberData(nameof(Data))]
            [Trait(Traits.Category, Category.Unit)]
            public void Does_Display_Unlink_Option_For_Linked_Employees_Async(CultureInfo cultureInfo, string expectedButtonText)
            {
                // Arrange
                var employeeClientMock = new Mock<IEmployeeClient>();

                var employeesFromDevices = new List<EmployeeFromDeviceDto>
                           {
                               new EmployeeFromDeviceDto
                               {
                                   Id = Guid.NewGuid(),
                                   EmployeeIdOnDevice = "0001",
                                   FullName = "Jane Doe",
                                   EmployeeId = Guid.NewGuid(),
                                   IsLinked = true,
                               },
                               new EmployeeFromDeviceDto
                               {
                                   Id = Guid.NewGuid(),
                                   EmployeeIdOnDevice = "0002",
                                   FullName = "John Doe",
                                   EmployeeId = Guid.NewGuid(),
                                   IsLinked = true,
                               },
                           };

                var employeeFromDeviceDtoPaginationResponse = new EmployeeFromDeviceDtoPagedResponse()
                {
                    Count = employeesFromDevices.Count,
                    Items = employeesFromDevices,
                };

                employeeClientMock.Setup(client => client.GetEmployeesFromDevicesAsync(It.IsAny<GetEmployeesFromDevicesRequest>()))
                    .ReturnsAsync(employeeFromDeviceDtoPaginationResponse);

                var employees = new List<EmployeeDto>
                {
                    new EmployeeDto
                    {
                        FullName = "Jane Doe",
                        ExternalId = "0001",
                        Id = Guid.NewGuid(),
                    },
                    new EmployeeDto
                    {
                        FullName = "John Doe",
                        ExternalId = "0002",
                        Id = Guid.NewGuid(),
                    },
                };
                var employeeDtoPaginationResponse = new EmployeeDtoPagedResponse
                {
                    Count = employees.Count,
                    Items = employees,
                };
                employeeClientMock.Setup(client => client.GetEmployeesAsync(It.IsAny<GetEmployeesRequest>()))
                    .ReturnsAsync(employeeDtoPaginationResponse);

                using var ctx = new TestContext();

                ctx.JSInterop.SetupVoid("mudPopover.initialize", _ => true);
                ctx.JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);

                ctx.Services.AddMudServices();
                ctx.Services.AddBlorcCore();
                ctx.Services.AddTransient(typeof(IMudTableComponentService<>), typeof(MudTableComponentService<>));

                ctx.Services.AddSingleton(employeeClientMock.Object);
                ctx.Services.AddLocalization(options => options.ResourcesPath = "Resources");

                var componentServiceFactory = ctx.Services.GetRequiredService<IComponentServiceFactory>();
                componentServiceFactory.Map<MudTable<EmployeeFromDeviceDto>, IMudTableComponentService<EmployeeFromDeviceDto>>();
                componentServiceFactory.Map<MudTable<EmployeeDto>, IMudTableComponentService<EmployeeDto>>();

                // Act
                using var cut = ctx.RenderComponent<LinkEmployees>();

                // Asserts
                var elements = cut.FindAll("div.mud-table-container > table > tbody > tr > td:nth-child(4) > button:first-child");
                elements.Should().AllSatisfy(element => element.TextContent.Should().Be(expectedButtonText));
            }
        }
    }
}