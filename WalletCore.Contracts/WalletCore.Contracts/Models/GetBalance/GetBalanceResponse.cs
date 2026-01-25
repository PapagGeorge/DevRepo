using System;

namespace WalletCore.Contracts.GetBalance
{
    public class GetBalanceResponse
    {
        public Guid WalletId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}
