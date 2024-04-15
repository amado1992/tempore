namespace Tempore.Tests.Fixtures.Postgres
{
    extern alias TemporeServer;

    /// <summary>
    /// The tempore docker postgres back end api host web application factory.
    /// </summary>
    public class TemporeServerWebApplicationFactory : TemporeWebApplicationFactoryBase<TemporeServer.Program>
    {
    }
}