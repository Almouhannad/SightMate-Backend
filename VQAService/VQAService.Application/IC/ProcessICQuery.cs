using SharedKernel.Messaging;

namespace VQAService.Application.IC;

public sealed record ProcessICQuery(
    List<int> ImageBytes, Dictionary<String, Object>? ImageMetadata,
    String? LanguageCode, String? Model) : IQuery<ProcessICQueryResponse>;