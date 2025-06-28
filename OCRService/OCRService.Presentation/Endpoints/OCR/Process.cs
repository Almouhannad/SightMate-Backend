using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.API;
using SharedKernel.Messaging;
using OCRService.Application.OCR;

namespace OCRService.Presentation.Endpoints.OCR;

internal sealed class ProcessOCR : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ocr", async
            (ProcessOCRQuery query,
            IQueryHandler<ProcessOCRQuery, ProcessOCRQueryResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithTags(Tags.OCR);
    }
}
