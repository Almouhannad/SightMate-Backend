using SharedKernel.Base;
using SharedKernel.Multimedia;
using VQAService.Domain.Entities.Input.Options;

namespace VQAService.Domain.Entities.Input;

public class ICInput
{
    public required Image Image { get; set; }
    public VQAOptions Options { get; set; } = new VQAOptions();
    public static Result<ICInput> Create(Image image, VQAOptions? options)
    {
        if (image is null)
        {
            return Result<ICInput>.ValidationFailure(Error.NullValue);
        }
        options ??= new VQAOptions();

        return Result.Success(new ICInput
        {
            Image = image,
            Options = options
        });
    }
}
