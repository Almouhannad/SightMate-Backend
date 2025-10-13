using FluentValidation;
using VQAService.Domain.Entities.Input.Options.Language;
using VQAService.Domain.Entities.Input.Options.Model;

namespace VQAService.Application.IC;

public sealed class ProcessICQueryValidator : AbstractValidator<ProcessICQuery>
{
    public ProcessICQueryValidator()
    {
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
