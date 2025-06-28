using OCRService.Domain.Entities.Output;
using System.Text.Json;

internal sealed class OCROutputDTO
{
    public OcrTextBlockDto[] Texts { get; set; } = [];

    public JsonElement? Description { get; set; }

    public OCROutput ToDomain()
    {
        var domainModel = new OCROutput
        {
            DetectedTexts = [.. Texts
                .Select(t => new OCROutputItem
                {
                    Text = t.Text,
                    Confidence = t.Confidence,
                    Box = new Rectangle
                    {
                        Left = t.Box.Left,
                        Top = t.Box.Top,
                        Right = t.Box.Right,
                        Bottom = t.Box.Bottom
                    }
                })],

            Description = Description is not null && Description.Value.ValueKind == JsonValueKind.Object
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(Description.Value.GetRawText())
                : null
        };
        return domainModel;
    }

    public class OcrTextBlockDto
    {
        public string Text { get; set; } = "";

        public double? Confidence { get; set; }

        public BoxDto Box { get; set; } = new();
    }

    public class BoxDto
    {
        public double Left { get; set; }

        public double Top { get; set; }

        public double Right { get; set; }
        public double Bottom { get; set; }
    }
}
