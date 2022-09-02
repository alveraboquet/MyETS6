using CommonDataContract;
using CommonDataContract.ReactData;
using CryptoCon.Huobi.Enums;
//using CryptoCon.Extension;
using CryptoCon.Huobi.Models;
using CryptoExchange.Net.Authentication;
using Huobi.Net.Clients;
using Huobi.Net.Enums;
using Huobi.Net.Objects;
using Huobi.Net.Objects.Models;
using HuobiFutures.Clients;
using HuobiFutures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Huobi.Clients
{
    public class HuobiRestAPI
    {
        private HuobiClient _huobiClient { get; set; }
        private HuobiFuturesClient _huobiFuturesClient { get; set; }

        public long UserId { get; set; } = 0;

        public BufferBlock<Order> BufferPlaceOrder { get; set; }
        public BufferBlock<long> BufferCancelOrder { get; set; }

        private TypeMarket _typeMarket { get; set; }

        public HuobiRestAPI(string key, string secretKey, TypeMarket typeMarket)
        {
            _huobiClient = new HuobiClient(new HuobiClientOptions() { ApiCredentials = new ApiCredentials(key, secretKey) });
            _huobiFuturesClient = new HuobiFuturesClient(new HuobiFuturesClientOptions() { ApiCredentials = new ApiCredentials(key, secretKey) });

            BufferPlaceOrder = new BufferBlock<Order>();
            BufferCancelOrder = new BufferBlock<long>();

            _typeMarket = typeMarket;
        }

        public async Task<List<Securities>> GetAllTickers()
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var symbols = await _huobiClient.SpotApi.ExchangeData.GetSymbolsAsync();
                if (symbols.Success)
                {
                    var symbolsOnline = symbols.Data.Where(x => x.State.Equals(SymbolState.Online)).ToList();

                    var tickers = symbolsOnline.Select(x => new Securities()
                    {
                        Seccode = x.Name.ToUpper(),
                        ClassCode = "Huobi",
                        IsCrypto = true,
                        BaseActive = x.QuoteAsset.ToUpper(),
                        ShortName = x.BaseAsset.ToUpper(),
                        TradingStatus = x.State.Equals(SymbolState.Online) ? "Торгуется" : "Не торгуется",
                        Status = x.ApiTrading.Equals("enabled") ? "Открыта" : "Закрыта",
                        Accuracy = x.PricePrecision,
                        LotSize = Convert.ToDouble(x.MinLimitOrderQuantity),
                        MinStep = Math.Pow(10, -x.PricePrecision),
                        PointCost = Math.Pow(10, -x.PricePrecision),
                        MinAmount = Math.Pow(10, -x.ValuePrecision),
                        IsTrade = true

                    }).Where(ticker => !String.IsNullOrEmpty(ticker.Seccode) && !String.IsNullOrEmpty(ticker.BaseActive)).ToList();

                    return tickers;
                }
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var symbols = await _huobiFuturesClient.FuturesApi.ExchangeData.GetTickersAsync();
                if (symbols.Success)
                {
                    var tickers = symbols.Data.Info.Select(x => new Securities()
                    {
                        Seccode = x.ConstractCode,
                        ClassCode = "Huobi",
                        IsCrypto = true,
                        BaseActive = "Фьючерсы",
                        ShortName = x.Symbol,
                        TradingStatus = "Торгуется",
                        Status = "Открыта",
                        Accuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(x.PriceTick))),
                        LotSize = Convert.ToDouble(x.ContractSize),
                        MinStep = Convert.ToDouble(x.PriceTick),
                        PointCost = Convert.ToDouble(x.PriceTick),
                        MinAmount = Convert.ToDouble(x.ContractSize),
                        IsTrade = true
                    }).Where(ticker => !String.IsNullOrEmpty(ticker.Seccode) && !String.IsNullOrEmpty(ticker.BaseActive)).ToList();

                    return tickers;
                }
            }

            return new List<Securities>();
        }

        public async Task<IEnumerable<HuobiFuturesKline>> GetKlinesAsync(string symbol, string interval, DateTime? startTime = null, DateTime? endTime = null)
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

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var klinesRequest = await _huobiFuturesClient.FuturesApi.ExchangeData.GetKlinesAsync(symbol, (HuobiFutures.Enums.KlineInterval)klineInterval, 
                    startTime, endTime, limit: 2000);

                if (klinesRequest.Success)
                {
                    return klinesRequest.Data;
                }
            }

            return new List<HuobiFuturesKline>();
        }

        public async Task<long> GetUserIdAsync()
        {
            if (UserId == 0)
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    var user = await _huobiClient.SpotApi.Account.GetAccountsAsync();
                    if (user.Success)
                    {
                        return user.Data.First().Id;
                    }

                    await Task.Delay(200);
                }
            }

            return UserId;
        }

        public async Task<IEnumerable<HuobiOpenOrder>> GetOpenOrders(string? symbol = null, int? limit = null)
        {
            long userId = await GetUserIdAsync();

            var openOrders = await _huobiClient.SpotApi.Trading.GetOpenOrdersAsync(userId, limit: limit);
            if (openOrders.Success)
            {
                return openOrders.Data;
            }

            return new List<HuobiOpenOrder>();
        }

        public async Task<IEnumerable<HuobiOrder>> GetHistoryOrdesAsync(string? symbol = null, int? limit = null)
        {
            var hisoryOrders = await _huobiClient.SpotApi.Trading.GetHistoricalOrdersAsync(symbol, limit: limit);
            if (hisoryOrders.Success)
            {
                return hisoryOrders.Data;
            }

            return new List<HuobiOrder>();
        }

        public async Task<IEnumerable<HuobiKline>> GetKlinesAsync(string symbol, KlineInterval klineInterval, int? limit = null)
        {
            var klines = await _huobiClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, klineInterval, limit);
            if (klines.Success)
            {
                return klines.Data;
            }

            return new List<HuobiKline>();
        }

        public async Task PlaceOrderAsync(string symbol, OrderSide orderSide, OrderType orderType, decimal quantity, decimal price, string? clientOrderId = null)
        {
            long userId = await GetUserIdAsync();

            var placedOrder = await _huobiClient.SpotApi.Trading.PlaceOrderAsync(userId, symbol, orderSide, orderType, quantity: quantity, price: price, clientOrderId: clientOrderId);
            if (placedOrder.Success)
            {
                BufferPlaceOrder.Post(new Order()
                {
                    ClientOrderId = clientOrderId,
                    OrderSide = orderSide,
                    OrderType = orderType,
                    Price = price,
                    Quantity = quantity,
                    Symbol = symbol.ToUpper(),
                    OrderId = placedOrder.Data
                });
            }
        }

        public async Task<long> CancelOrderAsync(long orderId)
        {
            var cancelOrder = await _huobiClient.SpotApi.Trading.CancelOrderAsync(orderId);

            if (cancelOrder.Success)
            {
                return cancelOrder.Data;
            }

            return 0;
        }
    }
}
