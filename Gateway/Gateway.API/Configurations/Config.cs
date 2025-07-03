using SharedKernel.Config;

namespace Gateway.API.Configurations;

public static class CONFIG
{
    // Env-var names
    private const string JWTSecretKeyVar = "JWT_SECRET_KEY";
    private const string JWTIssuerVar = "JWT_ISSUER";
    private const string JWTAudienceVar = "JWT_AUDIENCE";

    // Lazy, exception-safe properties
    private static readonly Lazy<String> _jwtSecretKey = new(() => ENVHelper.GetEnv(JWTSecretKeyVar));
    public static String JWTSecretKey => _jwtSecretKey.Value;

    private static readonly Lazy<String> _jwtIssuer = new(() => ENVHelper.GetEnv(JWTIssuerVar));
    public static String JWTIssuer => _jwtIssuer.Value;

    private static readonly Lazy<String> _jwtAudience = new(() => ENVHelper.GetEnv(JWTAudienceVar));
    public static String JWTAudience => _jwtAudience.Value;

}
