using SharedKernel.Messaging;
using VQAService.Domain.Entities.Output;

namespace VQAService.Application.VQA;

public sealed record ProcessVQAQuery(Guid ConversationId, String Question,
    String? LanguageCode, String? Model) : IQuery<VQAOutput>;