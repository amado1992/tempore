namespace Tempore.Tests.Fixtures.Postgres
{
    using System.Collections.Generic;

    using global::TestEnvironment.Docker;

    /// <summary>
    /// The docker environment.
    /// </summary>
    public class DockerEnvironment : TemporeDockerEnvironmentBase
    {
        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="dockerEnvironmentBuilder">
        /// The docker environment builder.
        /// </param>
        protected override void Configure(DockerEnvironmentBuilder dockerEnvironmentBuilder)
        {
            dockerEnvironmentBuilder.AddContainer(
                p => p with
                {
                    Name = TestEnvironment.Components.Storage.Name,
                    ImageName = TestEnvironment.Postgres.ImageName,
                    Tag = TestEnvironment.Postgres.Tag,
                    Ports = new Dictionary<ushort, ushort>
                                 {
                                     { TestEnvironment.Postgres.ContainerPort, TestEnvironment.Postgres.Port },
                                 },
                    ExposedPorts = new List<ushort> { TestEnvironment.Postgres.ContainerPort },
                    EnvironmentVariables = new Dictionary<string, string>
                                                {
                                                    { "POSTGRES_USER", TestEnvironment.Postgres.Username },
                                                    { "POSTGRES_PASSWORD", TestEnvironment.Postgres.Password },
                                                    { "POSTGRES_DB", TestEnvironment.Components.Storage.DatabaseName },
                                                },
                });

            dockerEnvironmentBuilder.AddContainer(
                p => p with
                {
                    Name = TestEnvironment.Components.Keycloak.Name,
                    ImageName = TestEnvironment.Keycloak.ImageName,
                    Tag = TestEnvironment.Keycloak.Tag,
                    Ports =
                         new Dictionary<ushort, ushort>
                         {
                             { TestEnvironment.Keycloak.ContainerPort, TestEnvironment.Keycloak.Port },
                         },
                    Entrypoint = new List<string> { "/opt/keycloak/bin/kc.sh", "start-dev" },
                    ExposedPorts = new List<ushort> { TestEnvironment.Keycloak.ContainerPort },
                    EnvironmentVariables = new Dictionary<string, string>
                                                {
                                                    { "DEV_TEST", "true" },
                                                    { "VERBOSE_LOGGING", "false" },
                                                    { "DISABLE_TRUST_STORE", "true" },
                                                    { "KEYCLOAK_ADMIN", TestEnvironment.Keycloak.Username },
                                                    { "KEYCLOAK_ADMIN_PASSWORD", TestEnvironment.Keycloak.Password },
                                                    { "KC_HTTP_ENABLED", "true" },
                                                    { "KC_HTTP_RELATIVE_PATH", "/auth" },
                                                    { "KC_PROXY", "passthrough" },
                                                    { "JAVA_OPTS_APPEND", "-XX:+UseContainerSupport -XX:MaxRAMPercentage=50.0 -Djava.awt.headless=true -Djgroups.dns.query=rancher-auth-headless" },
                                                },
                });
        }
    }
}