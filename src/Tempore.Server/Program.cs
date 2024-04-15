// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Coravel;
using Coravel.Queuing.Broadcast;

using FluentValidation;
using FluentValidation.AspNetCore;

using Hellang.Middleware.ProblemDetails;

using Mapster;

using MicroElements.Swashbuckle.FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Serilog;
using Serilog.Events;

using StoneAssemblies.EntityFrameworkCore.Services;
using StoneAssemblies.EntityFrameworkCore.Services.Interfaces;

using Tempore.Authorization.Policies;
using Tempore.Configuration.Extensions;
using Tempore.HealthChecks.Services;
using Tempore.HealthChecks.Services.Interfaces;
using Tempore.Hosting.Extensions;
using Tempore.Hosting.Services;
using Tempore.Hosting.Services.Interfaces;
using Tempore.Infrastructure.Keycloak.Services.Interfaces;
using Tempore.Keycloak.Services;
using Tempore.Processing.PayDay;
using Tempore.Processing.PayDay.Extensions;
using Tempore.Processing.PayDay.Services;
using Tempore.Processing.Services;
using Tempore.Processing.Services.Interfaces;
using Tempore.Server.Exceptions;
using Tempore.Server.Extensions;
using Tempore.Server.Handlers.Employees;
using Tempore.Server.Hubs;
using Tempore.Server.Invokables.Agents;
using Tempore.Server.Invokables.Employees;
using Tempore.Server.Invokables.FileProcessing;
using Tempore.Server.Invokables.ScheduledDay;
using Tempore.Server.Invokables.WorkforceMetrics;
using Tempore.Server.Listeners;
using Tempore.Server.Services;
using Tempore.Server.Services.Interfaces;
using Tempore.Storage;
using Tempore.Storage.Entities;
using Tempore.Storage.PostgreSQL;
using Tempore.Validation.Filters;

var logger = Log.Logger = new LoggerConfiguration()
                 .MinimumLevel
                 .Override("Microsoft", LogEventLevel.Information)
                 .Enrich.FromLogContext()
                 .WriteTo.Console()
                 .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Configuration
    .AddEnvironmentVariables()
    .AddEnvironmentVariablesWithPrefixAndSectionSeparator("TMP", "_")
    .AddJsonFiles(builder.Environment.EnvironmentName);

builder.Host.UseSerilog(
    (context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console();
    });

builder.Services
    .AddHealthChecks()
    .AddCheck<ApplicationInitializationHealthCheck>("app");

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(
    options =>
    {
        var supportedCultures = new[] { "en", "es" };
        options.SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        options.RequestCultureProviders.Insert(
            0,
            new CustomRequestCultureProvider(
                context =>
                {
                    var currentLanguage = context.Request.GetTypedHeaders().AcceptLanguage.FirstOrDefault()?.Value.Value ?? supportedCultures.First();
                    var providerCultureResult = new ProviderCultureResult(currentLanguage, currentLanguage);
                    return Task.FromResult(providerCultureResult)!;
                }));
    });

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, PreferredUsernameBasedUserIdProvider>();

builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssemblyContaining<Program>();
});

// TODO: Verify if it is working?
builder.Services.AddTransient(typeof(ProcessCompletedNotificationHandler<>));

logger.Information("Registering database access services");

// TODO: Remove direct dependency to PostgreSQLApplicationDbContext (load as plugin)
builder.Services.AddDbContext<ApplicationDbContext, PostgreSQLApplicationDbContext>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

// Service discovery.
builder.Services.AddSingleton<IEnvironmentVariableService, EnvironmentVariableService>();
builder.Services.AddServiceDiscovery();

