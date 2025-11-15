namespace WalletCore.Domain.DBModels
{
    public class ExchangeRate
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Rate { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
