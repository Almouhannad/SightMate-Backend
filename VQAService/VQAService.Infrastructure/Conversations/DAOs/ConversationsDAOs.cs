using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VQAService.Infrastructure.Conversations.DAOs;

public class ConversationsDAOs
{
    public class ImageDAO
    {
        [BsonElement("Bytes")]
        public List<int> Bytes { get; set; } = null!;

        [BsonElement("Metadata")]
        public BsonDocument? Metadata { get; set; }
    }

    public class HistoryItemDAO
    {
        [BsonElement("Question")]
        public string Question { get; set; } = null!;

        [BsonElement("Answer")]
        public string Answer { get; set; } = null!;
    }

    public class ConversationDAO
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("UserId")]
        public Guid UserId { get; set; }

        [BsonElement("Image")]
        public ImageDAO Image { get; set; } = null!;

        [BsonElement("History")]
        public List<HistoryItemDAO> History { get; set; } = [];
    }
}
