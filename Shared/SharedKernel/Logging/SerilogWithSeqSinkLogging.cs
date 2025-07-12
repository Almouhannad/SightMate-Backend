using Microsoft.AspNetCore.Builder;
using Serilog;
using SharedKernel.Config;

namespace SharedKernel.Logging;

public static class SerilogWithSeqSinkLogging
{
    public static ConfigureHostBuilder RegisterSerilogWithSeq(this ConfigureHostBuilder host, String serviceName)
    {
        host.UseSerilog((context, loggingContext) =>
        {
            loggingContext
            .Enrich.FromLogContext()
            .Enrich.WithProperty("sevice_name", serviceName)
            .WriteTo.Seq(SHARED_CONFIG.SeqServer.ToString());
        });
        return host;
    }

    public static IApplicationBuilder UseSerilogWithSeq(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}
