using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using WalletCore.Contrtacts.DBModels;
using WalletCore.DataService.Infrastructure;
using WalletCore.DataService.Infrastructure.Interfaces;
using WalletCore.DataService.Logging;

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
                _logger.LogInfoExt(
                    "Merging exchange rates",
                    enrich: b => b.WithPayload(new
                    {
                        Count = rates.Count()
                    }));

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

                _logger.LogInfoExt(
                    "Exchange rates merged successfully",
                    enrich: b => b.WithPayload(new
                    {
                        Rows = table.Rows.Count
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogErrorExt(
                    "Failed to merge exchange rates",
                    ex);

                throw;
            }
        }
    }
}
