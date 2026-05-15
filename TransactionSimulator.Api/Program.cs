using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using TransactionSimulator.Api.Validators;
using TransactionSimulator.Application;
using TransactionSimulator.Domain.Config;
using TransactionSimulator.Infrastructure;
var builder = WebApplication.CreateBuilder(args);

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
    var app = builder.Build();
  
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