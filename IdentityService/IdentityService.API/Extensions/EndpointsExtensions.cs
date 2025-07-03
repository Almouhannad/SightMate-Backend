using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedKernel.API;

namespace IdentityService.API.Extensions;

public static class EndpointsExtensions
{
    public static IServiceCollection AddAccountManagementEndpoints(this IServiceCollection services)
    {
        var assembly = typeof(EndpointsExtensions).Assembly;
        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();
        services.TryAddEnumerable(serviceDescriptors);
        return services;
    }

    public static IApplicationBuilder MapAccountManagementEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}
