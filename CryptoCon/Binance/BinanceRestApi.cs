using Adapter;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Spot;
using CommonDataContract;
using CommonDataContract.ReactData;
using CryptoCon.Binance.Enums;
using CryptoCon.Binance.Models;
using CryptoExchange.Net.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Binance
{
    public class BinanceRestApi
    {
        private BinanceClient _binanceClient { get; set; }

        private TypeMarket _typeMarket { get; set; }

        public BufferBlock<Order> BufferPlaceOrders { get; set; }
        public BufferBlock<CancelOrder> BufferCancelOrders { get; set; }

        public BufferBlock<BinanceOrderBook> BufferOrderBook { get; set; }

        public BufferBlock<IEnumerable<IBinanceKline>> BufferBookKlines { get; set; }
        public BufferBlock<string> BufferBookFailedKlines { get; set; }

        public BufferBlock<IEnumerable<KlineETS>> TransofmedKlinesForETS { get; set; }

        AbstractTerminal _abstractTerminal { get; set; }

        public BinanceRestApi(string key, string secretKey, TypeMarket typeMarket, AbstractTerminal abstractTerminal)
        {
            _binanceClient = new BinanceClient(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(key, secretKey),
                SpotApiOptions = new BinanceApiClientOptions()
            });

            _abstractTerminal = abstractTerminal;

            _typeMarket = typeMarket;

            BufferPlaceOrders = new BufferBlock<Order>();
            BufferCancelOrders = new BufferBlock<CancelOrder>();

            BufferOrderBook = new BufferBlock<BinanceOrderBook>();

            BufferBookKlines = new BufferBlock<IEnumerable<IBinanceKline>>();
            BufferBookFailedKlines = new BufferBlock<string>();

            TransofmedKlinesForETS = new BufferBlock<IEnumerable<KlineETS>>();
        }

        public async Task<IEnumerable<ExchangeInfo>> GetAllTickers()
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var exchangedInfo = await _binanceClient.SpotApi.ExchangeData.GetExchangeInfoAsync();

                if (exchangedInfo.Success)
                {
                    return exchangedInfo.Data.Symbols.Select(x => new ExchangeInfo()
                    {
                        Symbol = x.Name,
                        BaseAsset = x.BaseAsset,
                        QuoteAsset = x.QuoteAsset,
                        MinPrice = x.PriceFilter.MinPrice,
                        MinQuantity = x.LotSizeFilter.MinQuantity,
                        TickSize = x.PriceFilter.TickSize,
                        Status = x.Status,
                    });
                }
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var exchangedInfo = await _binanceClient.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync();

                if (exchangedInfo.Success)
                {
                    return exchangedInfo.Data.Symbols.Select(x => new ExchangeInfo()
                    {
                        Symbol = x.Name,
                        BaseAsset = x.BaseAsset,
                        QuoteAsset = x.QuoteAsset,
                        MinPrice = x.PriceFilter.MinPrice,
                        MinQuantity = x.LotSizeFilter.MinQuantity,
                        TickSize = x.PriceFilter.TickSize,
                        Status = x.Status
                    });
                }
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                var exchangedInfo = await _binanceClient.CoinFuturesApi.ExchangeData.GetExchangeInfoAsync();

                if (exchangedInfo.Success)
                {
                    return exchangedInfo.Data.Symbols.Select(x => new ExchangeInfo()
                    {
                        Symbol = x.Name,
                        BaseAsset = x.BaseAsset,
                        QuoteAsset = x.QuoteAsset,
                        MinPrice = x.PriceFilter.MinPrice,
                        MinQuantity = x.LotSizeFilter.MinQuantity,
                        TickSize = x.PriceFilter.TickSize,
                        Status = x.Status
                    });
                }
            }

            return new List<ExchangeInfo>();
        }

        public async Task<IEnumerable<BinanceBalance>> GetBalancesAccountAsync()
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var balancesRequest = await _binanceClient.SpotApi.Account.GetAccountInfoAsync();

                if (balancesRequest.Success)
                {
                    return balancesRequest.Data.Balances.Where(x => x.Available > 0).ToList();
                }
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var balancesRequest = await _binanceClient.UsdFuturesApi.Account.GetAccountInfoAsync();

                if (balancesRequest.Success)
                {
                    return balancesRequest.Data.Assets.Where(x => x.AvailableBalance > 0).Select(x => new BinanceBalance()
                    {
                        Asset = x.Asset,
                        Available = x.AvailableBalance,
                    }).ToList();
                }
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                var balancesRequest = await _binanceClient.CoinFuturesApi.Account.GetAccountInfoAsync();

                if (balancesRequest.Success)
                {
                    return balancesRequest.Data.Assets.Where(x => x.AvailableBalance > 0).Select(x => new BinanceBalance()
                    {
                        Asset = x.Asset,
                        Available = x.AvailableBalance,
                    }).ToList();
                }
            }

            return new List<BinanceBalance>();
        }

        public async Task<IEnumerable<BinanceOrder>> GetHistoryOrdesAsync(string symbol)
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var ordersHistoryRequest = await _binanceClient.SpotApi.Trading.GetOrdersAsync(symbol);
                if (ordersHistoryRequest.Success)
                {
                    return ordersHistoryRequest.Data;
                }
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var ordersHistoryRequest = await _binanceClient.UsdFuturesApi.Trading.GetOrdersAsync(symbol);
                if (ordersHistoryRequest.Success)
                {
                    return ordersHistoryRequest.Data.Select(x => new BinanceOrder()
                    {
                        ClientOrderId = x.ClientOrderId,
                        CreateTime = x.CreateTime,
                        Id = x.Id,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        Side = x.Side,
                        Status = x.Status,
                        Symbol = x.Symbol,
                        UpdateTime = x.UpdateTime,
                        StopPrice = x.StopPrice,
                        QuantityFilled = x.QuantityFilled,
                        QuoteQuantityFilled = x.QuoteQuantityFilled.HasValue ? x.QuoteQuantityFilled.Value : 0,
                        TimeInForce = x.TimeInForce
                    }).ToList();
                }
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                var ordersHistoryRequest = await _binanceClient.CoinFuturesApi.Trading.GetOrdersAsync(symbol);
                if (ordersHistoryRequest.Success)
                {
                    return ordersHistoryRequest.Data.Select(x => new BinanceOrder()
                    {
                        ClientOrderId = x.ClientOrderId,
                        CreateTime = x.CreateTime,
                        Id = x.Id,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        Side = x.Side,
                        Status = x.Status,
                        Symbol = x.Symbol,
                        UpdateTime = x.UpdateTime,
                        StopPrice = x.StopPrice,
                        QuantityFilled = x.QuantityFilled,
                        QuoteQuantityFilled = x.QuoteQuantityFilled.HasValue ? x.QuoteQuantityFilled.Value : 0,
                        TimeInForce = x.TimeInForce
                    }).ToList();
                }
            }
            return new List<BinanceOrder>();
        }

        public async Task PlaceOrderAsync(string symbol, OrderSide orderside, OrderType orderType, decimal quantity, string clientId, TimeInForce? timeInForce = null, decimal? price = null)
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var placedOrder = await _binanceClient.SpotApi.Trading.PlaceOrderAsync(symbol, orderside, (SpotOrderType)orderType, quantity: quantity, price: price, newClientOrderId: clientId, timeInForce: timeInForce);
                if (placedOrder.Success)
                {
                    BufferPlaceOrders.Post(new Order()
                    {
                        ClientOrderId = placedOrder.Data.ClientOrderId,
                        OrderSide = placedOrder.Data.Side,
                        OrderType = (OrderType)placedOrder.Data.Type,
                        Price = placedOrder.Data.Price,
                        Quantity = placedOrder.Data.Quantity,
                        Symbol = symbol.ToUpper(),
                        OrderId = placedOrder.Data.Id
                    });
                }
                else
                    _abstractTerminal.BaseMon.Raise_OnSomething($"PlaceOrderAsync: { placedOrder.Error?.Message }");
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var placedOrder = await _binanceClient.UsdFuturesApi.Trading.PlaceOrderAsync(symbol, orderside, (FuturesOrderType)orderType, quantity: quantity, price: price, newClientOrderId: clientId, timeInForce: timeInForce);
                if (placedOrder.Success)
                {
                    BufferPlaceOrders.Post(new Order()
                    {
                        ClientOrderId = placedOrder.Data.ClientOrderId,
                        OrderSide = placedOrder.Data.Side,
                        OrderType = (OrderType)placedOrder.Data.Type,
                        Price = placedOrder.Data.Price,
                        Quantity = placedOrder.Data.Quantity,
                        Symbol = symbol.ToUpper(),
                        OrderId = placedOrder.Data.Id
                    });
                }
                else
                    _abstractTerminal.BaseMon.Raise_OnSomething($"PlaceOrderAsync: { placedOrder.Error?.Message }");
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                var placedOrder = await _binanceClient.CoinFuturesApi.Trading.PlaceOrderAsync(symbol, orderside, (FuturesOrderType)orderType, quantity: quantity, price: price, newClientOrderId: clientId, timeInForce: timeInForce);
                if (placedOrder.Success)
                {
                    BufferPlaceOrders.Post(new Order()
                    {
                        ClientOrderId = placedOrder.Data.ClientOrderId,
                        OrderSide = placedOrder.Data.Side,
                        OrderType = (OrderType)placedOrder.Data.Type,
                        Price = placedOrder.Data.Price,
                        Quantity = placedOrder.Data.Quantity,
                        Symbol = symbol.ToUpper(),
                        OrderId = placedOrder.Data.Id
                    });
                }
                else
                    _abstractTerminal.BaseMon.Raise_OnSomething($"PlaceOrderAsync: { placedOrder.Error?.Message }");
            }
        }

        public async Task CancelOrderAsync(string symbol, long orderId)
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var cancelledOrder = await _binanceClient.SpotApi.Trading.CancelOrderAsync(symbol, orderId);
                if (cancelledOrder.Success)
                {
                    BufferCancelOrders.Post(new CancelOrder()
                    {
                        Symbol = cancelledOrder.Data.Symbol.ToUpper(),
                        Price = cancelledOrder.Data.Price,
                        Quantity = cancelledOrder.Data.Quantity,
                        Side = cancelledOrder.Data.Side,
                        ClientOrderId = cancelledOrder.Data.ClientOrderId,
                        CancelOrderTime = cancelledOrder.Data.UpdateTime,
                        CreateOrderTime = cancelledOrder.Data.CreateTime,
                        OrderStatus = cancelledOrder.Data.Status,
                        OrderId = cancelledOrder.Data.Id,
                        QuantityRemaining = cancelledOrder.Data.QuantityRemaining
                    });
                }
                else
                    _abstractTerminal.BaseMon.Raise_OnSomething($"CancelOrderAsync: { cancelledOrder.Error?.Message }");
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var cancelledOrder = await _binanceClient.UsdFuturesApi.Trading.CancelOrderAsync(symbol, orderId);
                if (cancelledOrder.Success)
                {
                    BufferCancelOrders.Post(new CancelOrder()
                    {
                        Symbol = cancelledOrder.Data.Symbol.ToUpper(),
                        Price = cancelledOrder.Data.Price,
                        Quantity = cancelledOrder.Data.Quantity,
                        Side = cancelledOrder.Data.Side,
                        ClientOrderId = cancelledOrder.Data.ClientOrderId,
                        CancelOrderTime = cancelledOrder.Data.UpdateTime,
                        OrderStatus = cancelledOrder.Data.Status
                    });
                }
                else
                    _abstractTerminal.BaseMon.Raise_OnSomething($"CancelOrderAsync: { cancelledOrder.Error?.Message }");
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                var cancelledOrder = await _binanceClient.CoinFuturesApi.Trading.CancelOrderAsync(symbol, orderId);
                if (cancelledOrder.Success)
                {
                    BufferCancelOrders.Post(new CancelOrder()
                    {
                        Symbol = cancelledOrder.Data.Symbol.ToUpper(),
                        Price = cancelledOrder.Data.Price,
                        Quantity = cancelledOrder.Data.Quantity,
                        Side = cancelledOrder.Data.Side,
                        ClientOrderId = cancelledOrder.Data.ClientOrderId,
                        CancelOrderTime = cancelledOrder.Data.UpdateTime,
                        OrderStatus = cancelledOrder.Data.Status
                    });
                }
                else
                    _abstractTerminal.BaseMon.Raise_OnSomething($"CancelOrderAsync: { cancelledOrder.Error?.Message }");
            }
        }

        public async Task<BinanceOrderBook> GetOrderBookAsync(string symbol, int? limit = null)
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var orderBooksRequest = await _binanceClient.SpotApi.ExchangeData.GetOrderBookAsync(symbol, limit);
                if (orderBooksRequest.Success)
                {
                    BufferOrderBook.Post(orderBooksRequest.Data);
                }
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var orderBooksRequest = await _binanceClient.UsdFuturesApi.ExchangeData.GetOrderBookAsync(symbol, limit);
                if (orderBooksRequest.Success)
                {
                    BufferOrderBook.Post(orderBooksRequest.Data);
                }
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                var orderBooksRequest = await _binanceClient.CoinFuturesApi.ExchangeData.GetOrderBookAsync(symbol, limit);
                if (orderBooksRequest.Success)
                {
                    BufferOrderBook.Post(orderBooksRequest.Data);
                }
            }

            return new BinanceOrderBook();
        }

        public async Task<IBinanceTick> GetTickerAsync(string symbol)
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var tickerRequest = await _binanceClient.SpotApi.ExchangeData.GetTickerAsync(symbol);
                if (tickerRequest.Success)
                {
                    return tickerRequest.Data;
                }
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var tickerRequest = await _binanceClient.UsdFuturesApi.ExchangeData.GetTickerAsync(symbol);
                if (tickerRequest.Success)
                {
                    return new Binance24HPrice()
                    {
                        LastPrice = tickerRequest.Data.LastPrice,
                        Symbol = tickerRequest.Data.Symbol,
                    };
                }
            }

            //if (_typeMarket.Equals(TypeMarket.CoinFutures))
            //{
            //    var tickerRequest = await _binanceClient.CoinFuturesApi.ExchangeData.GetTickersAsync();
            //    if (tickerRequest.Success)
            //    {
            //        return new Binance24HPrice()
            //        {
            //            LastPrice = tickerRequest.Data.LastPrice,
            //            Symbol = tickerRequest.Data.Symbol,
            //        };
            //    }
            //}

            return new Binance24HPrice();
        }

        public async Task<IEnumerable<IBinanceTick>> GetTickersAsync(IEnumerable<string> symbols)
        {
            var tickersRequest = await _binanceClient.SpotApi.ExchangeData.GetTickersAsync(symbols);
            if (tickersRequest.Success)
            {
                return tickersRequest.Data;
            }

            return new List<IBinanceTick>();
        }

        public async Task<TotalBalance> GetBalancesBTC()
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var balancesBTCRequests = await _binanceClient.SpotApi.Account.GetBalancesAsync(needBtcValuation: true);
                if (balancesBTCRequests.Success)
                {
                    var ticker = await GetTickerAsync("BTCUSDT");
                    if (ticker != null)
                    {
                        double usdtTotal = Convert.ToDouble(balancesBTCRequests.Data.Select(x => x.BtcValuation * ticker.LastPrice).Sum());

                        return new TotalBalance() { TotalBalanceUSDT = usdtTotal };
                    }
                }
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var balancesBTCRequests = await _binanceClient.UsdFuturesApi.Account.GetBalancesAsync();
                if (balancesBTCRequests.Success)
                {
                    var balanceUSDT = balancesBTCRequests.Data.Where(x => x.Asset.Equals("USDT")).FirstOrDefault();
                    if(balanceUSDT != null)
                    {
                        return new TotalBalance() { TotalBalanceUSDT = Convert.ToDouble(balanceUSDT.WalletBalance) };
                    }
                }
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                var balancesBTCRequests = await _binanceClient.CoinFuturesApi.Account.GetBalancesAsync();
                if (balancesBTCRequests.Success)
                {
                    // Необходимо уточнить в какой валюте выводить общий баланс!!!

                    var balanceUSDT = balancesBTCRequests.Data.Where(x => x.Asset.Equals("BTC")).FirstOrDefault();
                    if (balanceUSDT != null)
                    {
                        return new TotalBalance() { TotalBalanceUSDT = Convert.ToDouble(balanceUSDT.WalletBalance) };
                    }
                }
            }

            return new TotalBalance();
        }

        public async Task<IEnumerable<IBinanceKline>> GetKlinesAsync(string symbol, string interval, DateTime? startTime = null, DateTime? endTime = null)
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
                klineInterval = KlineInterval.FourHour;

            if (interval.Equals(CfgSourceEts.DownLoader1Day))
                klineInterval = KlineInterval.OneDay;

            if (interval.Equals(CfgSourceEts.DownLoader1Week))
                klineInterval = KlineInterval.OneWeek;

            if (interval.Equals(CfgSourceEts.DownLoader1Month))
                klineInterval = KlineInterval.OneMonth;

            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var klineRequest = await _binanceClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, klineInterval, startTime, endTime, limit: 1000);

                if (klineRequest.Success)
                    return klineRequest.Data;
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var klineRequest = await _binanceClient.UsdFuturesApi.ExchangeData.GetKlinesAsync(symbol, klineInterval, startTime, endTime, limit: 1000);

                if (klineRequest.Success)
                    return klineRequest.Data;
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                var klineRequest = await _binanceClient.CoinFuturesApi.ExchangeData.GetKlinesAsync(symbol, klineInterval, startTime, endTime, limit: 1000);

                if (klineRequest.Success)
                    return klineRequest.Data;
            }

            return new List<IBinanceKline>();
        }

        public async Task<IEnumerable<Order>> GetCurrentOpenOrdersAsync(string? symbol = null)
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var openOrdersRequest = await _binanceClient.SpotApi.Trading.GetOpenOrdersAsync(symbol);
                if (openOrdersRequest.Success)
                {
                    return openOrdersRequest.Data.Select(x => new Order()
                    {
                        ClientOrderId = x.ClientOrderId,
                        OrderSide = x.Side,
                        OrderType = (OrderType)x.Type,
                        Status = x.Status,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        Symbol = x.Symbol,
                        QuantityFilled = x.QuantityFilled,
                        OrderId = x.Id
                    });
                }
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var openOrdersRequest = await _binanceClient.UsdFuturesApi.Trading.GetOpenOrdersAsync(symbol);
                if (openOrdersRequest.Success)
                {
                    return openOrdersRequest.Data.Select(x => new Order()
                    {
                        ClientOrderId = x.ClientOrderId,
                        OrderSide = x.Side,
                        OrderType = (OrderType)x.Type,
                        Status = x.Status,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        Symbol = x.Symbol,
                        QuantityFilled = x.QuantityFilled,
                        OrderId = x.Id
                    });
                }
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                var openOrdersRequest = await _binanceClient.CoinFuturesApi.Trading.GetOpenOrdersAsync(symbol);
                if (openOrdersRequest.Success)
                {
                    return openOrdersRequest.Data.Select(x => new Order()
                    {
                        ClientOrderId = x.ClientOrderId,
                        OrderSide = x.Side,
                        OrderType = (OrderType)x.Type,
                        Status = x.Status,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        Symbol = x.Symbol,
                        QuantityFilled = x.QuantityFilled,
                        OrderId = x.Id
                    });
                }
            }

            return new List<Order>();
        }
    }
}
