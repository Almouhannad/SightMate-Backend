using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.API;
using SharedKernel.Messaging;
using VQAService.Application.IC;
using VQAService.Domain.Entities.Output;

namespace VQAService.Presentation.Endpoints.IC;

internal sealed class ProcssIC : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("ic", async
            (ProcessICQuery query,
            IQueryHandler<ProcessICQuery, ICOutput> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithTags(Tags.IC);
    }
}
