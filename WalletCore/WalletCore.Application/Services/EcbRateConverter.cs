using WalletCore.Application.Interfaces;
using WalletCore.Domain.Models.EcbRateConverter;
using WalletCore.Domain.Models.GetDailyRates;

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

            var envelope = await _ecbService.GetDailyRatesAsync();
            var rateDict = BuildRateDictionary(envelope);

            decimal fromRate = rateDict[request.FromCurrency];
            decimal toRate = rateDict[request.ToCurrency];

            decimal amountInEur = request.Amount / fromRate;
            decimal converted = amountInEur * toRate;

            return new CurrencyConversionResponse
            {
                ConvertedAmount = Math.Round(converted, 8)
            };
        }

        private static Dictionary<string, decimal> BuildRateDictionary(GesmesEnvelope envelope)
        {
            var dict = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                ["EUR"] = 1.0m
            };

            var latestCube = envelope.Cube.TimeCubes.First();

            foreach (var rate in latestCube.Rates)
            {
                dict[rate.Currency] = rate.Rate;
            }

            return dict;
        }
    }
}
