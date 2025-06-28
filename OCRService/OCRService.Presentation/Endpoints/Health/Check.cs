using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OCRService.Application.Health;
using SharedKernel.API;
using SharedKernel.Messaging;

namespace OCRService.Presentation.Endpoints.Health;

internal sealed class CheckHealth : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("health", async
            (IQueryHandler<CheckHealthQuery,CheckHealthQueryResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var checkHealthQuery = new CheckHealthQuery();
            var result = await handler.Handle(checkHealthQuery, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithTags(Tags.HEALTH);
    }
}