namespace VQAService.Domain.Entities.Output;

public class ICOutput
{
    public Guid ConversationId { get; set; }
    public required String Caption { get; set; }
}
