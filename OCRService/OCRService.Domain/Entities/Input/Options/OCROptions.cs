using OCRService.Domain.Entities.Input.Options.Language;
using OCRService.Domain.Entities.Input.Options.Model;
using LanguageClass = SharedKernel.Base.Language;

namespace OCRService.Domain.Entities.Input.Options;

public class OCROptions
{
    public LanguageClass Language { get; set; } = OCRLanguages.ENGLISH;
    public String Model { get; set; } = OCRModels.GEMMA;
    public bool Describe { get; set; } = true;

}
