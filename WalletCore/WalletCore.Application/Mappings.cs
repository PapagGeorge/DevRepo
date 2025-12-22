using WalletCore.Contrtacts.AdjustBalance;
using WalletCore.Contrtacts.CommandContracts;
using WalletCore.Contrtacts.DBModels;
using WalletCore.Contrtacts.GetDailyRates;

namespace WalletCore.Application
{
    public static class Mappings
    {
        public static string ToMessage(this WalletStrategyOperation strategy)
        {
            return strategy switch
            {
                WalletStrategyOperation.AddFunds => "Funds were successfully added.",
                WalletStrategyOperation.SubtractFunds => "Funds were successfully subtracted.",
                WalletStrategyOperation.ForceSubtractFunds => "Funds were forcefully subtracted.",
                _ => "Unknown operation."
            };
        }

        public static List<ExchangeRate> ParseRates(this GesmesEnvelope xml)
        {
            var timeCube = xml.Cube.TimeCubes.First(); // Daily rates → 1 element
            var date = DateOnly.Parse(timeCube.Time);

            return timeCube.Rates.Select(r =>
                new ExchangeRate
                {
                    Date = date,
                    CurrencyCode = r.Currency,
                    Rate = r.Rate,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();
        }

        public static ExchangeRateDto ToDataServiceRequest(this ExchangeRate r)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var now = DateTime.Now;

            return new ExchangeRateDto(
                Guid.NewGuid(),
                today,
                r.CurrencyCode,
                r.Rate,
                now);
        }
    }
}
