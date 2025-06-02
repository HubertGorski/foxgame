using Hub.Identity.Interfaces;
using Hub.Identity.Services;
using FoxTales.Composition;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddControllers();
await builder.Services.SeedDatabaseAsync();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();