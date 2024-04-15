// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Coravel;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

using Serilog;
using Serilog.Events;

using StoneAssemblies.EntityFrameworkCore.Services;
using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;
using StoneAssemblies.Hikvision.Extensions;

using Tempore.Agent.Invocables;
using Tempore.Agent.Services;
using Tempore.Agent.Services.Interfaces;
using Tempore.Client.Extensions;
using Tempore.Configuration.Extensions;

var logger = Log.Logger = new LoggerConfiguration()
                 .MinimumLevel
                 .Override("Microsoft", LogEventLevel.Information)
                 .Enrich.FromLogContext()
                 .WriteTo.Console()
                 .CreateBootstrapLogger();

var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

var webApplicationOptions = new WebApplicationOptions
{
    ContentRootPath = currentDirectory,
    Args = args,
};
var builder = WebApplication.CreateBuilder(webApplicationOptions);

builder.Configuration
    .AddEnvironmentVariables()
    .AddEnvironmentVariablesWithPrefixAndSectionSeparator("TMP", "_")
    .AddJsonFiles(builder.Environment.EnvironmentName);

builder.Host
    .UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console();
    })
    .UseWindowsService(options =>
    {
        // TODO: Allow register multiple agents in the same machine with different service name?
        options.ServiceName = "Tempore Agent";
    });

builder.Services.AddHikvision();
logger.Information("Registering Coravel's services");
builder.Services.AddScheduler();
builder.Services.AddScoped<UploadTimestampsInvocable>();

logger.Information("Registering application's services");
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddSingleton<IDeviceInfoRepository, ConfigurationBasedDeviceInfoRepository>();
builder.Services.AddScoped<AccessTokenDelegatingHandler>();
builder.Services.AddSingleton<ITokenResolver, TokenResolver>();

builder.Services.AddTemporeHttpClients(
    client => client.BaseAddress = new Uri(builder.Configuration["Server"]),
    clientBuilder => clientBuilder.AddHttpMessageHandler<AccessTokenDelegatingHandler>());

builder.Services.AddSingleton<IHubConnectionBuilder, HubConnectionBuilder>();
builder.Services.AddHostedService<AgentInitializationService>();

var app = builder.Build();

logger.Information("Starting host");

logger.Information("Migrating database");
using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await applicationDbContext.Database.MigrateAsync();

app.Services.UseScheduler(
        scheduler =>
        {
            scheduler
                .Schedule<UploadTimestampsInvocable>()
                .EveryTenSeconds()
                .PreventOverlapping(nameof(UploadTimestampsInvocable))
                .RunOnceAtStart();
        })
    .OnError(e =>
    {
        logger.Error(e, "Error scheduling task");
    });

try
{
    logger.Information("Built host, running now...");

    await app.RunAsync("http://*:0");
}
catch (Exception ex)
{
    logger.Fatal(ex, "Host terminated unexpectedly");

    Log.CloseAndFlush();
}

public partial class Program
{
}