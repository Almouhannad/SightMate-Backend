using FluentValidation;
using VQAService.Domain.Entities.Input.Options.Language;
using VQAService.Domain.Entities.Input.Options.Model;

namespace VQAService.Application.VQA;

public sealed class ProcessVQAQueryValidator : AbstractValidator<ProcessVQAQuery>
{
    public ProcessVQAQueryValidator()
    {
        RuleFor(q => q.ConversationId).NotEmpty();
        RuleFor(q => q.Question).NotEmpty();

        RuleFor(q => q.LanguageCode)
            .Must(languageCode =>
            {
                if (languageCode is not null)
                    return VQALanguages.VQASupportedLanguagesMap.TryGetValue(languageCode, out _);
                return true;
            })
            .WithErrorCode("NotSupportedLanguageValidator")
            .WithMessage("Selected language is not supported");

        RuleFor(q => q.Model)
            .Must(model =>
            {
                if (model is not null)
                    return VQAModels.VQASupportedModelsMap.TryGetValue(model, out _);
                return true;
            })
            .WithErrorCode("NotSupportedModelValidator")
            .WithMessage("Selected model is not supported");
    }
}
