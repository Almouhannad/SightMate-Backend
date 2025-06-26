using System.Collections.ObjectModel;

namespace OCRService.Domain.Entities.Input.Options.Language;

public static class OCRLanguages
{
    private static readonly OCRLanguage _english = new(name: "English", code: "en");
    public static OCRLanguage ENGLISH => _english;

    private static readonly OCRLanguage _arabic = new(name: "Arabic", code: "ar");
    public static OCRLanguage Arabic => _arabic;

    public static readonly ReadOnlyCollection<OCRLanguage> SupportedOCRLanguages = new([_english, _arabic]);
}
