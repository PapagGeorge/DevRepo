namespace WalletCore.Domain.Models
{
    public class Request<T>
    {
        public T Payload { get; set; }
    }
}
