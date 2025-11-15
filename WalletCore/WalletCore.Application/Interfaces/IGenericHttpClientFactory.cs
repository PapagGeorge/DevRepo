using WalletCore.Application.HttpClientInfrastructure;

namespace WalletCore.Application.Interfaces
{
    public interface IGenericHttpClientFactory
    {
        GenericHttpClient CreateClient(string name);
    }
}
