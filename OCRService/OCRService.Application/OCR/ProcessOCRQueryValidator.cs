using FluentValidation;
using OCRService.Domain.Entities.Input.Options.Language;
using OCRService.Domain.Entities.Input.Options.Model;

namespace OCRService.Application.OCR;

internal sealed class ProcessOCRQueryValidator : AbstractValidator<ProcessOCRQuery>
{
    public ProcessOCRQueryValidator()
    {
        RuleFor(q => q.ImageBytes).NotEmpty();
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
                    return OCRLanguages.OCRSupportedLanguagesMap.TryGetValue(languageCode, out _);
                return true;
            })
            .WithErrorCode("NotSupportedLanguageValidator")
            .WithMessage("Selected language is not supported");

        RuleFor(q => q.Model)
            .Must(model =>
            {
                if (model is not null)
                    return OCRModels.OCRSupportedModelsMap.TryGetValue(model, out _);
                return true;
            })
            .WithErrorCode("NotSupportedModelValidator")
            .WithMessage("Selected model is not supported");
    }
}
