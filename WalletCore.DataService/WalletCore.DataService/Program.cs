using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using WalletCore.Contrtacts.AdjustBalance;
using WalletCore.Contrtacts.CreateWallet;
using WalletCore.Contrtacts.DBModels;
using WalletCore.DataService.Infrastructure;
using WalletCore.DataService.Infrastructure.Configuration;
using WalletCore.DataService.Infrastructure.Interfaces;

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
                    IWalletRepository walletRepository) =>
                {
                    var wallet = await walletRepository.GetByIdAsync(id);

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
                IWalletRepository walletRepository) =>
                {
                    var wallet = new Wallet
                    {
                        Id = Guid.NewGuid(),
                        Currency = request.Currency,
                        Balance = 0m
                    };

                    await walletRepository.AddAsync(wallet);

                    return Results.Created($"/wallet/{wallet.Id}", new CreateWalletResponse
                    {
                        WalletId = wallet.Id,
                        IsSuccessful = true,
                        Message = $"Wallet with {wallet.Id} created syccessfully"
                    });
                });

                app.MapPost("/wallet/balance", async (
                AdjustBalanceRequestDto request,
                IWalletRepository walletRepository) =>
                {
                    await walletRepository.UpdateBalanceAsync(request.Wallet, request.NewBalance);

                    return Results.Created($"/wallet/balance", new AdjustBalanceResponse
                    {
                        WalletId = request.Wallet.Id,
                        IsSuccessful = true,
                        NewBalance = request.NewBalance,
                        WalletCurrency = request.Wallet.Currency
                    });
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
