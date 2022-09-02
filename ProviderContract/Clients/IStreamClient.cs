using ProviderContract.Data.Enums;
using System.Collections.Generic;

namespace ProviderContract.Clients
{
    internal interface IStreamClient
    {
        void OpenStream();
        void CloseStream();

        void SubscribeAggregatedTrades(IEnumerable<string> pairs);
        void SubscribeBookTickers(IEnumerable<string> pairs);
        void SubscribeKlines(IEnumerable<string> pairs, KlineTimeframe timeframe);
        void SubscribeOrderBooks(IEnumerable<string> pairs);
        void SubscribeTickers(IEnumerable<string> pairs);
        void SubscribeTicks(IEnumerable<string> pairs);
        void SubscribeUserData(IEnumerable<string> pairs);

        void UnsubscribeAggregatedTrades(IEnumerable<string> pairs);
        void UnsubscribeBookTickers(IEnumerable<string> pairs);
        void UnsubscribeKlines(IEnumerable<string> pairs);
        void UnsubscribeOrderBooks(IEnumerable<string> pairs);
        void UnsubscribeTickers(IEnumerable<string> pairs);
        void UnsubscribeTicks(IEnumerable<string> pairs);
        void UnsubscribeUserData(IEnumerable<string> pairs);
    }
}
