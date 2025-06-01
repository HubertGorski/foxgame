using FoxTales.Infrastructure;
using FoxTales.Infrastructure.Data.Seeders;
using Hub.Identity.Interfaces;
using Hub.Identity.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<AchievementSeeder>();
builder.Services.AddControllers();
await builder.Services.SeedDatabaseAsync();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();