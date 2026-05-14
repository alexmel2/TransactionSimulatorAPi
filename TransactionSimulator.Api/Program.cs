using TransactionSimulator.Application;
using TransactionSimulator.Domain.Config;
using TransactionSimulator.Infrastructure;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
// 1. SETUP LOGGING (Serilog)
// Reads from "Serilog" section in appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
try
{
    Log.Information("Starting Shva Simulator API...");
    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.Configure<AppSettings>(builder.Configuration);

    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication(builder.Configuration);
    var appSettings = builder.Configuration.Get<AppSettings>();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(appSettings.CorsConfig.PolicyName,
            policy => policy.WithOrigins(appSettings.CorsConfig.AllowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader());
    });



    var app = builder.Build();
    // 1. SETUP LOGGING (Serilog)
    // Reads from "Serilog" section in appsettings.json

    app.UseCors(appSettings?.CorsConfig?.PolicyName);

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}