using System;

namespace WalletCore.Contrtacts.DBModels
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}
