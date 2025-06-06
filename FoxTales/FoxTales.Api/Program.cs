using FoxTales.Composition;
using FoxTales.Application.Interfaces;
using FoxTales.Application.Services;
using FoxTales.Application.Mappings;
using NLog.Web;
using FoxTales.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddInfrastructure(builder.Configuration);
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
await builder.Services.SeedDatabaseAsync();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();