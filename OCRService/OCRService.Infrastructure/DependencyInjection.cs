using Microsoft.Extensions.DependencyInjection;
using OCRService.Domain.Interfaces;
using OCRService.Infrastructure.OCR;
namespace OCRService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient<OCRServiceProvider>(); // DI for client
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IOCRServiceProvider)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        return services;
    }
}
