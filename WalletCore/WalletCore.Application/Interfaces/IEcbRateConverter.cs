using WalletCore.Contracts.EcbRateConverter;

namespace WalletCore.Application.Interfaces
{
    public interface IEcbRateConverter
    {
        Task<CurrencyConversionResponse> ConvertAsync(CurrencyConversionRequest request);
    }
}
