using MongoDB.Bson;
using SharedKernel.Multimedia;
using System.Text.Json;
using VQAService.Domain.Entities.Conversations;
using static VQAService.Infrastructure.Conversations.DAOs.ConversationsDAOs;

namespace VQAService.Infrastructure.Conversations.DAOs;

internal static class ConversationsMappers
{
    public static ConversationDAO ToDAO(this Conversation domain)
        => new()
        {
            Id = domain.Id,
            UserId = domain.UserId,
            Image = domain.Image.ToDAO(),
            History = [.. domain.History.Select(h => h.ToDAO())]
        };

    public static ImageDAO ToDAO(this Image domain)
        => new()
        {
            Bytes = domain.Bytes,
            Metadata = domain.Metadata is null
                         ? null
                         : BsonDocument.Parse(JsonSerializer.Serialize(domain.Metadata))
        };

    public static HistoryItemDAO ToDAO(this HistoryItem domain)
        => new()
        {
            Question = domain.Question,
            Answer = domain.Answer
        };

    public static Conversation ToDomain(this ConversationDAO dao)
        => new()
        {
            Id = dao.Id,
            UserId = dao.UserId,
            Image = dao.Image.ToDomain(),
            History = [.. dao.History.Select(h => h.ToDomain())]
        };

    public static Image ToDomain(this ImageDAO dao)
        => new()
        {
            Bytes = dao.Bytes,
            Metadata = dao.Metadata is null
                         ? null
                         : JsonSerializer.Deserialize<Dictionary<string, object>>(dao.Metadata.ToJson())
        };

    public static HistoryItem ToDomain(this HistoryItemDAO dao)
        => new()
        {
            Question = dao.Question,
            Answer = dao.Answer
        };
}