using System.Collections.ObjectModel;
using LanguageClass = SharedKernel.Base.Language;

namespace OCRService.Domain.Entities.Input.Options.Language;

public static class OCRLanguages
{
    private static readonly LanguageClass _english = new(name: "English", code: "en");
    public static LanguageClass ENGLISH => _english;

    private static readonly LanguageClass _arabic = new(name: "Arabic", code: "ar");
    public static LanguageClass Arabic => _arabic;

    public static readonly ReadOnlyDictionary<string, LanguageClass> OCRSupportedLanguagesMap =
        new(
            new Dictionary<string, LanguageClass>(StringComparer.OrdinalIgnoreCase)
            {
                    { _english.Code, _english },
                    { _arabic.Code, _arabic }
            }
        );
}
