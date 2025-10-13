using SharedKernel.Messaging;

namespace VQAService.Application.Conversations.Create;

public sealed record CreateConversationQuery(List<int> ImageBytes, Dictionary<string, object>? ImageMetadata) : IQuery<CreateConversationQueryResponse>;
