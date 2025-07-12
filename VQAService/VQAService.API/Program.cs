using VQAService.Application;
using VQAService.Presentation;
using VQAService.Infrastructure;
using SharedKernel.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Host.RegisterSerilogWithSeq("vqa");
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services
    .AddInfrastructure()
    .AddApplication()
    .AddPresentation();

builder.Services.AddEndpoints();

builder.Services.AddOpenApi();

var app = builder.Build();
app.UseSerilogWithSeq();
app.UseExceptionHandler();
app.MapEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
}

app.Run();