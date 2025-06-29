using MongoDB.Driver;
using SharedKernel.Base;
using VQAService.Domain.Entities.Conversations;
using VQAService.Domain.Interfaces;
using VQAService.Infrastructure.Conversations.DAOs;
using static VQAService.Infrastructure.Conversations.DAOs.ConversationsDAOs;

namespace VQAService.Infrastructure.Conversations;

public class ConversationsRepository : IConversationsRepository
{
    private readonly String _collectionName = "conversations";
    private readonly IMongoCollection<ConversationDAO> _collection;

    public ConversationsRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<ConversationDAO>(_collectionName);
    }

    public async Task<Result<Conversation>> Create(Conversation conversation)
    {
        try
        {
            var dao = conversation.ToDAO();
            await _collection.InsertOneAsync(dao);
            return dao.ToDomain();
        }
        catch (Exception)
        {
            // TODO: log ex
            return Result.Failure<Conversation>(
                new Error("VQA.CREATE_CONVERSATION_ERROR",
                          "Unable to create new conversation",
                          ErrorType.Failure)
            );
        }
    }

    public async Task<Result<Conversation>> GetById(Guid conversationId)
    {
        try
        {
            var dao = await _collection
                .Find(c => c.Id == conversationId)
                .FirstOrDefaultAsync();

            if (dao is null)
                return Result.Failure<Conversation>(
                    new Error("VQA.CONVERSATION_NOT_FOUND",
                              "Requested conversation not found",
                              ErrorType.NotFound)
                );

            return dao.ToDomain();
        }
        catch (Exception)
        {
            // TODO: log ex
            return Result.Failure<Conversation>(
                new Error("VQA.GET_CONVERSATION_ERROR",
                          "Unable to get requested conversation",
                          ErrorType.Failure)
            );
        }
    }

    public async Task<Result> UpdateHistory(Conversation conversation, List<HistoryItem> newHistory)
    {
        try
        {
            var historyDaos = newHistory.Select(h => h.ToDAO()).ToList();
            var update = Builders<ConversationDAO>
                         .Update.Set(c => c.History, historyDaos);

            var result = await _collection
                .UpdateOneAsync(c => c.Id == conversation.Id, update);

            if (result.MatchedCount == 0)
                return Result.Failure<Conversation>(
                    new Error("VQA.CONVERSATION_NOT_FOUND",
                              "Requested conversation not found",
                              ErrorType.NotFound)
                );

            return Result.Success();
        }
        catch (Exception)
        {
            // TODO: log ex
            return Result.Failure<Conversation>(
                new Error("VQA.UPDATE_CONVERSATION_ERROR",
                          "Unable to update conversation",
                          ErrorType.Failure)
            );
        }
    }
}