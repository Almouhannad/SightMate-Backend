namespace OCRService.Domain.Entities.Output;

public class OCROutput
{
    public List<OCROutputItem> DetectedTexts { get; set; } = [];
    public Dictionary<String, Object>? Description { get; set; }
}
