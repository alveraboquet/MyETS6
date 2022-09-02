using Adapter;
using Adapter.Config;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Futures.Socket;
using Binance.Net.Objects.Models.Spot;
using Binance.Net.Objects.Models.Spot.Socket;
using CommonDataContract;
using CommonDataContract.ReactData;
using CryptoCon.Binance.Enums;
using CryptoCon.Binance.Models;
using SourceEts;
using SourceEts.Models.TimeFrameTransformModel;
using SourceEts.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Binance
{
    public class BinanceWrapper
    {
        private BinanceSocket _binanceSocket { get; set; }
        private BinanceRestApi _binanceRestApi { get; set; }

        private string Key { get; set; }
        private string SecretKey { get; set; }

        public AbstractTerminal _abstractTerminal { get; set; }

        private TypeMarket _typeMarket { get; set; }

        public BinanceWrapper(string key, string secretKey, TypeMarket typeMarket, AbstractTerminal abstractTerminal)
        {
            Key = key;
            SecretKey = secretKey;

            _binanceRestApi = new BinanceRestApi(key, secretKey, typeMarket, abstractTerminal);
            _binanceSocket = new BinanceSocket(key, secretKey, typeMarket);

            _abstractTerminal = abstractTerminal;

            _typeMarket = typeMarket;
        }

        public async Task SubscribeToAccountUpdateAsync()
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                await _binanceSocket.SubscribeToUserAccountUpdatesAsync();

                var tranferToTerminalOrders = new ActionBlock<BinanceStreamOrderUpdate>(x =>
                {
                    Orders order = null;

                    if (x.Status.Equals(OrderStatus.New))
                    {
                        order = new Orders
                        {
                            Status = ConfigTermins.Active,
                            Account = Key,
                            ClientCode = SecretKey,
                            Balance = Convert.ToDouble(x.Quantity),
                            Price = Convert.ToDouble(x.Price),
                            Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                            Number = x.Id.ToString(),
                            Time = x.CreateTime,
                            Symbol = x.Symbol.ToUpper(),
                            ClassCode = $"Binance {(_typeMarket)}",
                            Quantity = Convert.ToDouble(x.Quantity),
                            Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                        };

                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                    }

                    if (x.Status.Equals(OrderStatus.Canceled))
                    {
                        order = new Orders()
                        {
                            Status = ConfigTermins.Cancel,
                            Account = Key,
                            ClientCode = SecretKey,
                            Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                            Symbol = x.Symbol.ToUpper(),
                            ClassCode = $"Binance {(_typeMarket)}",
                            Time = x.CreateTime,
                            Number = x.Id.ToString(),
                            Balance = Convert.ToDouble(x.Quantity),
                            CancelOrderTime = x.UpdateTime,
                            Price = Convert.ToDouble(x.Price),
                            Quantity = Convert.ToDouble(x.Quantity - x.QuantityFilled),
                            Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                        };

                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                    }

                    if (x.Status.Equals(OrderStatus.Filled))
                    {
                        var deal = new Deal()
                        {
                            Account = Key,
                            ClientCode = SecretKey,
                            ClassCode = $"Binance {(_typeMarket)}",
                            Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                            Price = Convert.ToDouble(x?.Price),
                            Symbol = x.Symbol,
                            Order = x.Id.ToString(),
                            NumberTrade = x.TradeId.ToString(),
                            DateTrade = x.CreateTime,
                            Quantity = Convert.ToDouble(x?.Quantity),
                            Volume = Convert.ToDouble(x?.Quantity) * Convert.ToDouble(x?.Price),
                        };

                        order = new Orders()
                        {
                            Status = ConfigTermins.Performed,
                            Account = Key,
                            ClientCode = SecretKey,
                            Balance = Convert.ToDouble(x.Quantity - x.QuantityFilled),
                            Number = x.Id.ToString(),
                            Symbol = x.Symbol,
                            ClassCode = $"Binance {(_typeMarket)}"
                        };

                        _abstractTerminal.AddDeal(_abstractTerminal.BaseTable.DealsList, deal);
                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                    }
                });

                var transferToBalanceTerminal = new ActionBlock<BinanceStreamPositionsUpdate>(x =>
                {
                    var shares = x.Balances.Select(x => new MoneyPosition()
                    {
                        ClientCode = SecretKey,
                        Account = Key,
                        Asset = x.Asset.ToUpper(),
                        Currency = x.Asset.ToUpper(),
                        Balance = Convert.ToDouble(x.Available),
                        Group = "Binance",
                    }).ToList();

                    foreach (var share in shares)
                    {
                        _abstractTerminal.AddMoneyShare(_abstractTerminal.BaseTable.LimitMoneySharesList, share);
                    }
                });

                _binanceSocket.BufferUpdateOrdersSpot.LinkTo(tranferToTerminalOrders);
                _binanceSocket.BufferBalanceUpdateSpot.LinkTo(transferToBalanceTerminal);
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                await _binanceSocket.SubscribeToUserAccountUpdatesAsync();

                var tranferToTerminalOrders = new ActionBlock<BinanceFuturesStreamOrderUpdate>(x =>
                {
                    Orders order = null;

                    if (x.UpdateData.Status.Equals(OrderStatus.New))
                    {
                        order = new Orders
                        {
                            Status = ConfigTermins.Active,
                            Account = Key,
                            ClientCode = SecretKey,
                            Balance = Convert.ToDouble(x.UpdateData.Quantity),
                            Price = Convert.ToDouble(x.UpdateData.Price),
                            Id = x.UpdateData.ClientOrderId is null ? string.Empty : x.UpdateData.ClientOrderId,
                            Number = x.UpdateData.OrderId.ToString(),
                            Time = x.UpdateData.UpdateTime,
                            Symbol = x.UpdateData.Symbol.ToUpper(),
                            ClassCode = $"Binance {(_typeMarket)}",
                            Quantity = Convert.ToDouble(x.UpdateData.Quantity),
                            Operation = x.UpdateData.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                        };

                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                    }

                    if (x.UpdateData.Status.Equals(OrderStatus.Canceled))
                    {
                        order = new Orders()
                        {
                            Status = ConfigTermins.Cancel,
                            Account = Key,
                            ClientCode = SecretKey,
                            Id = x.UpdateData.ClientOrderId is null ? string.Empty : x.UpdateData.ClientOrderId,
                            Symbol = x.UpdateData.Symbol.ToUpper(),
                            ClassCode = $"Binance {(_typeMarket)}",
                            Time = x.UpdateData.UpdateTime,
                            Number = x.UpdateData.OrderId.ToString(),
                            Balance = Convert.ToDouble(x.UpdateData.Quantity),
                            CancelOrderTime = x.UpdateData.UpdateTime,
                            Price = Convert.ToDouble(x.UpdateData.Price),
                            Quantity = Convert.ToDouble(x.UpdateData.Quantity),
                            Operation = x.UpdateData.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                        };

                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                    }

                    if (x.UpdateData.Status.Equals(OrderStatus.Filled))
                    {
                        var deal = new Deal()
                        {
                            Account = Key,
                            ClientCode = SecretKey,
                            ClassCode = $"Binance {(_typeMarket)}",
                            Operation = x.UpdateData.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                            Price = Convert.ToDouble(x?.UpdateData.Price),
                            Symbol = x.UpdateData.Symbol,
                            Order = x.UpdateData.OrderId.ToString(),
                            NumberTrade = x.UpdateData.TradeId.ToString(),
                            DateTrade = x.UpdateData.UpdateTime,
                            Quantity = Convert.ToDouble(x?.UpdateData.Quantity),
                            Volume = Convert.ToDouble(x?.UpdateData.Quantity) * Convert.ToDouble(x?.UpdateData.Price),
                        };

                        order = new Orders()
                        {
                            Status = ConfigTermins.Performed,
                            Account = Key,
                            ClientCode = SecretKey,
                            Balance = Convert.ToDouble(x.UpdateData.Quantity),
                            Number = x.UpdateData.OrderId.ToString(),
                            Symbol = x.UpdateData.Symbol,
                            ClassCode = $"Binance {(_typeMarket)}"
                        };

                        _abstractTerminal.AddDeal(_abstractTerminal.BaseTable.DealsList, deal);
                        _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                    }
                });

                var transferToBalanceTerminal = new ActionBlock<BinanceFuturesStreamAccountUpdate>(x =>
                {
                    var shares = x.UpdateData.Balances.Select(x => new MoneyPosition()
                    {
                        ClientCode = SecretKey,
                        Account = Key,
                        Asset = x.Asset.ToUpper(),
                        Balance = Convert.ToDouble(x.CrossWalletBalance)
                    }).ToList();

                    foreach (var share in shares)
                    {
                        _abstractTerminal.AddMoneyShare(_abstractTerminal.BaseTable.LimitMoneySharesList, share);
                    }
                });

                _binanceSocket.BufferUpdateOrdersUsdFutures.LinkTo(tranferToTerminalOrders);
                _binanceSocket.BufferBalanceUpdateUsdFutures.LinkTo(transferToBalanceTerminal);
            }
        }

        public async Task SubcribeToInstrumentsAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            await _binanceSocket.SubscribeToTickerUpdatesAsync(avalibleInstrumentsModel);

            var transferTickersToTerminal = new ActionBlock<IBinanceTick>(data =>
            {
                Securities securities = new Securities()
                {
                    ClosePrice = Convert.ToDouble(data.LastPrice),
                    LastPrice = Convert.ToDouble(data.LastPrice),
                    OpenPrice = Convert.ToDouble(data.OpenPrice),
                    MaxPrice = Convert.ToDouble(data.HighPrice),
                    MinPrice = Convert.ToDouble(data.LowPrice),
                    Bid = Convert.ToDouble(data.BestBidPrice),
                    Offer = Convert.ToDouble(data.BestAskPrice),
                    TimeLastChange = DateTime.UtcNow,
                    Seccode = data.Symbol.ToUpper(),
                    ClassCode = $"Binance {_typeMarket}"
                };

                _abstractTerminal.AddSecurity(securities);
            });

            _binanceSocket.BufferTickers.LinkTo(transferTickersToTerminal);
        }

        public async Task GetBalancesAsync()
        {
            var balances = await _binanceRestApi.GetBalancesAccountAsync();

            var moneyPositions = balances.Select(x => new MoneyPosition()
            {
                ClientCode = SecretKey,
                Account = Key,
                Asset = x.Asset.ToUpper(),
                Currency = x.Asset.ToUpper(),
                Balance = Convert.ToDouble(x.Available),
                TotalBalance = Convert.ToDouble(x.Total),
                Group = _typeMarket.Equals(TypeMarket.Spot) ? $"Binance {_typeMarket}" : _typeMarket.Equals(TypeMarket.UsdFutures) ? $"Binance {_typeMarket}" : $"Binance {_typeMarket}",
            });

            foreach (var money in moneyPositions)
            {
                _abstractTerminal.AddMoneyShare(_abstractTerminal.BaseTable.LimitMoneySharesList, money);
            }
        }

        /// <summary>
        /// Получение общего баланса пользователя в USDT & USD по всем инструментам
        /// </summary>
        /// <returns></returns>
        public async Task<TotalBalance> GetBalanceByDollars()
        {
            return await _binanceRestApi.GetBalancesBTC();
        }

        public async Task GetHistoryOrdesAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModels)
        {
            var historyOrders = await _binanceRestApi.GetHistoryOrdesAsync("BTCUSDT");
        }

        public async Task<IEnumerable<Securities>> GetAllTickersAsync()
        {
            var tickers = await _binanceRestApi.GetAllTickers();

            return tickers.Where(x => x.Status.Equals(SymbolStatus.Trading))
                .Select(x => new Securities()
                {
                    Seccode = x.Symbol.ToUpper(),
                    ClassCode = "Binance",
                    IsCrypto = true,
                    BaseActive = x.QuoteAsset.ToUpper(),
                    ShortName = x.BaseAsset.ToUpper(),
                    TradingStatus = "Торгуется",
                    Status = "Открыта",
                    Accuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(x?.TickSize))),
                    LotSize = Convert.ToDouble(x?.MinQuantity),
                    MinStep = Convert.ToDouble(x?.TickSize),
                    PointCost = Convert.ToDouble(x?.MinPrice),
                    MinAmount = Convert.ToDouble(x?.MinQuantity),
                    IsTrade = true,
                }).Where(ticker => !String.IsNullOrEmpty(ticker.Seccode) && !String.IsNullOrEmpty(ticker.BaseActive)).ToList();
        }

        public async Task SubscribeToGlass(List<AvalibleInstrumentsModel> avalibleInstrumentsModels, int levels = 20)
        {
            if (!avalibleInstrumentsModels.Any())
                return;

            await _binanceSocket.SubcribeToOrderBookAsync(avalibleInstrumentsModels, levels);

            var transferToGlass = new ActionBlock<IBinanceOrderBook>(x =>
            {
                var asks = x.Asks.Select(x => new GlassQuotation()
                {
                    SellQty = Convert.ToDouble(x.Quantity),
                    Price = Convert.ToDouble(x.Price)
                }).ToList();
                var bids = x.Bids.Select(x => new GlassQuotation()
                {
                    BuyQty = Convert.ToDouble(x.Quantity),
                    Price = Convert.ToDouble(x.Price)
                }).ToList();

                Glass glass = new Glass()
                {
                    ClassCode = $"Binance {_typeMarket}",
                    Symbol = x.Symbol.ToUpper(),
                    Deep = levels,
                };

                glass.QuotationsBuy.AddRange(bids);
                glass.QuotationsSell.AddRange(asks);

                ((BitfinexConnector)_abstractTerminal).UpdateGlass(glass);
            });

            _binanceSocket.BufferGlass.LinkTo(transferToGlass);
        }

        public async Task PlaceOrderAsync(string symbol, OrderSide orderside, OrderType orderType, decimal quantity, string clientId, TimeInForce? timeInForce = null, decimal? price = null)
        {
            await _binanceRestApi.PlaceOrderAsync(symbol, orderside, orderType, quantity, clientId, timeInForce, price);

            var transferToTerminalOrders = new ActionBlock<Order>(x =>
            {
                var etsOrder = new Orders()
                {
                    Status = ConfigTermins.Active,
                    Account = Key,
                    ClientCode = SecretKey,
                    Balance = Convert.ToDouble(x.Quantity),
                    Price = Convert.ToDouble(x.Price),
                    Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                    Number = x.OrderId.ToString(),
                    Symbol = x.Symbol is null ? string.Empty : x.Symbol,
                    ClassCode = $"Binance {_typeMarket}",
                    Quantity = Convert.ToDouble(x.Quantity),
                    Operation = x.OrderSide.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                };

                _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, etsOrder);
            });

            _binanceRestApi.BufferPlaceOrders.LinkTo(transferToTerminalOrders);
        }

        public async Task CancelOrderAsync(string symbol, long orderId)
        {
            await _binanceRestApi.CancelOrderAsync(symbol, orderId);

            var transferToTerminalOrders = new ActionBlock<CancelOrder>(x =>
            {
                var etsOrder = new Orders()
                {
                    Status = ConfigTermins.Cancel,
                    Account = Key,
                    ClientCode = SecretKey,
                    Balance = Convert.ToDouble(x.Quantity),
                    Price = Convert.ToDouble(x.Price),
                    Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                    Number = x.OrderId.ToString(),
                    Symbol = x.Symbol is null ? string.Empty : x.Symbol,
                    ClassCode = $"Binance {_typeMarket}",
                    Quantity = Convert.ToDouble(x.QuantityRemaining),
                    Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                };

                _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, etsOrder);
            });

            _binanceRestApi.BufferCancelOrders.LinkTo(transferToTerminalOrders);
        }

        public async Task GetOrderBookAsync(string symbol, int limit = 100)
        {
            await _binanceRestApi.GetOrderBookAsync(symbol, limit);

            var transferToGlass = new ActionBlock<IBinanceOrderBook>(x =>
            {
                var asks = x.Asks.Select(x => new GlassQuotation()
                {
                    SellQty = Convert.ToDouble(x.Quantity),
                    Price = Convert.ToDouble(x.Price)
                }).ToList();

                var bids = x.Bids.Select(x => new GlassQuotation()
                {
                    BuyQty = Convert.ToDouble(x.Quantity),
                    Price = Convert.ToDouble(x.Price)
                }).ToList();

                Glass glass = new Glass()
                {
                    ClassCode = $"Binance {_typeMarket}",
                    Symbol = x.Symbol.ToUpper(),
                    Deep = limit,
                };

                glass.QuotationsBuy.AddRange(bids);
                glass.QuotationsSell.AddRange(asks);

                ((BitfinexConnector)_abstractTerminal).UpdateGlass(glass);
            });

            _binanceRestApi.BufferOrderBook.LinkTo(transferToGlass);
        }

        public async Task<Candles> GetHistoryKlinesAsync(AvalibleInstrumentsModel avalibleInstruments, string timeFrame, DateTime dateStart, DateTime dateEnd)
        {
            List<IBinanceKline> lst = new List<IBinanceKline>();

            DateTime firstKlineTime = (await _binanceRestApi.GetKlinesAsync(avalibleInstruments.Symbol, CfgSourceEts.DownLoader1Month, dateStart)).First().OpenTime;

            if (dateStart < firstKlineTime)
                dateStart = firstKlineTime;

            DateTime endTime = dateStart;

            while (dateStart < dateEnd)
            {
                AddTimeToEndTime(timeFrame, endTime);

                var currentKlines = await _binanceRestApi.GetKlinesAsync(avalibleInstruments.Symbol, timeFrame, dateStart, endTime);

                if (currentKlines.Any())
                {
                    lst.AddRange(currentKlines);
                    dateStart = AddTimeToDateStart(dateStart, timeFrame);
                }
            }

            Candles candles = new Candles();

            foreach (var item in lst)
            {
                candles.Candle.Add(new CandleModel()
                {
                    Close = Convert.ToDouble(item.ClosePrice),
                    High = Convert.ToDouble(item.HighPrice),
                    Open = Convert.ToDouble(item.OpenPrice),
                    Low = Convert.ToDouble(item.LowPrice),
                    Volume = Convert.ToDouble(item.Volume),
                    TradeDateTime = item.OpenTime,
                });
            }

            return candles;
        }

        public async Task SubscribeToTradesAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            await _binanceSocket.SubscribeToTradesAsync(avalibleInstrumentsModel);

            var transferTrades = new ActionBlock<Trades>(x =>
            {
                Tick tick = new Tick()
                {
                    Volume = Convert.ToDouble(x.Quantity),
                    TradeDateTime = x.TradeTime,
                    Price = Convert.ToDouble(x.Price),
                    BuySell = x.IsSell ? CfgSourceEts.SellForAllTradeTable : CfgSourceEts.BuyForAllTradeTable,
                    Seccode = x.Symbol
                };
            });

            _binanceSocket.BufferTrades.LinkTo(transferTrades);
        }

        public async Task GetOpenOrdersAsync(string? symbol = null)
        {
            var openOrders = await _binanceRestApi.GetCurrentOpenOrdersAsync(symbol);
            if (openOrders.Any())
            {
                var orders = openOrders.Select(x => new Orders()
                {
                    Status = ConfigTermins.Active,
                    Account = Key,
                    ClientCode = SecretKey,
                    Balance = Convert.ToDouble(x.QuantityRemaining),
                    Price = Convert.ToDouble(x.Price),
                    Number = x.OrderId.ToString(),
                    Time = x.CreateTime,
                    Symbol = x.Symbol,
                    ClassCode = $"Binance {(_typeMarket)}",
                    Quantity = Convert.ToDouble(x.Quantity),
                    Operation = x.OrderSide.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                }).ToList();

                foreach (var order in orders)
                {
                    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                }
            }
        }

        #region Helpers

        private DateTime AddTimeToDateStart(DateTime dateStart, string timeFrame)
        {
            if (timeFrame.Equals(CfgSourceEts.DownLoader1Minutes))
                return dateStart.AddMinutes(1);

            if (timeFrame.Equals(CfgSourceEts.DownLoader5Minutes))
                return dateStart.AddMinutes(5);

            if (timeFrame.Equals(CfgSourceEts.DownLoader15Minutes))
                return dateStart.AddMinutes(15);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Hour))
                return dateStart.AddHours(1);

            if (timeFrame.Equals(CfgSourceEts.DownLoader4Hour))
                return dateStart.AddHours(4);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Day))
                return dateStart.AddDays(1);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Week))
                return dateStart.AddDays(7);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Week))
                return dateStart.AddMonths(1);

            return new DateTime();
        }

        private DateTime AddTimeToEndTime(string timeFrame, DateTime endTime)
        {
            if (timeFrame.Equals(CfgSourceEts.DownLoader1Minutes))
                return endTime.AddMinutes(1000);

            if (timeFrame.Equals(CfgSourceEts.DownLoader5Minutes))
                return endTime.AddMinutes(1000 * 5);

            if (timeFrame.Equals(CfgSourceEts.DownLoader15Minutes))
                return endTime.AddMinutes(1000 * 15);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Hour))
                return endTime.AddHours(1000);

            if (timeFrame.Equals(CfgSourceEts.DownLoader4Hour))
                return endTime.AddHours(1000 * 4);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Day))
                return endTime.AddDays(1000);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Week))
                return endTime.AddDays(1000 * 7);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Week))
                return endTime.AddMonths(1000);

            return new DateTime();
        }

        #endregion
    }
}
