namespace SharedKernel.Multimedia;
// Image definition used by almost all AI services
public class Image
{
    public required List<int> Bytes { get; set; }

    public Dictionary<string, object>? Metadata { get; set; }
}
