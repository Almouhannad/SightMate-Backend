using System.Collections.ObjectModel;

namespace OCRService.Domain.Entities.Input.Options.Model;

public static class OCRModels
{
    private static readonly String _easyocr = "easyocr";
    public static String EASYOCR => _easyocr;

    private static readonly String _paddleocr = "paddleocr";
    public static String PADDLEOCR => _paddleocr;

    private static readonly String _gemma = "gemma";
    public static String GEMMA => _gemma;

    public static readonly ReadOnlyCollection<String> SupportedOCRModels =
        new([_easyocr, _paddleocr, _gemma]);

}
