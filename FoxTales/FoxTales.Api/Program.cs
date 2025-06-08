using FoxTales.Composition;
using FoxTales.Application.Interfaces;
using FoxTales.Application.Services;
using FoxTales.Application.Mappings;
using FoxTales.Api.Middleware;
using FoxTales.Api.Platform;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

await builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDylematyService, DylematyService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<DylematyCardProfile>();
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<UserCardProfile>();
});

builder.Services.AddScoped<ErrorHandlingMiddleware>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();