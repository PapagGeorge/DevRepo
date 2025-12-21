using System;

namespace WalletCore.Contrtacts.CreateWallet
{
    public class CreateWalletResponse
    {
        public Guid WalletId { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}
