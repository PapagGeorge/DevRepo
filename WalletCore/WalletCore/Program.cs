using Microsoft.Extensions.Options;
using Serilog;
using WalletCore.Application;
using WalletCore.Application.Configuration;
using WalletCore.Application.HttpClientInfrastructure;
using WalletCore.Domain.Exceptions;
using WalletCore.Infrastructure;
using WalletCore.Infrastructure.Configuration;

namespace WalletCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create bootstrap logger for startup errors
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting WalletCore application");

                var builder = WebApplication.CreateBuilder(args);

                // Configure Serilog from appsettings.json
                builder.Host.UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                        .Enrich.WithProperty("Application", "WalletCore");
                });

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
                });

                builder.Services.AddSingleton<ExceptionHandler>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<ExceptionHandler>>();
                    return new ExceptionHandler(logger, typeof(WalletCore.Domain.Exceptions.WalletException.BusinessException));
                });

                // Configure options
                builder.Services.Configure<ECBClientConfig>(builder.Configuration.GetSection("ECBConfig"));
                builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("ConnectionStrings"));
                builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("Redis"));

                builder.Services.AddControllers();
                builder.Services.AddInfrastructure(builder.Configuration);
                builder.Services.AddApplicationServices(builder.Configuration);

                var app = builder.Build();

                // Add Serilog request logging middleware
                app.UseSerilogRequestLogging(options =>
                {
                    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                    {
                        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                    };
                });

                // Configure the HTTP request pipeline
                app.UseCors("AllowAll");
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                Log.Information("WalletCore application started successfully");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
