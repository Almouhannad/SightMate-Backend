using SharedKernel.Base;
using VQAService.Domain.Entities.Input.Options;

namespace VQAService.Domain.Entities.Input;

public class VQAInput
{
    public Guid ConversationId { get; set; }
    public required String Question { get; set; }
    public VQAOptions Options { get; set; } = new VQAOptions();

    public static Result<VQAInput> Create(Guid conversationId, String question, VQAOptions? options)
    {
        if (question is null)
        {
            return Result<VQAInput>.ValidationFailure(Error.NullValue);
        }
        options ??= new VQAOptions();

        return Result.Success(new VQAInput
        {
            ConversationId = conversationId,
            Question = question,
            Options = options
        });
    }
}
