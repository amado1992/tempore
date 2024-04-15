namespace Tempore.Tests.Fixtures.Postgres;

extern alias TemporeAgent;
extern alias TemporeServer;

using System.Collections.Generic;
using System.Threading.Tasks;

using global::Tempore.Configuration.Extensions;
using global::Tempore.Integration.Tests.Fixtures;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TemporeAgent::Tempore.Agent.Services.Interfaces;

using TemporeServer::Tempore.Server.Extensions;

public abstract class TemporeAgentApplicationFactoryBase<TProgram> : TemporeBackEndWebApplicationFactoryBase<TProgram>
    where TProgram : class
{
    private readonly Dictionary<string, string> configurationData;

    protected TemporeAgentApplicationFactoryBase()
    {
        this.configurationData = new Dictionary<string, string>
        {
            // Service Discovery
            ["APP_INSTANCE"] = "0",
            ["TMP_SERVER"] = "http://localhost",

            // Keycloak
            ["TMP_IDENTITYSERVER_AUTHORITY"] = TestEnvironment.Keycloak.Autority,
        };

        foreach (var environmentVariable in this.configurationData)
        {
            this.EnvironmentVariableServiceMock.Setup(service => service.GetValue(environmentVariable.Key)).Returns(environmentVariable.Value);
        }
    }

    public virtual Task InitializeAsync()
    {
        using var client = this.CreateClient();

        return Task.CompletedTask;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(TemporeEnvironments.IntegrationTest);

        builder.ConfigureHostConfiguration(configurationBuilder =>
        {
            this.UpdateConfiguration(this.configurationData);

            configurationBuilder.AddInMemoryCollection(this.configurationData);
            configurationBuilder.AddInMemoryWithPrefixSectionSeparator("TMP", "_", this.configurationData);

            this.ConfigureHostConfiguration(configurationBuilder);
        });

        builder.ConfigureServices(
            collection =>
            {
                collection.AddSingleton(this.EnvironmentVariableServiceMock.Object);
                collection.AddSingleton<ITokenResolver, IntegrationTestsTokenResolver>();

                this.ConfigureServices(collection);
            });

        return base.CreateHost(builder);
    }

    protected virtual void UpdateConfiguration(Dictionary<string, string> configurationData)
    {
    }

    protected virtual void ConfigureServices(IServiceCollection collection)
    {
    }

    protected virtual void ConfigureHostConfiguration(IConfigurationBuilder configurationBuilder)
    {
    }
}