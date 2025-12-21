using System;

namespace WalletCore.Contrtacts.GetBalance
{
    public class GetBalanceRequest
    {
        public Guid WalletId { get; set; }
        public string ConvertToCurrency { get; set; }
    }
}
