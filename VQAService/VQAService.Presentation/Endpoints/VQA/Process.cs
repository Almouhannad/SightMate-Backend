using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.API;
using SharedKernel.Messaging;
using VQAService.Application.IC;
using VQAService.Application.VQA;
using VQAService.Domain.Entities.Output;

namespace VQAService.Presentation.Endpoints.VQA;

internal sealed class ProcessVQA : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("vqa", async
            (ProcessVQAQuery query,
            IQueryHandler<ProcessVQAQuery, VQAOutput> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithTags(Tags.VQA);
    }
}
