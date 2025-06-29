using SharedKernel.Base;
using SharedKernel.Multimedia;
using VQAService.Domain.Entities.Conversations;
using VQAService.Domain.Interfaces;

namespace VQAService.Infrastructure.VQA;

public class VQAServiceProvider : IVQAServiceProvider
{
    // Mock
    public Task<bool> IsAvailable()
    {
        return Task.FromResult(true);
    }

    public Task<Result<string>> ProcessIC(Image image)
    {
        return Task.FromResult(Result.Success("Messi"));
    }

    public Task<Result<string>> ProcessVQA(Image image, string question, List<HistoryItem>? history)
    {
        return Task.FromResult(Result.Success("Messi"));
    }
}
