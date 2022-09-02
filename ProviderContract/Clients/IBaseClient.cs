using ProviderContract.Data;

namespace ProviderContract.Clients
{
    internal interface IBaseClient
    {
        Response<T> SendRequest<T>(Request request);
        void DoSign(ref Request request);
        string ToPair(string symbol);
        string ToSymbol(string pair);
    }
}
