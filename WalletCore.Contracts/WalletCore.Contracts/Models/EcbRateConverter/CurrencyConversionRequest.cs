namespace WalletCore.Contracts.EcbRateConverter
{
    public class CurrencyConversionRequest
    {
        public decimal Amount { get; init; }
        public string FromCurrency { get; init; }
        public string ToCurrency { get; init; }
    }
}
