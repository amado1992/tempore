namespace Tempore.Tests.Fixtures
{
    extern alias TemporeServer;

    using global::Tempore.Client;

    /// <summary>
    /// The test environment.
    /// </summary>
    public static class TestEnvironment
    {
        /// <summary>
        /// The components.
        /// </summary>
        public static class Components
        {
            /// <summary>
            /// The storage.
            /// </summary>
            public static class Storage
            {
                /// <summary>
                /// The name.
                /// </summary>
                public const string Name = "tempore-storage";

                /// <summary>
                /// The keycloak database name.
                /// </summary>
                public const string DatabaseName = "tempore";

                /// <summary>
                /// The application connection string.
                /// </summary>
                public static readonly string PostgresConnectionString = $"User ID={Postgres.Username};Password={Postgres.Password};Host={Postgres.Host};Port={Postgres.Port};Database={DatabaseName};Pooling=true";
            }

            /// <summary>
            /// The auth storage.
            /// </summary>
            public static class AuthStorage
            {
                /// <summary>
                /// The name.
                /// </summary>
                public const string Name = "tempore-auth-storage";

                /// <summary>
                /// The tempore database name.
                /// </summary>
                public const string DatabaseName = "keycloak";
            }

            /// <summary>
            /// The keycloak.
            /// </summary>
            public static class Keycloak
            {
                /// <summary>
                /// The name.
                /// </summary>
                public const string Name = "tempore-keycloak";
            }
        }

        /// <summary>
        /// The agent.
        /// </summary>
        public static class Agent
        {
            /// <summary>
            /// The name.
            /// </summary>
            public const string Name = "Main Agent";
        }

        /// <summary>
        /// The device 00.
        /// </summary>
        public static class Device_00
        {
            /// <summary>
            /// The device name.
            /// </summary>
            public const string DeviceName = "T&A Access Controller";

            /// <summary>
            /// The mac address.
            /// </summary>
            public const string MacAddress = "00:11:22:AA:BB:CC";

            /// <summary>
            /// The model.
            /// </summary>
            public const string Model = "DS-K1T804AEF";

            /// <summary>
            /// The serial number.
            /// </summary>
            public const string SerialNumber = "K29703787";

            /// <summary>
            /// The name.
            /// </summary>
            public const string Name = "Main Door";

            /// <summary>
            /// The new name 01.
            /// </summary>
            public const string NewName01 = "Main Door New 01";

            /// <summary>
            /// The new name 02.
            /// </summary>
            public const string NewName02 = "Main Door New 02";

            /// <summary>
            /// The ip address.
            /// </summary>
            public const string IpAddress = "192.168.1.6";

            /// <summary>
            /// The username.
            /// </summary>
            public const string Username = "admin";

            /// <summary>
            /// The password.
            /// </summary>
            public const string Password = "h1kv1s1on123";

            /// <summary>
            /// The instance.
            /// </summary>
            public static readonly DeviceDto Instance = new DeviceDto
            {
                Name = Name,
                DeviceName = DeviceName,
                MacAddress = MacAddress,
                Model = Model,
                SerialNumber = SerialNumber,
            };

            /// <summary>
            /// The registration instance.
            /// </summary>
            public static readonly DeviceRegistrationDto RegistrationInstance = new DeviceRegistrationDto
            {
                Name = Name,
                DeviceName = DeviceName,
                MacAddress = MacAddress,
                Model = Model,
                SerialNumber = SerialNumber,
            };
        }

        /// <summary>
        /// The device 01.
        /// </summary>
        public static class Device_01
        {
            /// <summary>
            /// The device name.
            /// </summary>
            public const string DeviceName = "T&A Access Controller";

            /// <summary>
            /// The mac address.
            /// </summary>
            public const string MacAddress = "00:1A:2B:3C:4D:5E";

            /// <summary>
            /// The model.
            /// </summary>
            public const string Model = "DS-K1T804AEF";

            /// <summary>
            /// The serial number.
            /// </summary>
            public const string SerialNumber = "K2970378";

            /// <summary>
            /// The name.
            /// </summary>
            public const string Name = "Second Door";

            /// <summary>
            /// The ip address.
            /// </summary>
            public const string IpAddress = "192.168.1.8";

            /// <summary>
            /// The username.
            /// </summary>
            public const string Username = "admin";

            /// <summary>
            /// The password.
            /// </summary>
            public const string Password = "h1kv1s1on123";

            /// <summary>
            /// The instance.
            /// </summary>
            public static readonly DeviceDto Instance = new()
            {
                Name = Name,
                DeviceName = DeviceName,
                MacAddress = MacAddress,
                Model = Model,
                SerialNumber = SerialNumber,
            };

            /// <summary>
            /// The registration instance.
            /// </summary>
            public static readonly DeviceRegistrationDto RegistrationInstance = new DeviceRegistrationDto
            {
                Name = Name,
                DeviceName = DeviceName,
                MacAddress = MacAddress,
                Model = Model,
                SerialNumber = SerialNumber,
            };
        }

        /// <summary>
        /// The postgres.
        /// </summary>
        public static class Postgres
        {
            /// <summary>
            /// The image name.
            /// </summary>
            public const string ImageName = "postgres";

            /// <summary>
            /// The tag.
            /// </summary>
            public const string Tag = "14.6";

            /// <summary>
            /// The port.
            /// </summary>
            public const ushort Port = 5002;

            /// <summary>
            /// The container port.
            /// </summary>
            public const ushort ContainerPort = 5432;

            /// <summary>
            /// The username.
            /// </summary>
            public const string Username = "sa";

            /// <summary>
            /// The password.
            /// </summary>
            public const string Password = "tempore-123!";

            /// <summary>
            /// The host.
            /// </summary>
            public const string Host = "localhost";
        }

        /// <summary>
        /// The keycloak.
        /// </summary>
        public static class Keycloak
        {
            /// <summary>
            /// The image name.
            /// </summary>
            public const string ImageName = "quay.io/keycloak/keycloak";

            /// <summary>
            /// The tag.
            /// </summary>
            public const string Tag = "21.0.1";

            /// <summary>
            /// The username.
            /// </summary>
            public const string Username = "admin";

            /// <summary>
            /// The password.
            /// </summary>
            public const string Password = "tempore-123!";

            /// <summary>
            /// The host.
            /// </summary>
            public const string Host = "localhost";

            /// <summary>
            /// The port.
            /// </summary>
            public const int Port = 5003;

            /// <summary>
            /// The container port.
            /// </summary>
            public const ushort ContainerPort = 8080;

            /// <summary>
            /// The autority.
            /// </summary>
            public static readonly string Autority = $"http://{Host}:{Port}/auth/realms/master";

            /// <summary>
            /// The url.
            /// </summary>
            public static readonly string Url = $"http://{Host}:{Port}/auth";

            /// <summary>
            /// The base url.
            /// </summary>
            public static readonly string BaseUrl = $"http://{Host}:{Port}";

            /// <summary>
            /// The token url.
            /// </summary>
            public static readonly string TokenUrl = $"{Url}/realms/master/protocol/openid-connect/token";
        }

        /// <summary>
        /// The tempore.
        /// </summary>
        public static class Tempore
        {
            /// <summary>
            /// The host.
            /// </summary>
            public const string Host = "localhost";

            /// <summary>
            /// The port.
            /// </summary>
            public const int Port = 5000;

            /// <summary>
            /// The username.
            /// </summary>
            public const string Username = "tempore";

            /// <summary>
            /// The password.
            /// </summary>
            public const string Password = "tempore-123!";

            /// <summary>
            /// The invalid username.
            /// </summary>
            public const string InvalidUsername = "jane.doe";

            /// <summary>
            /// The invalid password.
            /// </summary>
            public const string InvalidPassword = "12345678";

            /// <summary>
            /// The ingress.
            /// </summary>
            public static readonly string Ingress = $"https://{Host}:{Port}";
        }
    }
}