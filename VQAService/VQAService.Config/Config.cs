using SharedKernel.Config;

namespace VQAService.Config;

public static class CONFIG
{
    // Env-var names
    private const string VqaBaseUriVar = "VQA_SERVICE_BASE_URI";
    private const string VqaApiKeyVar = "VQA_SERVICE_API_KEY";
    private const string MongoDatabaseVar = "MONGO_DATABASE";
    private const string MongoConnectionUriVar = "MONGODB_URI";

    // VQA Service
    private static readonly Lazy<Uri> _vqaBaseUri = new(() => ENVHelper.ParseUri(VqaBaseUriVar));
    public static Uri VqaServiceBaseUri => _vqaBaseUri.Value;

    private static readonly Lazy<string> _vqaApiKey = new(() => ENVHelper.GetEnv(VqaApiKeyVar));
    public static string VqaServiceApiKey => _vqaApiKey.Value;

    // MongoDB parts
    private static readonly Lazy<string> _mongoDatabase = new(() => ENVHelper.GetEnv(MongoDatabaseVar));
    public static string MongoDatabase => _mongoDatabase.Value;

    // Complete connection URI (e.g. "mongodb://user:pass@host:port/db")
    private static readonly Lazy<Uri> _mongoUri = new(() => ENVHelper.ParseUri(MongoConnectionUriVar));
    public static Uri MongoConnectionUri => _mongoUri.Value;

}
