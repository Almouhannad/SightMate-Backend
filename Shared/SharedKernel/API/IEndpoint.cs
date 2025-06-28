using Microsoft.AspNetCore.Routing;
namespace SharedKernel.API;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
