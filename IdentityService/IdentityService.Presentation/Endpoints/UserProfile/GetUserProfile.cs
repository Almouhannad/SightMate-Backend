using IdentityService.Application.UserProfile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Routing;
using SharedKernel.API;
using SharedKernel.Messaging;

namespace IdentityService.Presentation.Endpoints.UserProfile;

public class GetUserProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("profile", async
            (IQueryHandler<GetUserProfileQuery, GetUserProfileQueryResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserProfileQuery();
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
            .RequireAuthorization()
            .WithTags(Tags.PROFILE);
    }
}
