using SharedKernel.Messaging;

namespace VQAService.Application.Health;

public sealed record CheckHealthQuery() : IQuery<CheckHealthQueryResponse>;

