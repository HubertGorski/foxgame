using Hub.Identity.Interfaces;
using Hub.Identity.Services;
using FoxTales.Composition;
using FoxTales.Application.Interfaces;
using FoxTales.Application.Services;
using FoxTales.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDylematyService, DylematyService>();
builder.Services.AddAutoMapper(cfg => 
{
    cfg.AddProfile<DylematyCardProfile>();
});
builder.Services.AddControllers();
await builder.Services.SeedDatabaseAsync();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();