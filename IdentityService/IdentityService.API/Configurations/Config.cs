using SharedKernel.Config;

namespace IdentityService.API.Configurations;

public static class CONFIG
{
    // Env-var names
    private const string ConnectionStringVar = "CONNECTION_STRING";

    // Lazy, exception-safe properties
    private static readonly Lazy<String> _connectionString = new(() => ENVHelper.GetEnv(ConnectionStringVar));
    public static String ConnectionString => _connectionString.Value;

}
