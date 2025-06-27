using SharedKernel.Base;

namespace OCRService.Domain.Entities.Input;

public static class OCRInputValidationErrors
{
    private static readonly Error _notSupportedLanguage =
        new("OCR.NOT_SUPPORTED_LANGUAGE", "Selected Language is not supported", ErrorType.Validation);
    public static Error NotSupportedLanguage => _notSupportedLanguage;

    private static readonly Error _notSupportedModel =
        new("OCR.NOT_SUPPORTED_MODEL", "Selected Model is not supported", ErrorType.Validation);
    public static Error NotSupportedModel => _notSupportedModel;

}
