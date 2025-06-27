using OCRService.Domain.Entities.Input.Options;
using OCRService.Domain.Entities.Input.Options.Language;
using OCRService.Domain.Entities.Input.Options.Model;
using SharedKernel;

namespace OCRService.Domain.Entities.Input;

public class OCRInput
{
    public required Image Image { get; set; }
    public OCROptions Options { get; set; } = new OCROptions();

    public static Result<OCRInput> Create(Image image, OCROptions? options)
    {
        if (image == null)
        {
            return Result<OCRInput>.ValidationFailure(Error.NullValue);
        }
        options ??= new OCROptions();

        var supportedModels = OCRModels.SupportedOCRModels;
        if(!supportedModels.Contains(options.Model))
        {
            return Result<OCRInput>.ValidationFailure(OCRInputValidationErrors.NotSupportedModel);
        }

        if (!OCRLanguages.OCRSupportedLanguagesMap.TryGetValue(options.Language.Code, out var _))
        {
            return Result<OCRInput>.ValidationFailure(OCRInputValidationErrors.NotSupportedLanguage);
        }

        return Result.Success(new OCRInput { Image=image, Options=options});
    }
}
