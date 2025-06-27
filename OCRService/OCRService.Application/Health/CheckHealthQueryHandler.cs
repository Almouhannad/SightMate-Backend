using OCRService.Domain.Interfaces;
using SharedKernel.Base;
using SharedKernel.Messaging;

namespace OCRService.Application.Health;

internal sealed class CheckHealthQueryHandler(IOCRServiceProvider serviceProvider) : IQueryHandler<CheckHealthQuery, CheckHealthQueryResponse>
{
    public async Task<Result<CheckHealthQueryResponse>> Handle(CheckHealthQuery query, CancellationToken cancellationToken)
    {
        var healthResult = await serviceProvider.IsAvailable();

        if (healthResult)
        {
            return new CheckHealthQueryResponse { Status = "UP" }; 
        }
        else
        {
            return new CheckHealthQueryResponse { Status = "DOWN" };
        }
    }
}
