using OCRService.Domain.Entities.Input;

namespace OCRService.Infrastructure.OCR.DTOs;

internal sealed class OCRInputDTO
{
    internal static Object FromDomain(OCRInput ocrInput)
    {
        var dto = new
        {
            bytes = ocrInput.Image.Bytes,
            metadata = ocrInput.Image.Metadata,
            options = new
            {
                lang = new
                {
                    lang = ocrInput.Options.Language.Code
                }
            }
        };
        return dto;
    } 

}
