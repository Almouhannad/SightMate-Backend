﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.API;
using SharedKernel.Messaging;
using OCRService.Application.OCR;
using OCRService.Domain.Entities.Output;

namespace OCRService.Presentation.Endpoints.OCR;

internal sealed class ProcessOCR : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("ocr", async
            (ProcessOCRQuery query,
            IQueryHandler<ProcessOCRQuery, OCROutput> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithTags(Tags.OCR);
    }
}
