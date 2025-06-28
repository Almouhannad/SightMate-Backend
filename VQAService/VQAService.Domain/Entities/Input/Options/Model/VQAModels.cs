using System.Collections.ObjectModel;

namespace VQAService.Domain.Entities.Input.Options.Model;

public static class VQAModels
{
    private static readonly String _vlm = "vlm";
    public static String VLM => _vlm;

    // My MODEL :D
    private static readonly String _sightAssistant = "sight_assistant";
    public static String SIGHT_ASSISTANT => _sightAssistant;


    public static readonly ReadOnlyDictionary<String, String> VQASupportedModelsMap =
        new(
            new Dictionary<string, String>(StringComparer.OrdinalIgnoreCase)
            {
                    { _vlm, _vlm},
                    { _sightAssistant, _sightAssistant},
            }
        );

}
