using SharedKernel.Base;
using SharedKernel.Multimedia;
using VQAService.Domain.Entities.Conversations;

namespace VQAService.Domain.Interfaces;

public interface IVQAServiceProvider
{
    public Task<bool> IsAvailable();
    public Task<Result<String>> ProcessIC(Image image);
    public Task<Result<String>> ProcessVQA(Image image, String question, List<HistoryItem>? history);

}
