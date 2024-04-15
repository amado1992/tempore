namespace Tempore.Tests.Fixtures.Postgres;

using Xunit;

[CollectionDefinition(nameof(DockerCollection))]
public class DockerCollection :
    ICollectionFixture<DockerEnvironment>,
    ICollectionFixture<TemporeServerWebApplicationFactory>,
    ICollectionFixture<TemporeAgentWebApplicationFactory>,
    ICollectionFixture<TestEnvironmentInitializer>
{
}