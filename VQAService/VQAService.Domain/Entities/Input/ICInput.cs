using SharedKernel.Base;
using SharedKernel.Multimedia;
using VQAService.Domain.Entities.Input.Options;

namespace VQAService.Domain.Entities.Input;

public class ICInput
{
    public Guid ConversationId { get; set; }
    public VQAOptions Options { get; set; } = new VQAOptions();
    public static Result<ICInput> Create(Guid conversationId, VQAOptions? options)
    {
        options ??= new VQAOptions();

        return Result.Success(new ICInput
        {
            ConversationId = conversationId,
            Options = options
        });
    }
}
