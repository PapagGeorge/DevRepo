using System;

namespace WalletCore.Contrtacts.AdjustBalance
{
    public class AdjustBalanceRequest
    {
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public string AmountCurrency { get; set; }
        public WalletStrategyOperation AdjustmentStrategy { get; set; }
    }
}
