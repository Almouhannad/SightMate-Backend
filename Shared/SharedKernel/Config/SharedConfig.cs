namespace SharedKernel.Config;

public static class SHARED_CONFIG
{
    private const string JWTSecretKeyVar = "JWT_SECRET_KEY";
    private const string JWTIssuerVar = "JWT_ISSUER";
    private const string JWTAudienceVar = "JWT_AUDIENCE";
    private const string SeqServerVar = "SEQ_SERVER";


    private static readonly Lazy<String> _jwtSecretKey = new(() => ENVHelper.GetEnv(JWTSecretKeyVar));
    public static String JWTSecretKey => _jwtSecretKey.Value;

    private static readonly Lazy<String> _jwtIssuer = new(() => ENVHelper.GetEnv(JWTIssuerVar));
    public static String JWTIssuer => _jwtIssuer.Value;

    private static readonly Lazy<String> _jwtAudience = new(() => ENVHelper.GetEnv(JWTAudienceVar));
    public static String JWTAudience => _jwtAudience.Value;

    private static readonly Lazy<Uri> _seqServer = new(() => ENVHelper.ParseUri(SeqServerVar));
    public static Uri SeqServer => _seqServer.Value;
}