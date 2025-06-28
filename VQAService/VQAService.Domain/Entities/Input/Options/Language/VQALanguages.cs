using System.Collections.ObjectModel;
using LanguageClass = SharedKernel.Base.Language;
namespace VQAService.Domain.Entities.Input.Options.Language;

public static class VQALanguages
{
    private static readonly LanguageClass _english = new(name: "English", code: "en");
    public static LanguageClass ENGLISH => _english;

    public static readonly ReadOnlyDictionary<string, LanguageClass> VQASupportedLanguagesMap =
        new(
            new Dictionary<string, LanguageClass>(StringComparer.OrdinalIgnoreCase)
            {
                    { _english.Code, _english }
            }
        );
}
