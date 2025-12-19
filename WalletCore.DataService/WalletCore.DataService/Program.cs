using Microsoft.AspNetCore.Builder;
using Serilog;
using WalletCore.DataService.Infrastructure.Configuration;
using WalletCore.DataService.Infrastructure;

namespace WalletCore.DataService
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
                        .Enrich.WithProperty("Application", "WalletCore.DataService");
                    Serilog.Debugging.SelfLog.Enable(Console.Error);
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

                builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("ConnectionStrings"));

                // Add services
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddInfrastructure();

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

                app.UseCors("AllowAll");
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                Log.Information("WalletCore.DataService application started successfully");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "WalletCore.DataService unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}