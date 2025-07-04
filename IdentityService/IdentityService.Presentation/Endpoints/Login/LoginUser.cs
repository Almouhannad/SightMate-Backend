using IdentityService.Application.Login;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Routing;
using SharedKernel.API;
using SharedKernel.Messaging;

namespace IdentityService.Presentation.Endpoints.Login;

public class LoginUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("login", async
            (LoginQuery query,
            IQueryHandler<LoginQuery, LoginQueryResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .WithTags(Tags.SIGN_IN);
    }
}
