using VQAService.Domain.Entities.Output;

namespace VQAService.Application.IC;

public class ProcessICQueryResponse
{
    public required ICOutput ICOutput { get; set; }
}
