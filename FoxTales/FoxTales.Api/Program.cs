using Hub.Identity.Interfaces;
using Hub.Identity.Services;
using FoxTales.Composition;
using FoxTales.Domain.Interfaces;
using FoxTales.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDylematyService, DylematyService>();
builder.Services.AddControllers();
await builder.Services.SeedDatabaseAsync();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();