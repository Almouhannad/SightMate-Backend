namespace VQAService.Infrastructure.VQA.DTOs;

public class ImageDTO
{
    public required List<int> Bytes { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class HistoryItemDTO
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
}

public class CaptioningRequestDTO
{
    public required ImageDTO Image { get; set; }
    public List<HistoryItemDTO>? History { get; set; }
}

public class QuestionRequestDTO
{
    public required ImageDTO Image { get; set; }
    public required string Question { get; set; }
    public List<HistoryItemDTO>? History { get; set; }
}

public class ResponseDTO
{
    public required string Output { get; set; }
    public Dictionary<string, object>? Details { get; set; }
}