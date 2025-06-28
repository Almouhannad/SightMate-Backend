using SharedKernel.Config;

namespace OCRService.Config;

public static class CONFIG
{
    // Env-var names
    private const string BaseUriVar = "OCR_SERVICE_BASE_URI";
    private const string ApiKeyVar = "OCR_SERVICE_API_KEY";
    
    // Lazy, exception-safe properties
    private static readonly Lazy<Uri> _baseUri = new(() => ENVHelper.ParseUri(BaseUriVar));
    public static Uri OCRServiceBaseUri => _baseUri.Value;

    private static readonly Lazy<string> _apiKey = new(() => ENVHelper.GetEnv(ApiKeyVar));
    public static string OCRServiceApiKey => _apiKey.Value;
}
