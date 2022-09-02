using CryptoCon.FTX.Enums;
using CryptoCon.FTX.Models;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using FTX.Net.Clients;
using FTX.Net.Objects;
using FTX.Net.Objects.Models;
using FTX.Net.Objects.Models.Socket;
using SourceEts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.FTX
{
    public class FTXSocket
    {
        private string Key { get; set; }
        private string SecretKey { get; set; }

        private FTXSocketClient _fTXSocketClient { get; set; }

        private List<AvalibleInstrumentsModel> _subcribesGlasses { get; set; }
        private List<string> _subcribesGlassesSymbols { get; set; }

        #region Buffers for instruments

        public BufferBlock<FTXTicker> BufferUpdateSubscription { get; set; }
        private BufferBlock<string> _bufferUpdateSubscriptionFailed { get; set; }

        #endregion

        public BufferBlock<IEnumerable<Trades>> BufferTrades { get; set; }
        private BufferBlock<string> BufferTradesFailed { get; set; }
        private List<string> tradesSubscriptions { get; set; }

        private List<AvalibleInstrumentsModel> _subcribesIntrumets { get; set; }

        private TypeMarket _typeMarket { get; set; }

        #region Buffers

        public BufferBlock<FtxGlass> BufferGlass { get; set; }
        public BufferBlock<string> BufferGlassFailed { get; set; }

        public BufferBlock<FTXOrder> BufferUpdateOrders { get; set; }
        public BufferBlock<FTXUserTrade> BufferFilledOrders { get; set; }

        #endregion

        public FTXSocket(string key, string secretKey, TypeMarket typeMarket)
        {
            Key = key;
            SecretKey = secretKey;

            _typeMarket = typeMarket;

            _fTXSocketClient = new FTXSocketClient(new FTXSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials(key, secretKey)
            });

            _subcribesIntrumets = new List<AvalibleInstrumentsModel>();

            BufferGlass = new BufferBlock<FtxGlass>();
            BufferGlassFailed = new BufferBlock<string>();

            BufferUpdateSubscription = new BufferBlock<FTXTicker>();
            _bufferUpdateSubscriptionFailed = new BufferBlock<string>();

            _subcribesGlasses = new List<AvalibleInstrumentsModel>();

            BufferUpdateOrders = new BufferBlock<FTXOrder>();
            BufferFilledOrders = new BufferBlock<FTXUserTrade>();

            BufferTrades = new BufferBlock<IEnumerable<Trades>>();
            BufferTradesFailed = new BufferBlock<string>();
            tradesSubscriptions = new List<string>();

            _subcribesGlassesSymbols = new List<string>();
        }

        public async Task SubscribeToInstruments(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            List<AvalibleInstrumentsModel>? newSubscribeIntrumetns = avalibleInstrumentsModel
               .Except(_subcribesIntrumets, new AvalibleInstrumentsModelComparer()).ToList();

            foreach (var item in newSubscribeIntrumetns)
            {
                var subscribeInstruments = await _fTXSocketClient.Streams.SubscribeToTickerUpdatesAsync(_typeMarket.Equals(TypeMarket.Spot) ? item.ShortName + "/" + item.Security.BaseActive : item.Symbol, data =>
                {
                    var res = BufferUpdateSubscription.Post(new FTXTicker()
                    {
                        BestAskPrice = data.Data.BestAskPrice,
                        BestAskQuantity = data.Data.BestAskQuantity,
                        BestBidPrice = data.Data.BestBidPrice,
                        BestBidQuantity = data.Data.BestBidQuantity,
                        LastPrice = data.Data.LastPrice,
                        Timestamp = data.Data.Timestamp,
                        Symbol = item.Symbol
                    });
                });

                if (subscribeInstruments.Success)
                    _subcribesIntrumets.Add(item);

                if (!subscribeInstruments.Success)
                {
                    _bufferUpdateSubscriptionFailed.Post(item.Symbol);
                }
            }

            #region Переподписка на инструмент, в случае ошибки

            var reSubscribeToInstuments = new ActionBlock<string>(async x =>
            {
                var result = await _fTXSocketClient.Streams.SubscribeToTickerUpdatesAsync(x, data =>
                {
                    BufferUpdateSubscription.Post(new FTXTicker()
                    {
                        BestAskPrice = data.Data.BestAskPrice,
                        BestAskQuantity = data.Data.BestAskQuantity,
                        BestBidPrice = data.Data.BestBidPrice,
                        BestBidQuantity = data.Data.BestBidQuantity,
                        LastPrice = data.Data.LastPrice,
                        Timestamp = data.Data.Timestamp,
                        Symbol = x
                    });
                });

                if (!result.Success)
                {
                    _bufferUpdateSubscriptionFailed.Post(x);
                }
            });

            _bufferUpdateSubscriptionFailed.LinkTo(reSubscribeToInstuments);

            #endregion
        }

        public async Task SubcribeToUpdateOrdersAsync()
        {
            var sub = await _fTXSocketClient.Streams.SubscribeToOrderUpdatesAsync(data =>
            {
                BufferUpdateOrders.Post(data.Data);
            });

            if (!sub.Success)
                await SubcribeToUpdateOrdersAsync();
        }

        public async Task SuscribeToFilledOrders()
        {
            var sub = await _fTXSocketClient.Streams.SubscribeToUserTradeUpdatesAsync(data =>
            {
                BufferFilledOrders.Post(data.Data);
            });
            if (!sub.Success)
                await SuscribeToFilledOrders();
        }

        public async Task SubScribeToGlassAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            //List<AvalibleInstrumentsModel>? newSubscribeIntrumetns = avalibleInstrumentsModel
            //    .Except(_subcribesGlasses, new AvalibleInstrumentsModelComparer()).ToList();

            var newSubscribeIntrumetns = avalibleInstrumentsModel.Select(x => x.Symbol).Except(_subcribesGlassesSymbols).ToList();

            if (newSubscribeIntrumetns.Any())
            {
                newSubscribeIntrumetns.ForEach(async x => await OrderBookAsync(x));
            }

            var subFailedToGlass = new ActionBlock<string>(async symbolFailed =>
            {
                await OrderBookAsync(symbolFailed);
            });

            BufferGlassFailed.LinkTo(subFailedToGlass);
        }

        private async Task<CallResult<UpdateSubscription>> OrderBookAsync(string symbol)
        {
            var res = await _fTXSocketClient.Streams.SubscribeToOrderBookUpdatesAsync(symbol, updateGlass =>
            {
                //Stopwatch st = new Stopwatch();
                //st.Start();

                FtxGlass ftxGlass = new FtxGlass()
                {
                    Symbol = symbol,
                    Timestamp = updateGlass.Data.Timestamp,
                    Action = updateGlass.Data.Action
                };
                
                foreach (var ask in updateGlass.Data.Asks)
                {
                    ftxGlass.DictionaryUpdateAskGlass.Add(ask.Price, ask.Quantity);
                }

                foreach (var bid in updateGlass.Data.Bids)
                {
                    ftxGlass.DictionaryUpdateBidGlass.Add(bid.Price, bid.Quantity);
                }

                //st.Stop();

                //Console.WriteLine($"Время: {st.Elapsed}");

                //st.Reset();

                BufferGlass.Post(ftxGlass);
            });

            if (res.Success)
                _subcribesGlassesSymbols.Add(symbol);
            else
                BufferGlassFailed.Post(symbol);

            return res;
        }

        public async Task SubscribeToTradesAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            var newSubscribeTrades = avalibleInstrumentsModel.Select(x => x.Symbol).Except(tradesSubscriptions).ToList();

            if (newSubscribeTrades.Any())
                newSubscribeTrades.ForEach(async x => await SubcribeToTradeAsync(x));

            var subFailedToTrades = new ActionBlock<string>(async symbolFailed =>
            {
                await SubcribeToTradeAsync(symbolFailed);
            });

            BufferTradesFailed.LinkTo(subFailedToTrades);
        }

        private async Task SubcribeToTradeAsync(string symbol)
        {
            var tradesSubscribe = await _fTXSocketClient.Streams.SubscribeToTradeUpdatesAsync(symbol, trades =>
            {
                BufferTrades.Post(trades.Data.Select(trade=> new Trades()
                {
                    Id = trade.Id,
                    Liquidation = trade.Liquidation,
                    Price = trade.Price,
                    Quantity = trade.Quantity,
                    Side = trade.Side,
                    Timestamp = trade.Timestamp,
                    Symbol = symbol
                }).ToList());
            });

            if (tradesSubscribe.Success)
                tradesSubscriptions.Add(symbol);
            else
                BufferTradesFailed.Post(symbol);
        }
    }
}
