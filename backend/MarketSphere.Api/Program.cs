using MarketSphere.Api.Extensions;
using MarketSphere.Application;
using MarketSphere.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiAuthentication(builder.Configuration)
    .AddApiAuthorization()
    .AddLocalCors(builder.Configuration)
    .AddApiOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.ApplyDatabaseAsync();
}

app.UseApiPipeline();

app.MapApiFoundation();
app.MapControllers();

app.Run();

public partial class Program;
