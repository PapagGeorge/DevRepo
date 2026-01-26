using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using WalletCore.Contracts.DBModels;
using WalletCore.DataService.Application.Interfaces.Repositories;
using WalletCore.DataService.Infrastructure;
using WalletCore.Logging;

namespace WalletCore.DataService.Repositories.Repositorues
{
    public class ExchangeRateMergeRepository : IExchangeRateMergeRepository
    {
        private readonly WalletDbContext _dbContext;
        private readonly ILogger<ExchangeRateMergeRepository> _logger;

        public ExchangeRateMergeRepository(
            WalletDbContext dbContext,
            ILogger<ExchangeRateMergeRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task MergeRatesAsync(
            IEnumerable<ExchangeRate> rates,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Merging exchange rates", b => b.WithPayload(new { Count = rates.Count() }));

                var table = new DataTable();
                table.Columns.Add("Id", typeof(Guid));
                table.Columns.Add("Date", typeof(DateTime));
                table.Columns.Add("CurrencyCode", typeof(string));
                table.Columns.Add("Rate", typeof(decimal));
                table.Columns.Add("UpdatedAt", typeof(DateTime));

                foreach (var rate in rates)
                {
                    table.Rows.Add(
                        rate.Id,
                        rate.Date.ToDateTime(TimeOnly.MinValue),
                        rate.CurrencyCode,
                        rate.Rate,
                        rate.UpdatedAt);
                }

                var param = new SqlParameter("@Rates", table)
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "ExchangeRateType"
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC MergeExchangeRates @Rates",
                    new[] { param },
                    cancellationToken: cancellationToken);

                _logger.LogInformation("Exchange rates merged successfully", b => b.WithPayload(new { Rows = table.Rows.Count }));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to merge exchange rates", ex, b => b.WithPayload(new { Count = rates.Count() }));
                throw;
            }
        }
    }
}
