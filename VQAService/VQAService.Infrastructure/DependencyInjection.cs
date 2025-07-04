using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using VQAService.Config;
using VQAService.Domain.Interfaces;
using IdentityService.Infrastructure;

namespace VQAService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
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

        services.AddHttpContextAccessor();
        services.RegisterUserContextImplementationFromInfrastructure();
        return services;
    }
}
