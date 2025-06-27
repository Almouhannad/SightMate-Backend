using SharedKernel.Messaging;

namespace OCRService.Application.Health;

public sealed record CheckHealthQuery() : IQuery<CheckHealthQueryResponse>;

