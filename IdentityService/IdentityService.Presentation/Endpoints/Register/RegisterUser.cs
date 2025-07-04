using IdentityService.Application.Register;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Routing;
using SharedKernel.API;
using SharedKernel.Messaging;

namespace IdentityService.Presentation.Endpoints.Register;

internal sealed class RegisterUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("register", async
            (RegisterCommand command,
            ICommandHandler<RegisterCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Created, CustomResults.Problem);
        })
            .WithTags(Tags.REGISTER);
    }
}
