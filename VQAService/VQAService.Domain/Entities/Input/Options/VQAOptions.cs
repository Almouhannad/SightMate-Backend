using VQAService.Domain.Entities.Input.Options.Language;
using VQAService.Domain.Entities.Input.Options.Model;
using LanguageClass = SharedKernel.Base.Language;
namespace VQAService.Domain.Entities.Input.Options;

public class VQAOptions
{
    public LanguageClass Language { get; set; } = VQALanguages.ENGLISH;
    public String Model { get; set; } = VQAModels.VLM;
}
