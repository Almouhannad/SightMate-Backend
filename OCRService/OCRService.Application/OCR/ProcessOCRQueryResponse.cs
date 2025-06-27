using OCRService.Domain.Entities.Output;

namespace OCRService.Application.OCR;

public sealed class ProcessOCRQueryResponse
{
    public required OCROutput OCROutput { get; set; }
}
