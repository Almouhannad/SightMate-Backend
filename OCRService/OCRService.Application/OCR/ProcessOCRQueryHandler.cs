using OCRService.Domain.Entities.Input;
using OCRService.Domain.Entities.Input.Options;
using OCRService.Domain.Entities.Input.Options.Language;
using OCRService.Domain.Entities.Output;
using OCRService.Domain.Interfaces;
using SharedKernel.Base;
using SharedKernel.Messaging;
using SharedKernel.Multimedia;

namespace OCRService.Application.OCR;

public sealed class ProcessOCRQueryHandler(IOCRServiceProvider serviceProvider) : IQueryHandler<ProcessOCRQuery, OCROutput>
{
    public async Task<Result<OCROutput>> Handle(ProcessOCRQuery query, CancellationToken cancellationToken)
    {
        #region Process inputs
        var image = new Image { Bytes = query.ImageBytes, Metadata = query.ImageMetadata };

        var options = new OCROptions();
        options.Describe = query.Describe ?? options.Describe;
        options.Model = query.Model ?? options.Model;
        if (query.LanguageCode is not null &&
            OCRLanguages.OCRSupportedLanguagesMap.TryGetValue(query.LanguageCode, out var lang))
        {
            options.Language = lang;
        }

        var createInputResult = OCRInput.Create(image, options);
        if (createInputResult.IsFailure)
        {
            return Result.Failure<OCROutput>(createInputResult.Error);
        }
        var ocrInput = createInputResult.Value;
        #endregion

        #region Check service availability
        var isServiceAvailable = await serviceProvider.IsAvailable();
        if (!isServiceAvailable)
        {
            return Result.Failure<OCROutput>(
                new Error("OCR.SERVICE_NOT_AVAILABLE", "OCR service is not available currently", ErrorType.Conflict)
                );
        }
        #endregion

        #region Perform OCR
        var processOCRResult = await serviceProvider.ProcessOCR(ocrInput);
        if (processOCRResult.IsFailure)
        {
            return Result.Failure<OCROutput>(processOCRResult.Error);
        }
        var ocrOutput = processOCRResult.Value;
        #endregion

        return ocrOutput;
    }
}
