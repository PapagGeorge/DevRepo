using WalletCore.Application.Interfaces;
using WalletCore.Contrtacts.DBModels;
using WalletCore.Contrtacts.EcbRateConverter;

namespace WalletCore.Application.Services
{
    public class EcbRateConverter : IEcbRateConverter
    {
        private readonly IEcbService _ecbService;

        public EcbRateConverter(IEcbService ecbService)
        {
            _ecbService = ecbService;
        }

        public async Task<CurrencyConversionResponse> ConvertAsync(CurrencyConversionRequest request)
        {
            // Same currency → no conversion needed
            if (string.Equals(request.FromCurrency, request.ToCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return new CurrencyConversionResponse { ConvertedAmount = request.Amount };
            }

            var exchangeRates = await _ecbService.GetDailyRatesAsync();
            var rateDict = BuildRateDictionary(exchangeRates);

            decimal fromRate = rateDict[request.FromCurrency];
            decimal toRate = rateDict[request.ToCurrency];

            decimal amountInEur = request.Amount / fromRate;
            decimal converted = amountInEur * toRate;

            return new CurrencyConversionResponse
            {
                ConvertedAmount = Math.Round(converted, 8)
            };
        }

        private static Dictionary<string, decimal> BuildRateDictionary(List<ExchangeRate> exchangeRates)
        {
            var dict = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                ["EUR"] = 1.0m
            };

            foreach (var rate in exchangeRates)
            {
                dict[rate.CurrencyCode] = rate.Rate;
            }

            return dict;
        }
    }
}
