using OCRService.Domain.Entities.Output;
using SharedKernel.Messaging;

namespace OCRService.Application.OCR;

public sealed record ProcessOCRQuery(List<int> ImageBytes, Dictionary<String, Object>? ImageMetadata,
    String? LanguageCode, String? Model, bool? Describe) : IQuery<OCROutput>;