if (!builder.Environment.IsSwaggerGen())
{
    logger.Information("Registering initialization services");
    builder.Services.AddSingleton<IApplicationInitializationService, ApplicationInitializationService>();
    builder.Services.AddHostedService(provider => provider.GetRequiredService<IApplicationInitializationService>());

    builder.Services.AddSingleton<IInitializationService, ApplicationInitializationService>();
    builder.Services.AddSingleton<IDatabaseInitializationService, DatabaseInitializationService>();
    builder.Services.AddSingleton<IKeycloakInitializationService, KeycloakInitializationService>();
    builder.Services.AddSingleton<IKeycloakClientFactory, KeycloakClientFactory>();

    //// TODO: Remove this from here and register it the plugin.
    builder.Services.AddPayDay();
}

logger.Information("Registering health check services");
builder.Services.AddSingleton<IHttpHealthCheckServiceFactory, HttpHealthCheckServiceFactory>();
builder.Services.AddSingleton<IDatabaseHealthCheckServiceFactory, DatabaseHealthCheckServiceFactory>();

logger.Information("Registering application services");

builder.Services.AddScoped<IInvocationContextAccessor, InvocationContextAccessor>();
builder.Services.AddSingleton(typeof(IHubLifetimeManager<>), typeof(Tempore.Server.Services.HubLifetimeManager<>));
builder.Services.AddSingleton<IFileProcessingServiceFactory, FileProcessingServiceFactory>();
builder.Services.AddSingleton<IWorkforceMetricCollectionSchemaProviderFactory, WorkforceMetricCollectionSchemaProviderFactory>();
builder.Services.AddSingleton<IFileContentWriterFactory, FileContentWriterFactory>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddSingleton<IQueueService, QueueService>();
builder.Services.AddSingleton(typeof(IStringLocalizerService<>), typeof(StringLocalizerService<>));

builder.Services.AddSingleton<IDaySchedulerService, DaySchedulerService>();

// TODO: Read from configuration.
logger.Information("Configuring authentication");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options =>
            {
                options.Authority = builder.Configuration.GetSection("IdentityServer")["Authority"];
                options.RequireHttpsMetadata = false;
                options.Audience = "tempore-api";

                options.Events = new JwtBearerEvents
                {
                    // TODO: Review this later.
                    OnMessageReceived = context =>
                        {
                            // if (DateTime.UtcNow > context.Properties.ExpiresUtc)
                            // {
                            //    context.Fail("Session Expired");
                            // }
                            return Task.CompletedTask;
                        },
                };
            });

logger.Information("Configuring authorization");
builder.Services.AddAuthorization(
    options =>
        {
            logger.Information("Adding authorization policies");
            foreach (var policyInfo in Policies.All())
            {
                logger.Information("Added authorization policy '{PolicyName}'", policyInfo.Name);

                options.AddPolicy(
                    policyInfo.Name,
                    policyBuilder =>
                    {
                        policyBuilder.Requirements.Add(policyInfo.AuthorizationRequirement);
                    });
            }
        });

logger.Information("Registering validation");

builder.Services.AddScoped<ValidateAsyncActionFilter>();
builder.Services
    .AddValidatorsFromAssembly(typeof(Program).Assembly)
    .AddFluentValidationClientsideAdapters();

logger.Information("Registering Coravel's services");

if (!builder.Environment.IsIntegrationTest())
{
    builder.Services.AddScheduler();
    builder.Services.AddTransient<AgentsSynchronizationInvokable>();
}

builder.Services.AddQueue();
builder.Services.AddEvents();

logger.Information("Registering listeners");

builder.Services.AddTransient<TaskStartedListener>();
builder.Services.AddTransient<TaskCompletedListener>();

logger.Information("Registering invocables");
builder.Services.AddScoped<LinkEmployeesInvokable>();
builder.Services.AddScoped<ProcessFileInvokable>();
builder.Services.AddScoped<TimestampScheduledDayAssociatorInvokable>();
builder.Services.AddScoped<DaySchedulerInvokable>();
builder.Services.AddScoped<ComputeWorkforceMetricsInvokable>();

logger.Information("Configuring versioning, swagger and serialization");
builder.Services.AddApiVersioning();

builder.Services
    .AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });

