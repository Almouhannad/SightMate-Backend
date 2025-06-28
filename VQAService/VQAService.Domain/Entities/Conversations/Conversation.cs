using SharedKernel.Multimedia;

namespace VQAService.Domain.Entities.Conversations;

public class Conversation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required Image Image { get; set; }
    public List<HistoryItem> History { get; set; } = [];
}
