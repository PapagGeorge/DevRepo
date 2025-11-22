namespace WalletCore.Domain.Models.AdjustBalance
{
    public class AdjustBalanceRequest
    {
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public string AmountCurrency { get; set; }
        public string StrategyName { get; set; }
    }
}
