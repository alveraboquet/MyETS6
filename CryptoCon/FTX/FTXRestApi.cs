using CommonDataContract;
using CryptoCon.FTX.Enums;
using CryptoCon.FTX.Models;
using CryptoExchange.Net.Authentication;
using FTX.Net.Clients;
using FTX.Net.Enums;
using FTX.Net.Objects;
using FTX.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.FTX
{
    public class FTXRestApi
    {
        private FTXClient _ftxClient { get; set; }

        private TypeMarket _typeMarket { get; set; }

        public BufferBlock<IEnumerable<Kline>> BubberKlines { get; set; }
        private BufferBlock<string> _bufferKlineFailed { get; set; }


        public BufferBlock<FTXOrder> BufferPlaceOrders { get; set; }
        public BufferBlock<CancelOrder> BufferCancelOrders { get; set; }

        public FTXRestApi(string key, string secretKey, TypeMarket typeMarket)
        {
            _ftxClient = new FTXClient(new FTXClientOptions()
            {
                ApiCredentials = new ApiCredentials(key, secretKey)
            });

            _typeMarket = typeMarket;

            BubberKlines = new BufferBlock<IEnumerable<Kline>>();
            _bufferKlineFailed = new BufferBlock<string>();

            BufferPlaceOrders = new BufferBlock<FTXOrder>();
            BufferCancelOrders = new BufferBlock<CancelOrder>();
        }

        public async Task<IEnumerable<FTXSymbol>> GetAllTickers()
        {
            var instrumentsRequest = await _ftxClient.TradeApi.ExchangeData.GetSymbolsAsync();

            if (instrumentsRequest.Success)
            {
                return instrumentsRequest.Data;
            }

            return new List<FTXSymbol>();
        }

        public async Task PlaceOrderAsync(string symbol, OrderSide orderSide, OrderType orderType, decimal quantity, decimal price, string clientId)
        {
            //if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var placedOrder = await _ftxClient.TradeApi.Trading.PlaceOrderAsync(symbol, orderSide, orderType, quantity: quantity, price: price, clientOrderId: clientId);
                if (placedOrder.Success)
                {
                    BufferPlaceOrders.Post(placedOrder.Data);
                }

            }
        }

        public async Task CancelOrderAsync(long orderId)
        {
            //if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var cancelledOrder = await _ftxClient.TradeApi.Trading.CancelOrderAsync(orderId);
                if (cancelledOrder.Success)
                {
                    BufferCancelOrders.Post(new CancelOrder() { CancelOrderId = orderId });
                }
            }
        }

        public async Task<IEnumerable<FTXBalance>> GetBalanceAsync()
        {
            var balancesRequests = await _ftxClient.TradeApi.Account.GetBalancesAsync();
            if (balancesRequests.Success)
            {
                return balancesRequests.Data.Where(x => x.Available > 0).ToList();
            }

            return new List<FTXBalance>();
        }

        public async Task<TotalBalanceFTX> GetBalanceUSD()
        {
            var balancesRequests = await _ftxClient.TradeApi.Account.GetBalancesAsync();
            if (balancesRequests.Success)
            {
                var totalUSD = balancesRequests.Data.Where(x => x.Available > 0).Select(x => x.UsdValue).Sum();

                return new TotalBalanceFTX() { TotalBalanceUSDT = totalUSD };
            }

            return new TotalBalanceFTX();
        }

        public async Task<IEnumerable<FTXOrder>> GetCurrentOpenOrdersAsync(string? symbol = null)
        {
            var openOrdersRequest = await _ftxClient.TradeApi.Trading.GetOpenOrdersAsync(symbol);
            if (openOrdersRequest.Success)
            {
                return openOrdersRequest.Data;
            }

            return new List<FTXOrder>();
        }

        public async Task<IEnumerable<FTXOrder>> GetHistoryOrdersAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            var historyOrdersRequest = await _ftxClient.TradeApi.Trading.GetOrdersAsync(symbol, startTime, endTime);
            if (historyOrdersRequest.Success)
            {
                return historyOrdersRequest.Data;
            }

            return new List<FTXOrder>();
        }

        public async Task<IEnumerable<FTXKline>> GetKlinesAsync(string symbol, string interval, DateTime? startTime = null, DateTime? endTime = null)
        {
            KlineInterval klineInterval = KlineInterval.OneMinute;

            if (interval.Equals(CfgSourceEts.DownLoader1Minutes))
                klineInterval = KlineInterval.OneMinute;

            if (interval.Equals(CfgSourceEts.DownLoader5Minutes))
                klineInterval = KlineInterval.FiveMinutes;

            if (interval.Equals(CfgSourceEts.DownLoader15Minutes))
                klineInterval = KlineInterval.FifteenMinutes;

            if (interval.Equals(CfgSourceEts.DownLoader1Hour))
                klineInterval = KlineInterval.OneHour;

            if (interval.Equals(CfgSourceEts.DownLoader4Hour))
                klineInterval = KlineInterval.FourHours;

            if (interval.Equals(CfgSourceEts.DownLoader1Day))
                klineInterval = KlineInterval.OneDay;

            if (interval.Equals(CfgSourceEts.DownLoader1Week))
                klineInterval = KlineInterval.OneWeek;

            if (interval.Equals(CfgSourceEts.DownLoader1Month))
                klineInterval = KlineInterval.OneMonth;

            var klineRequest = await _ftxClient.TradeApi.ExchangeData.GetKlinesAsync(symbol, klineInterval, startTime, endTime);
            if (klineRequest.Success)
            {
                return klineRequest.Data;
            }

            return new List<FTXKline>();
        }
    }
}
