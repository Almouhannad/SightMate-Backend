using FluentValidation;
namespace VQAService.Application.Conversations.Create;

public sealed class CreateConversationQueryValidator : AbstractValidator<CreateConversationQuery>
{
    public CreateConversationQueryValidator()
    {
        var x = RuleFor(q => q.ImageBytes).NotEmpty();
        RuleFor(q => q.ImageBytes)
            .Must(bytes => bytes != null && bytes.All(byteItem =>
                byteItem >= 0 &&
                byteItem <= 255))
            .WithErrorCode("ImageInputValuesValidator")
            .WithMessage("ImageBytes must contain integers in [0, 255]");
    }
}
