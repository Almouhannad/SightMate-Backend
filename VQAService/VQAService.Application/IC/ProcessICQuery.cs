using SharedKernel.Messaging;
using VQAService.Domain.Entities.Output;

namespace VQAService.Application.IC;

public sealed record ProcessICQuery(Guid ConversationId, String? LanguageCode, String? Model) : IQuery<ICOutput>;