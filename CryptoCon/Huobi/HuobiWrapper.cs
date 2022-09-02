using Adapter;
using Adapter.Config;
using CommonDataContract;
using CommonDataContract.ReactData;
using CryptoCon.Huobi.Clients;
using CryptoCon.Huobi.Enums;
using CryptoCon.Huobi.Models;
using Huobi.Net.Enums;
using Huobi.Net.Objects.Models;
using Huobi.Net.Objects.Models.Socket;
using HuobiFutures.Models;
using SourceEts;
using SourceEts.Models.TimeFrameTransformModel;
using SourceEts.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Huobi
{
    public class HuobiWrapper
    {
        private HuobiSocket _huobiSocket { get; set; }
        private HuobiRestAPI _huobiRestAPI { get; set; }

        private string Key { get; set; }
        private string SecretKey { get; set; }

        private TypeMarket _typeMarket { get; set; }

        public AbstractTerminal _abstractTerminal { get; set; }

        public HuobiWrapper(string key, string secretKey, TypeMarket typeMarket, AbstractTerminal abstractTerminal)
        {
            _abstractTerminal = abstractTerminal;

            Key = key;
            SecretKey = secretKey;

            _typeMarket = typeMarket;

            _huobiSocket = new HuobiSocket(key, secretKey);
            _huobiRestAPI = new HuobiRestAPI(key, secretKey, _typeMarket);
        }

        public async Task<Candles> GetHistoryKlinesAsync(AvalibleInstrumentsModel avalibleInstruments, string timeFrame, DateTime dateStart, DateTime dateEnd)
        {
            List<HuobiFuturesKline> lst = new List<HuobiFuturesKline>();

            DateTime firstKlineTime = DateTimeOffset.FromUnixTimeSeconds((await _huobiRestAPI.GetKlinesAsync(avalibleInstruments.Symbol, CfgSourceEts.DownLoader1Month, dateStart, dateEnd)).First().OpenTime).DateTime;

            if (dateStart < firstKlineTime)
                dateStart = firstKlineTime;

            DateTime endTime = dateStart;

            while (dateStart < dateEnd)
            {
                AddTimeToEndTime(timeFrame, endTime);

                var currentKlines = await _huobiRestAPI.GetKlinesAsync(avalibleInstruments.Symbol, timeFrame, dateStart, endTime);

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
                    Close = Convert.ToDouble(item.Close),
                    High = Convert.ToDouble(item.High),
                    Open = Convert.ToDouble(item.Open),
                    Low = Convert.ToDouble(item.Low),
                    Volume = Convert.ToDouble(item.Volume),
                    TradeDateTime = DateTimeOffset.FromUnixTimeSeconds(item.OpenTime).DateTime
                });
            }

            return candles;
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

        public async Task<List<Securities>> GetAllTickers()
        {
            return await _huobiRestAPI.GetAllTickers();
        }

        public async Task SubscribeToUpdateOrder(string? symbol = null)
        {
            await _huobiSocket.SubscribeToOrderUpdatesAsync(symbol);

            var transferSubmittedOrderToTerminal = new ActionBlock<HuobiSubmittedOrderUpdate>(x =>
            {
                var order = new Orders()
                {
                    Status = ConfigTermins.Active,
                    Account = Key,
                    ClientCode = SecretKey,
                    Balance = Convert.ToDouble(x.Quantity),
                    Price = Convert.ToDouble(x.Price),
                    Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                    Number = x.OrderId.ToString(),
                    Time = x.CreateTime,
                    Symbol = x.Symbol.ToUpper(),
                    ClassCode = "Huobi (Spot)",
                    Quantity = Convert.ToDouble(x.Quantity),
                    Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                };

                _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
            }, new ExecutionDataflowBlockOptions() { EnsureOrdered = true });

            var transferCancelledOrderToTerminal = new ActionBlock<HuobiCanceledOrderUpdate>(x =>
            {
                var order = new Orders()
                {
                    Status = ConfigTermins.Cancel,
                    Account = Key,
                    ClientCode = SecretKey,
                    Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                    Symbol = x.Symbol.ToUpper(),
                    ClassCode = "Huobi (Spot)",
                    Time = x.UpdateTime.HasValue ? x.UpdateTime.Value : new DateTime(),
                    Number = x.OrderId.ToString(),
                    Balance = Convert.ToDouble(x.Quantity),
                    CancelOrderTime = x.UpdateTime.HasValue ? x.UpdateTime.Value : new DateTime(),
                    Price = Convert.ToDouble(x.Price),
                    Quantity = Convert.ToDouble(x.QuantityRemaining),
                    Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                };

                _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
            }, new ExecutionDataflowBlockOptions() { EnsureOrdered = true });

            var transferMatchedOrderToTerminal = new ActionBlock<HuobiMatchedOrderUpdate>(x =>
            {
                var deal = new Deal()
                {
                    Account = Key,
                    ClientCode = SecretKey,
                    ClassCode = "Huobi (Spot)",
                    Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                    Price = Convert.ToDouble(x?.Price),
                    Symbol = x.Symbol.ToUpper(),
                    Order = x.OrderId.ToString(),
                    NumberTrade = x.TradeId.ToString(),
                    DateTrade = x.TradeTime,
                    Quantity = Convert.ToDouble(x?.Quantity),
                    Volume = Convert.ToDouble(x?.Quantity) * Convert.ToDouble(x?.Price),
                };

                var order = new Orders()
                {
                    Status = x.Status.Equals(OrderState.Filled) ? ConfigTermins.Performed : ConfigTermins.Active,
                    Account = Key,
                    ClientCode = SecretKey,
                    Balance = Convert.ToDouble(x.QuantityRemaining),
                    Number = x.OrderId.ToString(),
                    Symbol = x.Symbol,
                    ClassCode = "Huobi (Spot)",
                };

                _abstractTerminal.AddDeal(_abstractTerminal.BaseTable.DealsList, deal);
                _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);

            }, new ExecutionDataflowBlockOptions() { EnsureOrdered = true });

            _huobiSocket.BufferSubmittedOrderUpdate.LinkTo(transferSubmittedOrderToTerminal);
            _huobiSocket.BufferCancelledOrderUpdate.LinkTo(transferCancelledOrderToTerminal);
            _huobiSocket.BufferMatchedOrderUpdate.LinkTo(transferMatchedOrderToTerminal);
        }

        public async Task SubcribeToInstrumentsAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            await _huobiSocket.SubcribeToInstrumentsAsync(avalibleInstrumentsModel);

            var transferTradesToTerminal = new ActionBlock<HuobiSymbolTick>(data =>
            {
                Securities securities = new Securities()
                {
                    ClosePrice = Convert.ToDouble(data.ClosePrice),
                    LastPrice = Convert.ToDouble(data.LastTradePrice),
                    OpenPrice = Convert.ToDouble(data.OpenPrice),
                    MaxPrice = Convert.ToDouble(data.HighPrice),
                    MinPrice = Convert.ToDouble(data.LowPrice),
                    Bid = Convert.ToDouble(data.BestBidPrice),
                    Offer = Convert.ToDouble(data.BestAskPrice),
                    TimeLastChange = DateTime.UtcNow,
                    Seccode = data.Symbol.ToUpper()
                };

                _abstractTerminal.AddSecurity(securities);
            });

            _huobiSocket.BufferUpdateSubscription.LinkTo(transferTradesToTerminal);
        }

        public async Task SubscribeToTradesAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            await _huobiSocket.SubscribeToTradesAsync(avalibleInstrumentsModel);

            var transferTrades = new ActionBlock<TradesUpdate>(x =>
            {
                string symbol = x.Symbol.ToUpper();

                var ticks = x.Details.Select(x => new Tick()
                {
                    Seccode = symbol,
                    ClassCode = "Huobi (Spot)",
                    Price = Convert.ToDouble(x.Price),
                    TradeNum = x.TradeId,
                    Qty = Convert.ToDouble(x.Quantity),
                    Volume = Convert.ToDouble(x?.Quantity * x?.Price),
                    BuySell = x.Side.Equals(OrderSide.Buy) ? ConfigTermins.BuyForAllTradeTable : ConfigTermins.SellForAllTradeTable,
                }).ToList();
            });


            _huobiSocket.BufferTrades.LinkTo(transferTrades);
        }

        public async Task GetOpenOrdersAsync()
        {
            var openOrders = await _huobiRestAPI.GetOpenOrders();

            if (openOrders.Any())
            {
                var orders = openOrders.Select(x => new Orders()
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
                    ClassCode = "Huobi (Spot)",
                    Quantity = Convert.ToDouble(x.Quantity),
                    Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                });

                foreach (var order in orders)
                {
                    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                }
            }
        }

        public async Task GetHistoryOrdersAsync(string? symbol = null, int? limit = null)
        {
            var historyOrders = await _huobiRestAPI.GetHistoryOrdesAsync(symbol, limit);
            if (historyOrders.Any())
            {
                var orders = historyOrders.Select(x => new Orders()
                {
                    Status = x.State.Equals(OrderState.Canceled) ? ConfigTermins.Cancel : x.State.Equals(OrderState.Filled) ? ConfigTermins.Performed : ConfigTermins.Active,
                    Account = Key,
                    ClientCode = SecretKey,
                    Balance = Convert.ToDouble(x.Quantity - x.QuantityFilled),
                    Price = Convert.ToDouble(x.Price),
                    Id = x.ClientOrderId is null ? string.Empty : x.ClientOrderId,
                    Number = x.Id.ToString(),
                    Time = x.CreateTime,
                    Symbol = x.Symbol.ToUpper(),
                    ClassCode = "Huobi (Spot)",
                    Quantity = Convert.ToDouble(x.Quantity),
                    Operation = x.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy,
                }).ToList();

                foreach (var order in orders)
                {
                    _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, order);
                }
            }
        }

        public async Task SubscribeToBalanceAsync()
        {
            await _huobiSocket.SubcribeToBalancesAccountAsync();

            var transferToBalanceTerminal = new ActionBlock<HuobiAccountUpdate>(account =>
            {
                MoneyPosition IMoneyShares = new MoneyPosition()
                {
                    ClientCode = SecretKey,
                    Account = Key,
                    Asset = account.Asset.ToUpper(),
                    Currency = account.Asset.ToUpper(),
                    Balance = Convert.ToDouble(account.Available),
                    Group = "Huobi",
                };

                _abstractTerminal.AddMoneyShare(_abstractTerminal.BaseTable.LimitMoneySharesList, IMoneyShares);
            });

            _huobiSocket.BufferBalances.LinkTo(transferToBalanceTerminal);
        }

        public async Task PlaceOrderAsync(string symbol, OrderSide orderSide, OrderType orderType, decimal quantity, decimal price, string? clientOrderId = null)
        {
            await _huobiRestAPI.PlaceOrderAsync(symbol, orderSide, orderType, quantity, price, clientOrderId);

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
                    ClassCode = $"Huobi",
                    Quantity = Convert.ToDouble(x.Quantity),
                    Operation = x.OrderSide.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                };

                _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, etsOrder);
            });
            _huobiRestAPI.BufferPlaceOrder.LinkTo(transferToTerminalOrders);

        }

        public async Task CancelOrderAsync(long orderId)
        {
            await _huobiRestAPI.CancelOrderAsync(orderId);
        }

        public async Task SubscribeToGlasses(List<AvalibleInstrumentsModel> avalibleInstrumentsModel, int levels = 20)
        {
            if (!avalibleInstrumentsModel.Any())
                return;

            await _huobiSocket.SubscribeToOrderBookAsync(avalibleInstrumentsModel);

            var transferToGlass = new ActionBlock<HuobiGlass>(x =>
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
                    ClassCode = $"Huobi",
                    Symbol = x.Symbol,
                    Deep = levels,
                };

                glass.QuotationsBuy.AddRange(bids);
                glass.QuotationsSell.AddRange(asks);

                ((HuobiClass)_abstractTerminal).UpdateGlass(glass);
            });

            _huobiSocket.BufferGlass.LinkTo(transferToGlass);
        }
    }
}
