using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.DBModels;

namespace WalletCore.Infrastructure.Repositories
{
    public class ExchangeRateMergeRepository : IExchangeRateRepository
    {
        private readonly ExchangeRateDbContext _dbContext;

        public ExchangeRateMergeRepository(ExchangeRateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task MergeRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken cancellationToken = default)
        {
            // Convert to DataTable for TVP
            var table = new DataTable();
            table.Columns.Add("Id", typeof(Guid));
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("CurrencyCode", typeof(string));
            table.Columns.Add("Rate", typeof(decimal));
            table.Columns.Add("UpdatedAt", typeof(DateTime));

            foreach (var rate in rates)
                table.Rows.Add(rate.Id, rate.Date.ToDateTime(TimeOnly.MinValue), rate.CurrencyCode, rate.Rate, rate.UpdatedAt);

            var param = new SqlParameter("@Rates", table)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "ExchangeRateType"
            };

            await _dbContext.Database.ExecuteSqlRawAsync("EXEC MergeExchangeRates @Rates", new[] { param }, cancellationToken: cancellationToken);
        }
    }
}
