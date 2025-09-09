using FoxTales.Composition;
using FoxTales.Api.Middleware;
using FoxTales.Api.Platform;
using FluentValidation.AspNetCore;
using FoxTales.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

builder.Configuration.AddEnvironmentVariables();
await builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestMiddleware>();

builder.Services.AddControllers().AddFluentValidation(); //TODO: Zmienic na nowsze

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueCorsPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:4173",
                "https://foxtales.cc",
                "https://www.foxtales.cc"
            )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fox Tales v1");
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestMiddleware>();
app.UseRouting();
app.UseCors("VueCorsPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<PsychHub>("/psychhub");

app.Urls.Add("http://0.0.0.0:5161");
await app.RunAsync();