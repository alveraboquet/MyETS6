using CommonDataContract.ReactData;
using ProviderContract.Data.Entities;
using ProviderContract.Data.Enums;
using System;
using System.Collections.Generic;

namespace ProviderContract.Clients
{
    internal interface IAPIClient
    {
        List<Tick> GetAggregatedTrades(IEnumerable<string> pairs, int limit);
        List<SecPosition> GetBalance(IEnumerable<string> pairs);
        List<Kline> GetKlines(IEnumerable<string> pairs, KlineTimeframe timeframe, DateTime from, DateTime to);
        List<OrderBook> GetOrderBooks(IEnumerable<string> pairs, int depth = 50);
        List<Orders> GetOrders(IEnumerable<string> pairs);
        List<Securities> GetPairs(IEnumerable<string> pairs = null);
        long GetServerTime();
        List<Securities> GetTickers(IEnumerable<string> pairs);
        List<Tick> GetTicks(IEnumerable<string> pairs, int limit);
        List<Deal> GetTrades(IEnumerable<string> pairs);
    }
}
