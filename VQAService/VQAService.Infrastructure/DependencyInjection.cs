using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using VQAService.Config;

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
        return services;
    }
}
