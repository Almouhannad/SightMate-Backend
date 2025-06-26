namespace SharedKernel;
// Image definition used by almost all AI services
public class Image
{
    public required List<int> Bytes { get; set; }

    public Dictionary<String, Object>? Metadata { get; set; }
}
