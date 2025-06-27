using OCRService.Domain.Entities.Input;
using OCRService.Domain.Entities.Output;
using OCRService.Domain.Interfaces;
using SharedKernel.Base;

namespace OCRService.Infrastructure.OCR;

public class OCRServiceProvider : IOCRServiceProvider
{
    public Task<bool> IsAvailable()
    {
        return Task.FromResult(true);
    }

    public Task<Result<OCROutput>> ProcessOCR(OCRInput ocrInput)
    {
        var mockOutput = new OCROutput { DetectedTexts = [], Description = null };
        return Task.FromResult(Result.Success(mockOutput));
    }
}
