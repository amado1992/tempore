// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;

using Blazored.LocalStorage;

using Blorc.OpenIdConnect;
using Blorc.Services;

using FluentValidation;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;

using MudBlazor;
using MudBlazor.Services;

using Tempore.App;
using Tempore.App.Extensions;
using Tempore.App.Services;
using Tempore.App.Services.Interfaces;
using Tempore.App.Validators.ViewModels.Dialogs;
using Tempore.App.ViewModels.Dialogs.Shifts;
using Tempore.Authorization.Policies;
using Tempore.Client;
using Tempore.Client.Extensions;
using Tempore.Client.Services.Interfaces;

using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddLoadingBarService();

builder.Services.AddBlorcCore();
builder.Services.AddBlorcOpenIdConnect(
    options =>
    {
        builder.Configuration.Bind("IdentityServer", options);
    });

builder.Services.AddAuthorizationCore(
    options =>
    {
        foreach (var policyInfo in Policies.All())
        {
            options.AddPolicy(policyInfo.Name, policyBuilder => policyBuilder.AddRequirements(policyInfo.AuthorizationRequirement));
        }
    });

builder.Services.AddMudServices(
    configuration =>
    {
        configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
        configuration.SnackbarConfiguration.PreventDuplicates = false;
        configuration.SnackbarConfiguration.NewestOnTop = false;
        configuration.SnackbarConfiguration.ShowCloseIcon = true;
        configuration.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        configuration.SnackbarConfiguration.HideTransitionDuration = 500;
        configuration.SnackbarConfiguration.ShowTransitionDuration = 500;
    });

// Component Services
builder.Services.AddTransient(typeof(IMudTableComponentService<>), typeof(MudTableComponentService<>));
builder.Services.AddTransient<IMudDialogInstanceComponentService, MudDialogInstanceComponentService>();
builder.Services.AddTransient<IMudFormComponentService, MudFormComponentService>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
    };

    return httpClient;
});

builder.Services.AddTemporeHttpClients(
    (sp, httpClient) =>
    {
        httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);

        httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);

        httpClient.EnableIntercept(sp);
    },
    clientBuilder => clientBuilder.AddAccessToken());

builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();

builder.Services.AddSingleton<IHubConnectionBuilder, HubConnectionBuilder>();

builder.Services.AddSingleton<NotificationCenter>();
builder.Services.AddSingleton<INotificationReceiver>(provider => provider.GetRequiredService<NotificationCenter>());
builder.Services.AddSingleton<INotificationCenter>(provider => provider.GetRequiredService<NotificationCenter>());

// Validators
builder.Services.AddScoped<IValidator<AssignEmployeesToScheduledShiftConfirmationViewModel>, AssignEmployeesToScheduledShiftConfirmationViewModelValidator>();

builder.UseLoadingBar();

var host = builder.Build();

await host.ConfigureDocumentAsync(
    async documentService =>
    {
        await documentService.InjectBlorcCoreJsAsync();
        await documentService.InjectOpenIdConnectAsync();
    });

// TODO: Register this in other place.
var componentServiceFactory = host.Services.GetRequiredService<IComponentServiceFactory>();

componentServiceFactory.Map<MudTable<AgentDto>, IMudTableComponentService<AgentDto>>();
componentServiceFactory.Map<MudTable<DeviceDto>, IMudTableComponentService<DeviceDto>>();
componentServiceFactory.Map<MudTable<EmployeeFromDeviceDto>, IMudTableComponentService<EmployeeFromDeviceDto>>();
componentServiceFactory.Map<MudTable<EmployeeDto>, IMudTableComponentService<EmployeeDto>>();
componentServiceFactory.Map<MudTable<DataFileDto>, IMudTableComponentService<DataFileDto>>();
componentServiceFactory.Map<MudTable<IDictionary<string, string>>, IMudTableComponentService<IDictionary<string, string>>>();
componentServiceFactory.Map<MudTable<ShiftDto>, IMudTableComponentService<ShiftDto>>();
componentServiceFactory.Map<MudTable<ScheduledShiftOverviewDto>, IMudTableComponentService<ScheduledShiftOverviewDto>>();
componentServiceFactory.Map<MudTable<ScheduledShiftEmployeeDto>, IMudTableComponentService<ScheduledShiftEmployeeDto>>();
componentServiceFactory.Map<MudTable<DayDto>, IMudTableComponentService<DayDto>>();
componentServiceFactory.Map<MudTable<WorkforceMetricCollectionDto>, IMudTableComponentService<WorkforceMetricCollectionDto>>();
componentServiceFactory.Map<MudTable<IDictionary<string, object>>, IMudTableComponentService<IDictionary<string, object>>>();

componentServiceFactory.Map<MudDialogInstance, IMudDialogInstanceComponentService>();

componentServiceFactory.Map<MudForm, IMudFormComponentService>();

await host.SetDefaultCulture();
await host.RunAsync();