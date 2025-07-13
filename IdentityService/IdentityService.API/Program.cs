using IdentityService.Application;
using IdentityService.Infrastructure;
using IdentityService.Presentation;
using SharedKernel.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Host.RegisterSerilogWithSeq("identity");
builder.Services
    .AddInfrastructure()
    .AddApplication()
    .AddPresentation();

builder.Services.AddEndpoints();

builder.Services.AddOpenApi();

var app = builder.Build();
app.UseSerilogWithSeq();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });

    app.ApplyMigrations();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();

app.Run();