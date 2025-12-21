using System;

namespace WalletCore.Contrtacts.Models
{
    public class Request<T>
    {
        public Guid TransactionId { get; set; }
        public T Payload { get; set; }
    }
}
