using Microsoft.Extensions.DependencyInjection;
using OCRService.Domain.Interfaces;
namespace OCRService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IOCRServiceProvider)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        return services;
    }
}
