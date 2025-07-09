using IdentityService.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddRateLimiter(options =>
{
    // fixed-window policy
    options.AddFixedWindowLimiter("rateLimitingStandardPolicy", opts =>
    {
        opts.PermitLimit = 5;
        opts.Window = TimeSpan.FromSeconds(15);
        // 5 requests every 15 seconds
        opts.QueueLimit = 2; // Requests to be delayrd
        opts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services
    .AddAuthenticationFromInfrastructure()
    .AddAuthorizationFromInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
}

app.MapGet("/health", () =>
{
    return new { Status = "OK" };
})
.WithName("Health");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.MapReverseProxy();

app.Run();
