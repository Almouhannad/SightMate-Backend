using FluentValidation;
using VQAService.Domain.Entities.Input.Options.Language;
using VQAService.Domain.Entities.Input.Options.Model;

namespace VQAService.Application.IC;

public sealed class ProcessICQueryValidator : AbstractValidator<ProcessICQuery>
{
    public ProcessICQueryValidator()
    {
        var x =  RuleFor(q => q.ImageBytes).NotEmpty();
        RuleFor(q => q.ImageBytes)
            .Must(bytes => bytes != null && bytes.All(byteItem =>
                byteItem >= 0 &&
                byteItem <= 255))
            .WithErrorCode("ImageInputValuesValidator")
            .WithMessage("ImageBytes must contain integers in [0, 255]");

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
