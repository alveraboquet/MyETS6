using Adapter;
using Adapter.Config;
using CommonDataContract;
using CommonDataContract.ReactData;
using CryptoCon.FTX.Enums;
using CryptoCon.FTX.Models;
using FTX.Net.Enums;
using FTX.Net.Objects.Models;
using FTX.Net.Objects.Models.Socket;
using SourceEts;
using SourceEts.Models.TimeFrameTransformModel;
using SourceEts.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.FTX
{
    public class FTXWrapper
    {
        private FTXRestApi _ftxRestApi { get; set; }
        private FTXSocket _fTXSocket { get; set; }

        private TypeMarket _typeMarket { get; set; }

        public AbstractTerminal _abstractTerminal { get; set; }

        private SortedDictionary<string, FtxGlass> symbolsDic { get; set; }

        private string Key { get; set; }
        private string SecretKey { get; set; }

        public FTXWrapper(string key, string secretKey, TypeMarket typeMarket, AbstractTerminal abstractTerminal)
        {
            Key = key;
            SecretKey = secretKey;

            _ftxRestApi = new FTXRestApi(key, secretKey, typeMarket);
            _fTXSocket = new FTXSocket(key, secretKey, typeMarket);
            _typeMarket = typeMarket;

            _abstractTerminal = abstractTerminal;

            symbolsDic = new();
        }

        public async Task<IEnumerable<Securities>> GetAllTickersAsync()
        {
            var tickers = await _ftxRestApi.GetAllTickers();

            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                return tickers.Where(x => x.Type.ToString().Equals(SymbolType.Spot.ToString()) && x.Enabled == true)
                    .Select(x => new Securities()
                    {
                        Seccode = x.Name,
                        Isin = x.Name,
                        ClassCode = $"FTX {_typeMarket}",
                        IsCrypto = true,
                        BaseActive = x.QuoteAsset,
                        ShortName = x.BaseAsset,
                        TradingStatus = "Торгуется",
                        Status = "Открыта",
                        Accuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(x?.PriceStep))),
                        LotSize = Convert.ToDouble(x?.QuantityStep),
                        MinStep = Convert.ToDouble(x?.PriceStep),
                        PointCost = Convert.ToDouble(x?.PriceStep),
                        IsTrade = true,
                    }).Where(ticker => !String.IsNullOrEmpty(ticker.Seccode) && !String.IsNullOrEmpty(ticker.BaseActive)).ToList();
            }

            if (_typeMarket.Equals(TypeMarket.Futures))
            {
                return tickers.Where(x => x.Type.ToString().Equals(SymbolType.Future.ToString()) && x.Enabled == true)
                    .Select(x => new Securities()
                    {
                        Seccode = x.Name.ToUpper(),
                        Isin = x.Name,
                        ClassCode = $"FTX {_typeMarket}",
                        IsCrypto = true,
                        BaseActive = "Фьючерс",
                        ShortName = x.Underlying.ToUpper(),
                        TradingStatus = "Торгуется",
                        Status = "Открыта",
                        Accuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(x?.PriceStep))),
                        LotSize = Convert.ToDouble(x?.QuantityStep),
                        MinStep = Convert.ToDouble(x?.PriceStep),
                        PointCost = Convert.ToDouble(x?.PriceStep),
                        IsTrade = true,
                    }).Where(ticker => !String.IsNullOrEmpty(ticker.Seccode) && !String.IsNullOrEmpty(ticker.BaseActive)).ToList();
            }

            return new List<Securities>();
        }

        public async Task SubcribeToInstrumentsAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            await _fTXSocket.SubscribeToInstruments(avalibleInstrumentsModel);

            var transferTickersToTerminal = new ActionBlock<FTXTicker>(data =>
            {
                Securities securities = new Securities()
                {
                    ClosePrice = Convert.ToDouble(data.LastPrice),
                    LastPrice = Convert.ToDouble(data.LastPrice),
                    //OpenPrice = Convert.ToDouble(data.OpenPrice),
                    //MaxPrice = Convert.ToDouble(data.HighPrice),
                    //MinPrice = Convert.ToDouble(data.LowPrice),
                    Bid = Convert.ToDouble(data.BestBidPrice),
                    Offer = Convert.ToDouble(data.BestAskPrice),
                    TimeLastChange = DateTime.UtcNow,
                    Seccode = data.Symbol
                };

                _abstractTerminal.AddSecurity(securities);
            });

            _fTXSocket.BufferUpdateSubscription.LinkTo(transferTickersToTerminal);
        }

        public async Task PlaceOrderAsync(string symbol, OrderSide orderSide, OrderType orderType, decimal quantity, decimal price, string clientId)
        {
            await _ftxRestApi.PlaceOrderAsync(symbol, orderSide, orderType, quantity, price, clientId);

            var transferToTerminalOrders = new ActionBlock<FTXOrder>(x =>
            {
                var etsOrder = new Orders()
                {
                    Status = ConfigTermins.Active,
                    Account = Key,
                    ClientCode = SecretKey,
                    Balance = Convert.ToDouble(x.Quantity),
                    Price = Convert.ToDouble(x.Price),
                    Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                    Number = x.Id.ToString(),
                    Symbol = x.Symbol is null ? string.Empty : x.Symbol,
                    ClassCode = $"Binance {_typeMarket}",
                    Quantity = Convert.ToDouble(x.Quantity),
                    Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                };

                _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, etsOrder);
            });

            _ftxRestApi.BufferPlaceOrders.LinkTo(transferToTerminalOrders);
        }

        public async Task CancelOrderAsync(long orderId)
        {
            await _ftxRestApi.CancelOrderAsync(orderId);

            //var transferToTerminalOrders = new ActionBlock<CancelOrder>(x =>
            //{
            //    var etsOrder = new Orders()
            //    {
            //        Status = ConfigTermins.Cancel,
            //        Account = Key,
            //        ClientCode = SecretKey,
            //        Balance = Convert.ToDouble(x.Quantity),
            //        Price = Convert.ToDouble(x.Price),
            //        Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
            //        Number = x.OrderId.ToString(),
            //        Symbol = x.Symbol is null ? string.Empty : x.Symbol,
            //        ClassCode = $"Binance {_typeMarket}",
            //        Quantity = Convert.ToDouble(x.QuantityRemaining),
            //        Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
            //    };

            //    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, etsOrder);
            //});
        }

        public async Task SubcribeToUpdateOrdersAsync()
        {
            await _fTXSocket.SubcribeToUpdateOrdersAsync();

            var tranferToTerminalOrders = new ActionBlock<FTXOrder>(x =>
            {
                if (x.Status.Equals(OrderStatus.New))
                {
                    var order = new Orders
                    {
                        Status = ConfigTermins.Active,
                        Account = Key,
                        ClientCode = SecretKey,
                        Balance = Convert.ToDouble(x.QuantityRemaining),
                        Price = Convert.ToDouble(x.Price),
                        Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                        Number = x.Id.ToString(),
                        Time = x.CreateTime,
                        Symbol = x.Symbol.ToUpper(),
                        ClassCode = $"FTX {(_typeMarket)}",
                        Quantity = Convert.ToDouble(x.Quantity),
                        Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                    };

                    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                }

                if (x.Status.Equals(OrderStatus.Open))
                {
                    var order = new Orders
                    {
                        Status = ConfigTermins.Active,
                        Account = Key,
                        ClientCode = SecretKey,
                        Balance = Convert.ToDouble(x.QuantityRemaining),
                        Price = Convert.ToDouble(x.Price),
                        Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                        Number = x.Id.ToString(),
                        Time = x.CreateTime,
                        Symbol = x.Symbol.ToUpper(),
                        ClassCode = $"FTX {(_typeMarket)}",
                        Quantity = Convert.ToDouble(x.Quantity),
                        Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                    };

                    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                }

                if (x.Status.Equals(OrderStatus.Closed))
                {
                    //if (x.QuantityRemaining > 0)
                    //{
                    //    var deal = new Deal()
                    //    {
                    //        Account = Key,
                    //        ClientCode = SecretKey,
                    //        ClassCode = $"FTX {(_typeMarket)}",
                    //        Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                    //        Price = Convert.ToDouble(x?.Price),
                    //        Symbol = x.Symbol,
                    //        Order = x.cliToString(),
                    //        NumberTrade = x.Id.ToString(),
                    //        DateTrade = x.CreateTime,
                    //        Quantity = Convert.ToDouble(x?.Quantity),
                    //        Volume = Convert.ToDouble(x?.Quantity) * Convert.ToDouble(x?.Price),
                    //    };

                    //    _abstractTerminal.AddDeal(_abstractTerminal.BaseTable.DealsList, deal);
                    //}

                    var order = new Orders()
                    {
                        Status = x.QuantityFilled == x.Quantity ? ConfigTermins.Performed : ConfigTermins.Cancel,
                        Account = Key,
                        ClientCode = SecretKey,
                        Balance = Convert.ToDouble(x?.Quantity - x?.QuantityFilled),
                        Number = x.Id.ToString(),
                        Id = x.ClientOrderId.ToString(),
                        Symbol = x.Symbol,
                        ClassCode = $"FTX {(_typeMarket)}",
                        Time = x.CreateTime,
                        Quantity = Convert.ToDouble(x.Quantity),
                        Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                        Price = Convert.ToDouble(x.Price)
                    };

                    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                }
            });

            _fTXSocket.BufferUpdateOrders.LinkTo(tranferToTerminalOrders);
        }

        public async Task SubscribeToFilledOrdersAsync()
        {
            await _fTXSocket.SuscribeToFilledOrders();

            var tranferToTerminalOrders = new ActionBlock<FTXUserTrade>(x =>
            {
                var order = new Orders
                {
                    Status = ConfigTermins.Performed,
                    Account = Key,
                    ClientCode = SecretKey,
                    Balance = 0.0,
                    Price = Convert.ToDouble(x.Price),
                    Number = x.OrderId.ToString(),
                    Time = x.Timestamp,
                    Symbol = x.Symbol,
                    ClassCode = $"FTX {(_typeMarket)}",
                    Quantity = Convert.ToDouble(x.Quantity),
                    Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                };

                var deal = new Deal
                {
                    Account = Key,
                    ClientCode = SecretKey,
                    ClassCode = $"FTX {(_typeMarket)}",
                    Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                    Price = Convert.ToDouble(x?.Price),
                    Symbol = x.Symbol,
                    Order = x.OrderId.ToString(),
                    NumberTrade = x.Id.ToString(),
                    DateTrade = x.Timestamp,
                    Quantity = Convert.ToDouble(x?.Quantity),
                    Volume = Convert.ToDouble(x?.Quantity) * Convert.ToDouble(x?.Price),
                };

                _abstractTerminal.AddDeal(_abstractTerminal.BaseTable.DealsList, deal);
                _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
            });

            _fTXSocket.BufferFilledOrders.LinkTo(tranferToTerminalOrders);
        }

        public async Task GetBalancesAsync()
        {
            var balances = await _ftxRestApi.GetBalanceAsync();

            var moneyPositions = balances.Select(x => new MoneyPosition()
            {
                ClientCode = SecretKey,
                Account = Key,
                Asset = x.Asset.ToUpper(),
                Currency = x.Asset.ToUpper(),
                Balance = Convert.ToDouble(x.Available),
                TotalBalance = Convert.ToDouble(x.Total),
                Group = _typeMarket.Equals(TypeMarket.Spot) ? $"FTX {_typeMarket}" : _typeMarket.Equals(TypeMarket.Futures) ? $"FTX {_typeMarket}" : $"FTX {_typeMarket}",
            });

            foreach (var money in moneyPositions)
            {
                _abstractTerminal.AddMoneyShare(_abstractTerminal.BaseTable.LimitMoneySharesList, money);
            }
        }

        /// <summary>
        /// Получение общего баланса пользователя в USD по всем инструментам
        /// </summary>
        /// <returns></returns>
        public async Task<TotalBalanceFTX> GetBalanceByDollars()
        {
            return await _ftxRestApi.GetBalanceUSD();
        }

        public async Task GetHistoryOrdersAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            var historyOrders = await _ftxRestApi.GetHistoryOrdersAsync(symbol, startTime, endTime);
            if (historyOrders.Any())
            {
                var orders = historyOrders.Select(x => new Orders()
                {
                    Status = ConfigTermins.Performed,
                    Account = Key,
                    ClientCode = SecretKey,
                    Balance = Convert.ToDouble(x.Quantity),
                    Price = Convert.ToDouble(x.Price),
                    Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                    Number = x.Id.ToString(),
                    Symbol = x.Symbol is null ? string.Empty : x.Symbol,
                    ClassCode = $"Binance {_typeMarket}",
                    Quantity = Convert.ToDouble(x.QuantityRemaining),
                    Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                });

                foreach (var order in orders)
                {
                    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                }
            }
        }

        public async Task SubScribeToGlassesAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            await _fTXSocket.SubScribeToGlassAsync(avalibleInstrumentsModel);

            Stopwatch st = new Stopwatch();
            Stopwatch st2 = new Stopwatch();

            var transferToETS = new ActionBlock<FtxGlass>(updateGlass =>
            {
                st.Start();

                if (!symbolsDic.ContainsKey(updateGlass.Symbol))
                {
                    symbolsDic.Add(updateGlass.Symbol, new FtxGlass() { Action = updateGlass.Action, Symbol = updateGlass.Symbol });

                    if (!symbolsDic[updateGlass.Symbol].DictionaryAskGlass.Any() && !symbolsDic[updateGlass.Symbol].DictionaryBidGlass.Any())
                    {
                        foreach (var ask in updateGlass.DictionaryUpdateAskGlass)
                        {
                            symbolsDic[updateGlass.Symbol].DictionaryAskGlass.Add(ask.Key, ask.Value);
                        }

                        foreach (var bid in updateGlass.DictionaryUpdateBidGlass)
                        {
                            symbolsDic[updateGlass.Symbol].DictionaryBidGlass.Add(bid.Key, bid.Value);
                        }
                    }
                }
                else
                {
                    var currentElementOfDictionary = symbolsDic[updateGlass.Symbol];

                    foreach (var ask in updateGlass.DictionaryUpdateAskGlass)
                    {
                        // Если есть такой элемент с ценой, то изменяем объем (т.к данные приходят по сокету только с обновлением)
                        // Иначе, добавляем элемент
                        if (currentElementOfDictionary.DictionaryAskGlass.ContainsKey(ask.Key))
                            currentElementOfDictionary.DictionaryAskGlass[ask.Key] = ask.Value;
                        else
                            currentElementOfDictionary.DictionaryAskGlass.Add(ask.Key, ask.Value);

                        // Если по сокету пришел аск с объем равным 0, то это означает, что лимитки либо сняли по цене аска, либо все лимитки исполнились
                        if (ask.Value == 0.0m)
                        {
                            currentElementOfDictionary.DictionaryAskGlass.Remove(ask.Key);
                        }
                    }

                    foreach (var bid in updateGlass.DictionaryUpdateBidGlass)
                    {
                        // Если есть такой элемент с ценой, то изменяем объем (т.к данные приходят по сокету только с обновлением)
                        // Иначе, добавляем элемент
                        if (currentElementOfDictionary.DictionaryBidGlass.ContainsKey(bid.Key))
                            currentElementOfDictionary.DictionaryBidGlass[bid.Key] = bid.Value;
                        else
                            currentElementOfDictionary.DictionaryBidGlass.Add(bid.Key, bid.Value);

                        // Если по сокету пришел аск с объем равным 0, то это означает, что лимитки либо сняли по цене аска, либо все лимитки исполнились
                        if (bid.Value == 0.0m)
                        {
                            currentElementOfDictionary.DictionaryBidGlass.Remove(bid.Key);
                        }
                    }

                    var asks = currentElementOfDictionary.DictionaryAskGlass.Select(x => new GlassQuotation()
                    {
                        SellQty = Convert.ToDouble(x.Value),
                        Price = Convert.ToDouble(x.Key)
                    }).ToList();
                    var bids = currentElementOfDictionary.DictionaryBidGlass.Select(x => new GlassQuotation()
                    {
                        BuyQty = Convert.ToDouble(x.Value),
                        Price = Convert.ToDouble(x.Key)
                    }).ToList();

                    Glass glass = new Glass()
                    {
                        ClassCode = $"FTX {_typeMarket}",
                        Symbol = updateGlass.Symbol,
                        Deep = 100
                    };

                    glass.QuotationsBuy.AddRange(bids.OrderByDescending(x => x.Price));
                    glass.QuotationsSell.AddRange(asks);

                    st2.Start();

                    ((FTXDima80LVL)_abstractTerminal).UpdateGlass(glass);

                    st2.Stop();

                    Console.WriteLine($"Время: {st2.Elapsed}");

                    st2.Reset();
                }

                st.Stop();


                Console.WriteLine($"Время: {st.Elapsed}");

                st.Reset();
            });

            _fTXSocket.BufferGlass.LinkTo(transferToETS);
        }

        public async Task GetOpenOrdersAsync(string? symbol = null)
        {
            var openOrders = await _ftxRestApi.GetCurrentOpenOrdersAsync(symbol);
            if (openOrders.Any())
            {
                var orders = openOrders.Select(x => new Orders()
                {
                    Status = ConfigTermins.Active,
                    Account = Key,
                    ClientCode = SecretKey,
                    Balance = Convert.ToDouble(x.QuantityRemaining),
                    Price = Convert.ToDouble(x.Price),
                    Number = x.Id.ToString(),
                    Time = x.CreateTime,
                    Symbol = x.Symbol,
                    ClassCode = $"FTX {(_typeMarket)}",
                    Quantity = Convert.ToDouble(x.Quantity),
                    Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                }).ToList();

                foreach (var order in orders)
                {
                    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                }
            }
        }

        public async Task<Candles> GetHistoryKlinesAsync(AvalibleInstrumentsModel avalibleInstruments, string timeFrame, DateTime dateStart, DateTime dateEnd)
        {
            List<FTXKline> lst = new List<FTXKline>();

            DateTime firstKlineTime = (await _ftxRestApi.GetKlinesAsync(avalibleInstruments.Symbol, CfgSourceEts.DownLoader1Month, dateStart)).First().OpenTime;

            if (dateStart < firstKlineTime)
                dateStart = firstKlineTime;

            DateTime endTime = dateStart;

            while (dateStart < dateEnd)
            {
                AddTimeToEndTime(timeFrame, endTime);

                var currentKlines = await _ftxRestApi.GetKlinesAsync(avalibleInstruments.Symbol, timeFrame, dateStart, endTime);

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
                    TradeDateTime = item.OpenTime
                });
            }

            return candles;
        }

        public async Task SubscribeToTradesAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            await _fTXSocket.SubscribeToTradesAsync(avalibleInstrumentsModel);

            var transferTrades = new ActionBlock<IEnumerable<Trades>>(trades =>
            {
                var ticks = trades.Select(x => new Tick()
                {
                    Volume = Convert.ToDouble(x.Quantity),
                    TradeDateTime = x.Timestamp,
                    Price = Convert.ToDouble(x.Price),
                    BuySell = x.Side.Equals(OrderSide.Sell) ? CfgSourceEts.SellForAllTradeTable : CfgSourceEts.BuyForAllTradeTable,
                    Seccode = x.Symbol
                });
            });

            _fTXSocket.BufferTrades.LinkTo(transferTrades);
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
                return endTime.AddMinutes(1500);

            if (timeFrame.Equals(CfgSourceEts.DownLoader5Minutes))
                return endTime.AddMinutes(1500 * 5);

            if (timeFrame.Equals(CfgSourceEts.DownLoader15Minutes))
                return endTime.AddMinutes(1500 * 15);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Hour))
                return endTime.AddHours(1500);

            if (timeFrame.Equals(CfgSourceEts.DownLoader4Hour))
                return endTime.AddHours(1500 * 4);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Day))
                return endTime.AddDays(1500);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Week))
                return endTime.AddDays(1500 * 7);

            if (timeFrame.Equals(CfgSourceEts.DownLoader1Week))
                return endTime.AddMonths(1500);

            return new DateTime();
        }

        #endregion
    }
}
