using Adapter;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects;
using CryptoCon.Binance;
using CryptoCon.Bitfinex.Enums;
using CryptoCon.Bitfinex.Models;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using SourceEts;
using SourceEts.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Bitfinex
{
    public class BitfinexSocket
    {
        private string Key { get; set; }
        private string SecretKey { get; set; }

        private BitfinexSocketClient _bitfinexSocketClient { get; set; }

        private List<AvalibleInstrumentsModel> _subcribesGlasses { get; set; }
        private List<string> _subcribesGlassesSymbols { get; set; }

        #region Buffers for instruments

        public BufferBlock<BitfinexTickerModel> _bufferUpdateSubscription { get; set; }
        private BufferBlock<string> _bufferUpdateSubscriptionFailed { get; set; }

        #endregion

        private List<AvalibleInstrumentsModel> _subcribesIntrumets { get; set; }

        private TypeMarket _typeMarket { get; set; }
        public BufferBlock<OrderBooks> BufferGlass { get; set; }

        public OrderBooks _orderBook { get; set; }
        //#region Buffers

        //public BufferBlock<FtxGlass> BufferGlass { get; set; }
        //public BufferBlock<string> BufferGlassFailed { get; set; }

        //public BufferBlock<FTXOrder> BufferUpdateOrders { get; set; }
        public BitfinexSocket(string key, string secretKey, TypeMarket typeMarket)
        {
            Key = key;
            SecretKey = secretKey;

            _typeMarket = typeMarket;

            _bitfinexSocketClient = new BitfinexSocketClient(new BitfinexSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials(key, secretKey)
            });

            _subcribesGlasses = new List<AvalibleInstrumentsModel>();
            _subcribesIntrumets = new List<AvalibleInstrumentsModel>();

            BufferGlass = new BufferBlock<OrderBooks>();
            //BufferGlass = new BufferBlock<FtxGlass>();
            //BufferGlassFailed = new BufferBlock<string>();
            _orderBook = new OrderBooks() { Bids = new List<BidsAndAsks>(), Asks = new List<BidsAndAsks>() };

            _bufferUpdateSubscription = new BufferBlock<BitfinexTickerModel>();
            _bufferUpdateSubscriptionFailed = new BufferBlock<string>();


            //BufferUpdateOrders = new BufferBlock<FTXOrder>();


            _subcribesGlassesSymbols = new List<string>();
        }

        public async Task SubscribeToTickersAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            List<AvalibleInstrumentsModel>? newSubscribeIntrumetns = avalibleInstrumentsModel
               .Except(_subcribesIntrumets, new AvalibleInstrumentsModelComparer()).ToList();

            foreach (var item in newSubscribeIntrumetns)
            {
                var subscribeInstruments = await _bitfinexSocketClient.SpotStreams.SubscribeToTickerUpdatesAsync(item.Symbol, async data =>
                {
                    await _bufferUpdateSubscription.SendAsync(new BitfinexTickerModel
                    {
                        BestAskPrice = data.Data.BestAskPrice,
                        BestBidPrice = data.Data.BestBidPrice,
                        BestAskQuantity = data.Data.BestAskQuantity,
                        BestBidQuantity = data.Data.BestBidQuantity,
                        DailyChange = data.Data.DailyChange,
                        DailyChangePercentage = data.Data.DailyChangePercentage,
                        HighPrice = data.Data.HighPrice,
                        LastPrice = data.Data.LastPrice,
                        LowPrice = data.Data.LowPrice,
                        Volume = data.Data.Volume,
                        Symbol = item.Symbol
                    });
                });

                if (subscribeInstruments.Success)
                    _subcribesIntrumets.Add(item);

                if (!subscribeInstruments.Success)
                {
                    await _bufferUpdateSubscriptionFailed.SendAsync(item.Symbol);
                }
            }
            #region resubscribtion, if occured an issue

            var reSubscribeToInstuments = new ActionBlock<string>(async x =>
            {
                var result = await _bitfinexSocketClient.SpotStreams.SubscribeToTickerUpdatesAsync(x, async data =>
                {
                    await _bufferUpdateSubscription.SendAsync(new BitfinexTickerModel
                    {
                        BestAskPrice = data.Data.BestAskPrice,
                        BestBidPrice = data.Data.BestBidPrice,
                        BestAskQuantity = data.Data.BestAskQuantity,
                        BestBidQuantity = data.Data.BestBidQuantity,
                        DailyChange = data.Data.DailyChange,
                        DailyChangePercentage = data.Data.DailyChangePercentage,
                        HighPrice = data.Data.HighPrice,
                        LastPrice = data.Data.LastPrice,
                        LowPrice = data.Data.LowPrice,
                        Volume = data.Data.Volume,
                        Symbol = x
                    });
                });

                if (!result.Success)
                {
                    await _bufferUpdateSubscriptionFailed.SendAsync(x);
                }
            });

            _bufferUpdateSubscriptionFailed.LinkTo(reSubscribeToInstuments);
            #endregion
        }

        public async Task SubcribeToGlasses(TypeMarket typeMarket, List<AvalibleInstrumentsModel> avalibleInstrumentsModel, int levels = 100)
        {
            List<AvalibleInstrumentsModel>? newSubscribeGlases = avalibleInstrumentsModel
                .Except(_subcribesGlasses, new AvalibleInstrumentsModelComparer()).ToList();

            if (typeMarket.ToString().Equals("Spot"))
            {
                newSubscribeGlases.ForEach(async x => await OrderBookAsync(x.Symbol, levels));
            }
            if (typeMarket.ToString().Equals("UsdFutures"))
            {
                string devidedSymbol = String.Empty; string futureSymbol = String.Empty;

                foreach (var symbol in newSubscribeGlases)
                {
                    devidedSymbol = symbol.Symbol.Replace("usd", "");
                    futureSymbol = $"{devidedSymbol}"/*F0:USTF0*/;

                    await OrderBookAsync(futureSymbol, levels = 100);
                }
            }
        }
        private async Task<CallResult<UpdateSubscription>> OrderBookAsync(string symbol, int levels = 100)
        {
            BidsAndAsks ask = new BidsAndAsks();
            BidsAndAsks bid = new BidsAndAsks();
            var socketResult = await _bitfinexSocketClient.SpotStreams.SubscribeToRawOrderBookUpdatesAsync(symbol, levels, async data =>
            {
                foreach (var item in data.Data.ToList())
                {
                    ask = _orderBook.Asks.Where(x => x.Id == item.OrderId).FirstOrDefault();
                    bid = _orderBook.Bids.Where(x => x.Id == item.OrderId).FirstOrDefault();

                    if (item.Quantity > 0 && item.Price != 0 && bid == null)
                        _orderBook.Bids.Add(new BidsAndAsks { Price = item.Price, Quantity = item.Quantity, Id = item.OrderId });
                    else if (item.Quantity < 0 && item.Price != 0 && ask == null)
                        _orderBook.Asks.Add(new BidsAndAsks { Price = item.Price, Quantity = item.Quantity, Id = item.OrderId });
                    else if (item.Price == 0)
                    {
                        if (ask != null)
                            _orderBook.Asks.Remove(ask);
                        if (bid != null)
                            _orderBook.Bids.Remove(bid);
                    }
                    else if (item.Price > 0)
                    {
                        if (ask != null)
                        {
                            ask.Price = item.Price;
                            ask.Quantity = item.Quantity;
                        }
                        if (bid != null)
                        {
                            bid.Price = item.Price;
                            bid.Quantity = item.Quantity;
                        }
                    }
                    BufferGlass.Post(_orderBook);
                }
            });

            return socketResult;
        }
    }
}
