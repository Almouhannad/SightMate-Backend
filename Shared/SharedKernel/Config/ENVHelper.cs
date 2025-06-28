namespace SharedKernel.Config;

public class ENVHelper
{
    public static string GetEnv(string name)
    {
        var v = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(v))
            throw new InvalidOperationException(
                $"Required environment variable '{name}' is not set or is empty.");
        return v!;
    }

    public static Uri ParseUri(string name)
    {
        if (!Uri.TryCreate(Environment.GetEnvironmentVariable(name), UriKind.Absolute, out var uri))
            throw new InvalidOperationException(
                $"Environment variable '{name}' does not contain a valid absolute URI.");
        return uri;
    }

}
