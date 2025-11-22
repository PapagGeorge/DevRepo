using Microsoft.Extensions.Options;
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
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<ExceptionHandler>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ExceptionHandler>>();
                return new ExceptionHandler(logger, typeof(WalletException));
            });

            // Add services to the container.
            builder.Services.Configure<ECBClientConfig>(builder.Configuration.GetSection("ECBConfig"));
            builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("ConnectionStrings"));
            builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("Redis"));

            builder.Services.AddControllers();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration, clients =>
            {
                var sp = builder.Services.BuildServiceProvider();

                clients.AddClient(HttpClientsRegistry.ECBClientRegistry(sp.GetRequiredService<IOptions<ECBClientConfig>>()));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
