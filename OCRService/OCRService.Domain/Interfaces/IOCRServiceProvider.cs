using OCRService.Domain.Entities.Input;
using OCRService.Domain.Entities.Output;
using SharedKernel.Base;

namespace OCRService.Domain.Interfaces;

public interface IOCRServiceProvider
{
    public Task<bool> IsAvailable();
    public Task<Result<OCROutput>> ProcessOCR (OCRInput ocrInput);
}