builder.Services.AddRazorPages();
builder.Services.AddVersionedApiExplorer(o =>
{
    o.GroupNameFormat = "'v'VVV";
    o.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
    options =>
    {
        options.CustomOperationIds(description => $"{description.ActionDescriptor.RouteValues["controller"]}_{description.ActionDescriptor.RouteValues["action"]}");

        options.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Version = "v1",
                Title = "Tempore API",
                Description = "A Web API for Tempore",
                TermsOfService = new Uri("https://tempore.io/terms"),
                Contact = new OpenApiContact { Name = "Tempore Contact", Url = new Uri("https://tempore.io/contact") },
                License = new OpenApiLicense { Name = "Tempore License", Url = new Uri("https://tempore.io/license") },
            });

        options.MapType<TimeSpan>(() => new OpenApiSchema { Type = "string", Format = "time-span" });

        // TODO: Review this?
        options.UseAllOfToExtendReferenceSchemas();
    });

builder.Services.AddFluentValidationRulesToSwagger();
builder.Services.AddSwaggerGenNewtonsoftSupport();

logger.Information("Configuring problem details");
builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (ctx, env) => builder.Environment.IsDevelopment() || builder.Environment.IsIntegrationTest();
    options.ShouldLogUnhandledException = (context, exception, problemDetails) => true;

    options.MapToStatusCode<NotFoundException>(StatusCodes.Status404NotFound);
    options.MapToStatusCode<BadRequestException>(StatusCodes.Status400BadRequest);
    options.MapToStatusCode<ConflictException>(StatusCodes.Status409Conflict);
});

var app = builder.Build();

logger.Information("Configure the HTTP request pipeline");

app.MapHealthChecks("/health");

app.UseRequestLocalization();

app.UseProblemDetails();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

logger.Information("Configuring endpoints");

app.UseEndpoints(
    endpoints =>
        {
            // Also remove this.
            endpoints.MapHub<AgentHub>("/agentHub");
            endpoints.MapHub<NotificationHub>("/notificationHub");

            endpoints.MapControllers();

            endpoints.MapFallbackToFile("index.html");
        });

if (!app.Environment.IsSwaggerGen())
{
    logger.Information("Configuring Coravel's services");

    if (!app.Environment.IsIntegrationTest())
    {
        app.Services
            .UseScheduler(
                scheduler =>
                {
                    scheduler.Schedule<AgentsSynchronizationInvokable>()
                        .EveryTenSeconds()
                        .PreventOverlapping(nameof(AgentsSynchronizationInvokable));

                    scheduler.Schedule<TimestampScheduledDayAssociatorInvokable>()
                        .EveryTenSeconds()
                        .PreventOverlapping(nameof(TimestampScheduledDayAssociatorInvokable));
                })
            .OnError(
                e =>
                {
                    logger.Information(e, "Error queuing task");
                });
    }

    app.Services
        .ConfigureQueue()
        .OnError(e =>
        {
            logger.Information(e, "Error queuing task");
        });

    var eventRegistration = app.Services.ConfigureEvents();

    eventRegistration
        .Register<QueueTaskStarted>()
        .Subscribe<TaskStartedListener>();

    eventRegistration
        .Register<QueueTaskCompleted>()
        .Subscribe<TaskCompletedListener>();

    logger.Information("Registering file content writer services ...");
    var fileContentWriterFactory = app.Services.GetRequiredService<IFileContentWriterFactory>();
    fileContentWriterFactory.Register<CsvFileContentWriter>(FileFormat.CSV);

    logger.Information("Registering file processing services ...");
    var fileProcessingServiceFactory = app.Services.GetRequiredService<IFileProcessingServiceFactory>();

    // TODO: Load as plugin later.
    fileProcessingServiceFactory.Register<PayDayFileProcessingService>(FileType.PayDay);
}

try
{
    TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

    logger.Information("Built host, running now...");
    if (!app.Environment.IsIntegrationTest() && !app.Environment.IsSwaggerGen())
    {
        await app.SyncBlazorHostedConfigurationAsync();
    }

    await app.RunAsync();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Host terminated unexpectedly");

    Log.CloseAndFlush();
}

public partial class Program
{
}