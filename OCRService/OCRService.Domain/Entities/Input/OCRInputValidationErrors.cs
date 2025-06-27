using SharedKernel.Base;

namespace OCRService.Domain.Entities.Input;

public static class OCRInputValidationErrors
{
    private static readonly Error _notSupportedLanguage =
        new("OCR.NOT_SUPPORTED_LANGUAGE", "Selected language is not supported", ErrorType.Validation);
    public static Error NotSupportedLanguage => _notSupportedLanguage;

    private static readonly Error _notSupportedModel =
        new("OCR.NOT_SUPPORTED_MODEL", "Selected model is not supported", ErrorType.Validation);
    public static Error NotSupportedModel => _notSupportedModel;

}
