using IdentityService.Domain.Interfaces;
using SharedKernel.Base;
using SharedKernel.Messaging;
using SharedKernel.Multimedia;
using VQAService.Domain.Entities.Conversations;
using VQAService.Domain.Interfaces;

namespace VQAService.Application.Conversations.Create;

public sealed class CreateConversationQueryHandler(IUserContext userContext, IConversationsRepository conversationsRepository) : IQueryHandler<CreateConversationQuery, CreateConversationQueryResponse>
{
    public async Task<Result<CreateConversationQueryResponse>> Handle(CreateConversationQuery query, CancellationToken cancellationToken)
    {
        var image = new Image { Bytes = query.ImageBytes, Metadata = query.ImageMetadata };
        Conversation conversation = new() { Id = Guid.NewGuid(), UserId = userContext.UserId, Image = image, History = [] };
        var createConversationResult = await conversationsRepository.Create(conversation);
        if (createConversationResult.IsFailure)
        {
            return Result.Failure<CreateConversationQueryResponse>(createConversationResult.Error);

        }
        conversation = createConversationResult.Value;
        return new CreateConversationQueryResponse { ConversationId = conversation.Id };
    }
}
