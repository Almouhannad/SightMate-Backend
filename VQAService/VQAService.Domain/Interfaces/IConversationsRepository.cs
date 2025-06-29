using SharedKernel.Base;
using VQAService.Domain.Entities.Conversations;

namespace VQAService.Domain.Interfaces;

public interface IConversationsRepository
{
    public Task<Result<Conversation>> GetById(Guid conversationId);
    public Task<Result<Conversation>> Create(Conversation conversation);
    public Task<Result> UpdateHistory(Conversation conversation, List<HistoryItem> NewHistory);
}
