using FluentValidation;
using FluentValidation.AspNetCore;

using Serilog;
using System.Text;
using TransactionSimulator.Api.Validators;
using TransactionSimulator.Application;
using TransactionSimulator.Domain.Config;
using TransactionSimulator.Domain.Interfaces;
using TransactionSimulator.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Serilog Setup ---
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Shva Simulator API...");

  

    // --- 4. Service Registrations ---
    builder.Services.Configure<AppSettings>(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication(builder.Configuration);

    var appSettings = builder.Configuration.Get<AppSettings>();

    #region CorsConfig
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(appSettings.CorsConfig.PolicyName,
            policy => policy.WithOrigins(appSettings.CorsConfig.AllowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader());
    });
    #endregion

    #region FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<TransactionRequestValidator>();
    #endregion

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // --- 5. Middleware Pipeline ---
    app.UseCors(appSettings?.CorsConfig?.PolicyName);

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication(); // Identification
    app.UseAuthorization();  // Permissions

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