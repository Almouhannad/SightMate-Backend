using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.API;
using SharedKernel.Base;
using SharedKernel.Messaging;
using VQAService.Application.Conversations.Create;

namespace VQAService.Presentation.Endpoints.Conversations;

internal sealed class CreateConversation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("create", async
            (CreateConversationQuery query,
            IQueryHandler<CreateConversationQuery, CreateConversationQueryResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithTags(Tags.CONVERSATIONS);
    }
}
