using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using WalletCore.Contracts.AdjustBalance;
using WalletCore.Contracts.CreateWallet;
using WalletCore.DataService.Application;
using WalletCore.DataService.Application.Interfaces;
using WalletCore.DataService.Infrastructure;
using WalletCore.DataService.Infrastructure.Configuration;

namespace WalletCore.DataService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting WalletCore.DataService");

                var builder = WebApplication.CreateBuilder(args);

                // ----------------------------
                // Serilog
                // ----------------------------
                builder.Host.UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                        .Enrich.WithProperty("Application", "WalletCore.DataService");
                });

                // ----------------------------
                // CORS (internal service)
                // ----------------------------
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader());
                });

                // ----------------------------
                // Configuration
                // ----------------------------
                builder.Services.Configure<DatabaseOptions>(
                    builder.Configuration.GetSection("ConnectionStrings"));

                builder.Services.Configure<RedisOptions>(
                    builder.Configuration.GetSection("Redis"));

                // ----------------------------
                // Infrastructure (EF + MassTransit + Repositories)
                // ----------------------------
                builder.Services.AddInfrastructure(builder.Configuration);

                // ----------------------------
                // Application (Services)
                // ----------------------------
                builder.Services.AddApplication();

                var app = builder.Build();

                // ----------------------------
                // Middleware
                // ----------------------------
                app.UseSerilogRequestLogging();
                app.UseCors("AllowAll");

                // ----------------------------
                // Minimal API
                // ----------------------------
                app.MapGet("/wallets/{id:guid}", async (
                    Guid id,
                    IWalletService walletService) =>
                {
                    var wallet = await walletService.GetByIdAsync(id);

                    return wallet is null
                        ? Results.NotFound()
                        : Results.Ok(new
                        {
                            wallet.Id,
                            wallet.Balance,
                            wallet.Currency
                        });
                });

                app.MapPost("/wallet", async (
                    CreateWalletRequest request,
                    IWalletService walletService) =>
                {
                    var response = await walletService.CreateWalletAsync(request);

                    return Results.Created($"/wallet/{response.WalletId}", response);
                });

                app.MapPost("/wallet/balance", async (
                    AdjustBalanceRequestDto request,
                    IWalletService walletService) =>
                {
                    var response = await walletService.AdjustBalanceAsync(request);

                    return Results.Created($"/wallet/balance", response);
                });

                Log.Information("WalletCore.DataService started successfully");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "WalletCore.DataService terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
