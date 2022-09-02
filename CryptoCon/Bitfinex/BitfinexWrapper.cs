using Adapter;
using Adapter.Config;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Models;
using CommonDataContract;
using CommonDataContract.ReactData;
using CryptoCon.Binance;
using CryptoCon.Bitfinex.Enums;
using CryptoCon.Bitfinex.Models;
using CryptoCon.FTX;
using CryptoCon.FTX.Models;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using FTX.Net.Objects.Models;
using SourceEts;
using SourceEts.Models.TimeFrameTransformModel;
using SourceEts.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Bitfinex
{
    public class BitfinexWrapper
    {
        private BitfinexSocket _bitfinexSocket { get; set; }
        private BitfinexClient _bitfinexClient { get; set; }
        private BitfinexRestApi _bitfinexRestApi { get; set; }
        private string Key { get; set; }
        private string SecretKey { get; set; }
        private List<AvalibleInstrumentsModel> _subcribesGlasses { get; set; }
        public AbstractTerminal _abstractTerminal { get; set; }

        private TypeMarket _typeMarket { get; set; }
        public BitfinexWrapper(string key, string secretKey, TypeMarket typeMarket, AbstractTerminal abstractTerminal)
        {
            Key = key;
            SecretKey = secretKey;
            _bitfinexSocket = new BitfinexSocket(key, secretKey, typeMarket);
            _abstractTerminal = abstractTerminal;
            _typeMarket = typeMarket;

            _bitfinexRestApi = new BitfinexRestApi(key, secretKey, typeMarket);
            _subcribesGlasses = new List<AvalibleInstrumentsModel>();
        }

       

        public async Task SubcribeToInstrumentsAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            await _bitfinexSocket.SubscribeToTickersAsync(avalibleInstrumentsModel);

            var transferTickersToTerminal = new ActionBlock<BitfinexTickerModel>(data =>
            {
                Securities securities = new Securities()
                {
                    ClosePrice = Convert.ToDouble(data.LastPrice),
                    LastPrice = Convert.ToDouble(data.LastPrice),
                    //OpenPrice = Convert.ToDouble(data.pric),
                    MaxPrice = Convert.ToDouble(data.HighPrice),
                    MinPrice = Convert.ToDouble(data.LowPrice),
                    Bid = Convert.ToDouble(data.BestBidPrice),
                    Offer = Convert.ToDouble(data.BestAskPrice),
                    TimeLastChange = DateTime.UtcNow,
                    Seccode = data.Symbol,
                    ClassCode = "Bifinex"
                };

                _abstractTerminal.AddSecurity(securities);
            });

            _bitfinexSocket._bufferUpdateSubscription.LinkTo(transferTickersToTerminal);
        }
        public async Task<IEnumerable<Securities>> GetAllTickersAsync()
        {
            var tickers = await _bitfinexRestApi.GetTickersAsync();

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                return tickers
                        .Select(x => new Securities()
                        {
                            Seccode = x.Symbol,
                            //Isin = x.Name,
                            ClassCode = $"Bitfinex {_typeMarket}",
                            IsCrypto = true,
                            BaseActive = "Фьючерс",
                            //ShortName = x.Underlying.ToUpper(),
                            TradingStatus = "Торгуется",
                            Status = "Открыта",
                            Accuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(x?.PricePrecision))),
                            LotSize = Convert.ToDouble(x?.Margin),
                            MinStep = Convert.ToDouble(x?.MinimumOrderQuantity),
                            //PointCost = Convert.ToDouble(x?.),
                            IsTrade = true,
                        }).Where(ticker => !String.IsNullOrEmpty(ticker.Seccode) && !String.IsNullOrEmpty(ticker.BaseActive)).ToList();
            }
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                return tickers
                        .Select(x => new Securities()
                        {
                            Seccode = x.Symbol,
                            //Isin = x.Name,
                            ClassCode = $"Bitfinex {_typeMarket}",
                            IsCrypto = true,
                            BaseActive = "Спот",
                            //ShortName = x.Underlying.ToUpper(),
                            TradingStatus = "Торгуется",
                            Status = "Открыта",
                            Accuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(x?.PricePrecision))),
                            LotSize = Convert.ToDouble(x?.Margin),
                            MinStep = Convert.ToDouble(x?.MinimumOrderQuantity),
                            //PointCost = Convert.ToDouble(x?.PriceStep),
                            IsTrade = true,
                        }).Where(ticker => !String.IsNullOrEmpty(ticker.Seccode) && !String.IsNullOrEmpty(ticker.BaseActive)).ToList();
            }

            return new List<Securities>();
        }
        public async Task GetOrderBookAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModels, int levels = 25)
        {
            if (!avalibleInstrumentsModels.Any())
                return;

            await _bitfinexSocket.SubcribeToGlasses(_typeMarket, avalibleInstrumentsModels, levels);

            var transferToGlass = new ActionBlock<OrderBooks>(async x =>
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
                    ClassCode = $"Bitfinex {_typeMarket}",
                    //Symbol = x,
                    Deep = levels,
                };

                glass.QuotationsBuy.AddRange(bids);
                glass.QuotationsSell.AddRange(asks);

                ((BitfinexConnector)_abstractTerminal).UpdateGlass(glass);
            });

            _bitfinexSocket.BufferGlass.LinkTo(transferToGlass);
        }
        public async Task<Candles> GetHistoryKlinesAsync(AvalibleInstrumentsModel avalibleInstruments, string timeFrame, DateTime dateStart, DateTime dateEnd)
        {
            List<BitfinexKline> lst = new List<BitfinexKline>();

            DateTime firstKlineTime = (await _bitfinexRestApi.GetKlinesAsync(avalibleInstruments.Symbol, CfgSourceEts.DownLoader1Month, dateStart, dateEnd)).First().OpenTime;

            if (dateStart < firstKlineTime)
                dateStart = firstKlineTime;

            DateTime endTime = dateStart;
            while (dateStart < dateEnd)
            {
                AddTimeToEndTime(timeFrame, endTime);

                var currentKlines = await _bitfinexRestApi.GetKlinesAsync(avalibleInstruments.Symbol, timeFrame, dateStart, endTime);

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
        public async Task PlaceOrderAsync(string symbol, OrderSide orderSide, OrderType orderType, decimal quantity, decimal price, string clientId)
        {
            await _bitfinexRestApi.PlaceOrderAsync(symbol, orderSide, orderType, quantity, price, clientId);

            var transferToTerminalOrders = new ActionBlock<BitfinexWriteResult<BitfinexOrder>>(x =>
            {
                var etsOrder = new Orders()
                {
                    Status = ConfigTermins.Active,
                    Account = Key,
                    ClientCode = SecretKey,
                    Balance = Convert.ToDouble(x.Data.Quantity),
                    Price = Convert.ToDouble(x.Data.Price),
                    Id = x.Data.ClientOrderId is null ? string.Empty : x.Data.ClientOrderId.ToString(),
                    Number = x.Id.ToString(),
                    Symbol = x.Data.Symbol is null ? string.Empty : x.Data.Symbol,
                    ClassCode = $"Binance {_typeMarket}",
                    Quantity = Convert.ToDouble(x.Data.Status),
                    Operation = x.Data.Side.Equals(OrderSide.Sell) ? ConfigTermins.Sell : ConfigTermins.Buy
                };

                _abstractTerminal.AddOrder(_abstractTerminal.BaseTable.OrdersList, etsOrder);
            });

            _bitfinexRestApi.BufferPlaceOrders.LinkTo(transferToTerminalOrders);
        }

        public async Task CancelOrderAsync(long orderId)
        {
            await _bitfinexRestApi.CancelOrderAsync(orderId);

            var transferToTerminalOrders = new ActionBlock<CancelOrderEntity>(x =>
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
        }
    }
}
