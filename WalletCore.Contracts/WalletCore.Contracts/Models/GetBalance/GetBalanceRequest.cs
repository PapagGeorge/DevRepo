using System;

namespace WalletCore.Contracts.GetBalance
{
    public class GetBalanceRequest
    {
        public Guid WalletId { get; set; }
        public string ConvertToCurrency { get; set; }
    }
}
