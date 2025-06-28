using OCRService.Domain.Entities.Input.Options;
using SharedKernel.Base;
using SharedKernel.Multimedia;

namespace OCRService.Domain.Entities.Input;

public class OCRInput
{
    public required Image Image { get; set; }
    public OCROptions Options { get; set; } = new OCROptions();

    public static Result<OCRInput> Create(Image image, OCROptions? options)
    {
        if (image == null)
        {
            return Result<OCRInput>.ValidationFailure(Error.NullValue);
        }
        options ??= new OCROptions();

        return Result.Success(new OCRInput { Image=image, Options=options});
    }
}
