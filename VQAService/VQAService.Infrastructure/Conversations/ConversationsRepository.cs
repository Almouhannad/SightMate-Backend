using MongoDB.Driver;
using SharedKernel.Base;
using VQAService.Domain.Entities.Conversations;
using VQAService.Domain.Interfaces;

namespace VQAService.Infrastructure.Conversations;

public class ConversationsRepository : IConversationsRepository
{
    private readonly string _collectionName = "conversations";
    private readonly IMongoCollection<Conversation> _collection;

    public ConversationsRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Conversation>(_collectionName);
    }

    public async Task<Result<Conversation>> Create(Conversation conversation)
    {
        try
        {
            await _collection.InsertOneAsync(conversation);
            return conversation;
        }
        catch (Exception)
        {
            // TODO: LOG
            return Result.Failure<Conversation>(new Error("VQA.CREATE_CONVERSATION_ERROR", "Unable to create new conversation", ErrorType.Failure));
        }
    }

    public async Task<Result<Conversation>> GetById(Guid conversationId)
    {
        try
        {
            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, conversationId);
            var conversation = await _collection
                .Find(filter)
                .FirstOrDefaultAsync();

            if (conversation is null)
                return Result.Failure<Conversation>(new Error("VQA.CONVERSATION_NOT_FOUND", "Requested conversation not found", ErrorType.NotFound));

            return conversation;
        }
        catch (Exception)
        {
            return Result.Failure<Conversation>(new Error("VQA.GET_CONVERSATION_ERROR", "Unable to get requested conversation", ErrorType.Failure));
        }
    }

    public async Task<Result> UpdateHistory(Conversation conversation, List<HistoryItem> newHistory)
    {
        try
        {
            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, conversation.Id);
            var update = Builders<Conversation>.Update.Set(c => c.History, newHistory);

            var result = await _collection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                return Result.Failure<Conversation>(new Error("VQA.CONVERSATION_NOT_FOUND", "Requested conversation not found", ErrorType.NotFound));

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure<Conversation>(new Error("VQA.UPDATE_CONVERSATION_ERROR", "Unable to update conversation", ErrorType.Failure));
        }
    }
}