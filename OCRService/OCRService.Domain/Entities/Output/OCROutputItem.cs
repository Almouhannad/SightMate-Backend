namespace OCRService.Domain.Entities.Output;

public class OCROutputItem
{
    public required String Text { get; set; }
    public double? Confidence { get; set; }
    public Rectangle? Box { get; set; }
}
