using OCRService.Domain.Entities.Input.Options.Language;
using OCRService.Domain.Entities.Input.Options.Model;

namespace OCRService.Domain.Entities.Input.Options;

public class OCROptions
{
    public OCRLanguage Language { get; set; } = OCRLanguages.ENGLISH;
    public String Model { get; set; } = OCRModels.GEMMA;
    public bool Describe { get; set; } = true;

}
