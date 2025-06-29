using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using VQAService.Config;
using VQAService.Domain.Interfaces;

namespace VQAService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            return new MongoClient(CONFIG.MongoConnectionUri.ToString());
        });
        services.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(CONFIG.MongoDatabase);
        });
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IConversationsRepository)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IVQAServiceProvider)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        return services;
    }
}
