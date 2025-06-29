using SharedKernel.Multimedia;
using VQAService.Domain.Entities.Conversations;

namespace VQAService.Infrastructure.VQA.DTOs;

public static class VQAServiceMappers
{
    public static ImageDTO ToDTO(this Image img) => new()
    {
        Bytes = img.Bytes,
        Metadata = img.Metadata
    };

    public static HistoryItemDTO ToDTO(this HistoryItem hi) => new()
    {
        Question = hi.Question,
        Answer = hi.Answer
    };

    public static Image ToDomain(this ImageDTO dto) => new()
    {
        Bytes = dto.Bytes,
        Metadata = dto.Metadata
    };

    public static HistoryItem ToDomain(this HistoryItemDTO dto) => new()
    {
        Question = dto.Question,
        Answer = dto.Answer
    };

    public static string ToDomain(this ResponseDTO dto)
        => dto.Output;
}