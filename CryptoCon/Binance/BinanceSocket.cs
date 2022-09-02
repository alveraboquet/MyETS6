using Binance.Net.Clients;
using Binance.Net.Interfaces;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Futures.Socket;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoCon.Binance.Enums;
using CryptoCon.Binance.Models;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using SourceEts;
using SourceEts.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Binance
{
    public class BinanceSocket
    {
        private BinanceClient _binanceClient { get; set; }
        private BinanceSocketClient _binanceSocketClient { get; set; }

        #region Buffers Spot, UsdFutures, CoinFutures updates orders

        public BufferBlock<BinanceStreamOrderUpdate> BufferUpdateOrdersSpot { get; set; }
        public BufferBlock<BinanceFuturesStreamOrderUpdate> BufferUpdateOrdersUsdFutures { get; set; }

        #endregion

        #region Update balances Spot, UsdFutures, CoinFutures

        public BufferBlock<BinanceStreamPositionsUpdate> BufferBalanceUpdateSpot { get; set; }
        public BufferBlock<BinanceFuturesStreamAccountUpdate> BufferBalanceUpdateUsdFutures { get; set; }

        #endregion

        private List<AvalibleInstrumentsModel> _subcribesIntrumets { get; set; }
        private List<AvalibleInstrumentsModel> _subcribesGlasses { get; set; }

        public BufferBlock<IBinanceTick> BufferTickers { get; set; }
        public BufferBlock<IBinanceTick> BufferTickersFailed { get; set; }

        public BufferBlock<Trades> BufferTrades { get; set; }
        private BufferBlock<string> BufferTradesFailed { get; set; }
        private List<string> tradesSubscriptions { get; set; }

        private string Key { get; set; }
        private string SecretKey { get; set; }

        private TypeMarket _typeMarket { get; set; }

        // Стакан
        public BufferBlock<IBinanceOrderBook> BufferGlass { get; set; }

        public BinanceSocket(string key, string secretKey, TypeMarket typeMarket)
        {
            Key = key;
            SecretKey = secretKey;

            _typeMarket = typeMarket;

            _binanceClient = new BinanceClient(new BinanceClientOptions() { ApiCredentials = new ApiCredentials(key, secretKey) });

            _binanceSocketClient = new BinanceSocketClient(new BinanceSocketClientOptions() { AutoReconnect = true });

            BufferUpdateOrdersSpot = new BufferBlock<BinanceStreamOrderUpdate>();
            BufferUpdateOrdersUsdFutures = new BufferBlock<BinanceFuturesStreamOrderUpdate>();

            BufferBalanceUpdateSpot = new BufferBlock<BinanceStreamPositionsUpdate>();
            BufferBalanceUpdateUsdFutures = new BufferBlock<BinanceFuturesStreamAccountUpdate>();

            BufferTickers = new BufferBlock<IBinanceTick>();
            BufferTickersFailed = new BufferBlock<IBinanceTick>();

            _subcribesIntrumets = new List<AvalibleInstrumentsModel>();
            _subcribesGlasses = new List<AvalibleInstrumentsModel>();

            BufferGlass = new BufferBlock<IBinanceOrderBook>();

            BufferTrades = new BufferBlock<Trades>();
            BufferTradesFailed = new BufferBlock<string>();
            tradesSubscriptions = new List<string>();
        }

        public async Task SubscribeToUserAccountUpdatesAsync()
        {
            string listenKey = string.Empty;

            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                listenKey = (await _binanceClient.SpotApi.Account.StartUserStreamAsync()).Data;

                var subscribe = await _binanceSocketClient.SpotStreams.SubscribeToUserDataUpdatesAsync(listenKey,
                    onOrderUpdateMessage =>
                    {
                        // Handle order update

                        BufferUpdateOrdersSpot.Post(onOrderUpdateMessage.Data);
                    },
                    data =>
                    {
                        // Handle oco order update
                    },
                    onAccountPositionMessage =>
                    {
                        // Handle account balance update, caused by trading

                        BufferBalanceUpdateSpot.Post(onAccountPositionMessage.Data);

                    },
                    data =>
                    {
                        // Handle account balance update, caused by withdrawal/deposit or transfers
                    });
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                listenKey = (await _binanceClient.UsdFuturesApi.Account.StartUserStreamAsync()).Data;

                var subscribe = await _binanceSocketClient.UsdFuturesStreams.SubscribeToUserDataUpdatesAsync(listenKey,
                 data =>
                 {

                 },
                 data =>
                 {

                 },
                 data =>
                 {
                     // account update

                     BufferBalanceUpdateUsdFutures.Post(data.Data);
                 },
                 data =>
                 {
                     // order update

                     BufferUpdateOrdersUsdFutures.Post(data.Data);
                 },
                 data =>
                 {

                 });
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                listenKey = (await _binanceClient.CoinFuturesApi.Account.StartUserStreamAsync()).Data;

                var subscribe = await _binanceSocketClient.CoinFuturesStreams.SubscribeToUserDataUpdatesAsync(listenKey,
                data =>
                {

                },
                data =>
                {

                },
                data =>
                {
                     // account update

                     BufferBalanceUpdateUsdFutures.Post(data.Data);
                },
                data =>
                {
                     // order update

                     BufferUpdateOrdersUsdFutures.Post(data.Data);
                },
                data =>
                {

                });
            }
        }

        public async Task SubscribeToTickerUpdatesAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            List<AvalibleInstrumentsModel>? newSubscribeIntrumetns = avalibleInstrumentsModel
              .Except(_subcribesIntrumets, new AvalibleInstrumentsModelComparer()).ToList();

            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                if (newSubscribeIntrumetns.Count >= 150)
                {
                    int counter = 150;

                    var chunks = newSubscribeIntrumetns.GroupBy(_ => counter++ / 150).Select(v => v.ToList()).ToList();

                    List<BinanceSocketClient>? sockets = chunks.Select(i => new BinanceSocketClient(new BinanceSocketClientOptions())).ToList();

                    for (int i = 0; i < sockets.Count; i++)
                    {
                        var subscribes = await sockets[i].SpotStreams.SubscribeToTickerUpdatesAsync(chunks[i].Select(x => x.Symbol), data =>
                        {
                            BufferTickers.Post(data.Data);
                        });

                        if (subscribes.Success)
                        {
                            _subcribesIntrumets.AddRange(chunks[i]);
                        }
                    }
                }
                else
                {
                    var subscribeToIntstruments = await _binanceSocketClient.SpotStreams.SubscribeToTickerUpdatesAsync(newSubscribeIntrumetns.Select(x => x.Symbol), data =>
                    {
                        BufferTickers.Post(data.Data);
                    });

                    if (subscribeToIntstruments.Success)
                    {
                        _subcribesIntrumets.AddRange(newSubscribeIntrumetns);
                    }
                }
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                if (newSubscribeIntrumetns.Count >= 150)
                {
                    int counter = 150;

                    var chunks = newSubscribeIntrumetns.GroupBy(_ => counter++ / 150).Select(v => v.ToList()).ToList();

                    List<BinanceSocketClient>? sockets = chunks.Select(i => new BinanceSocketClient(new BinanceSocketClientOptions())).ToList();

                    for (int i = 0; i < sockets.Count; i++)
                    {
                        var subscribes = await sockets[i].UsdFuturesStreams.SubscribeToTickerUpdatesAsync(chunks[i].Select(x => x.Symbol), data =>
                        {
                            BufferTickers.Post(data.Data);
                        });

                        if (subscribes.Success)
                        {
                            _subcribesIntrumets.AddRange(chunks[i]);
                        }
                    }
                }
                else
                {
                    var subscribeToIntstruments = await _binanceSocketClient.UsdFuturesStreams.SubscribeToTickerUpdatesAsync(newSubscribeIntrumetns.Select(x => x.Symbol), data =>
                    {
                        BufferTickers.Post(data.Data);
                    });

                    if (subscribeToIntstruments.Success)
                    {
                        _subcribesIntrumets.AddRange(newSubscribeIntrumetns);
                    }
                }
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                if (newSubscribeIntrumetns.Count >= 150)
                {
                    int counter = 150;

                    var chunks = newSubscribeIntrumetns.GroupBy(_ => counter++ / 150).Select(v => v.ToList()).ToList();

                    List<BinanceSocketClient>? sockets = chunks.Select(i => new BinanceSocketClient(new BinanceSocketClientOptions())).ToList();

                    for (int i = 0; i < sockets.Count; i++)
                    {
                        var subscribes = await sockets[i].CoinFuturesStreams.SubscribeToTickerUpdatesAsync(chunks[i].Select(x => x.Symbol), data =>
                        {
                            BufferTickers.Post(data.Data);
                        });

                        if (subscribes.Success)
                        {
                            _subcribesIntrumets.AddRange(chunks[i]);
                        }
                    }
                }
                else
                {
                    var subscribeToIntstruments = await _binanceSocketClient.CoinFuturesStreams.SubscribeToTickerUpdatesAsync(newSubscribeIntrumetns.Select(x => x.Symbol), data =>
                    {
                        BufferTickers.Post(data.Data);
                    });

                    if (subscribeToIntstruments.Success)
                    {
                        _subcribesIntrumets.AddRange(newSubscribeIntrumetns);
                    }
                }
            }
        }

        public async Task SubcribeToOrderBookAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel, int levels = 20)
        {
            List<AvalibleInstrumentsModel>? newSubscribeGlases = avalibleInstrumentsModel
                .Except(_subcribesGlasses, new AvalibleInstrumentsModelComparer()).ToList();

                newSubscribeGlases.ForEach(async x => await OrderBookAsync(x.Symbol, levels));
        }

        private async Task<CallResult<UpdateSubscription>> OrderBookAsync(string symbol, int levels = 20)
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                return await _binanceSocketClient.SpotStreams.SubscribeToPartialOrderBookUpdatesAsync(symbol, levels, 100, updateGlass =>
                {
                    BufferGlass.SendAsync(updateGlass.Data);
                });
            }
            else if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                return await _binanceSocketClient.UsdFuturesStreams.SubscribeToPartialOrderBookUpdatesAsync(symbol, levels, 100, updateGlass =>
                {
                    BufferGlass.SendAsync(updateGlass.Data);
                });
            }
            else
            {
                return await _binanceSocketClient.CoinFuturesStreams.SubscribeToPartialOrderBookUpdatesAsync(symbol, levels, 100, updateGlass =>
                {
                    BufferGlass.SendAsync(updateGlass.Data);
                });
            }
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
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var tradesSubscribe = await _binanceSocketClient.SpotStreams.SubscribeToTradeUpdatesAsync(symbol, trades =>
                {
                    BufferTrades.Post(new Trades()
                    {
                        Id = trades.Data.Id,
                        IsSell = trades.Data.BuyerIsMaker,
                        Price = trades.Data.Price,
                        Quantity = trades.Data.Quantity,
                        Symbol = trades.Data.Symbol,
                        TradeTime = trades.Data.TradeTime
                    });
                });

                if (tradesSubscribe.Success)
                    tradesSubscriptions.Add(symbol);
                else
                    BufferTradesFailed.Post(symbol);
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var tradesSubscribe = await _binanceSocketClient.UsdFuturesStreams.SubscribeToAggregatedTradeUpdatesAsync(symbol, trades =>
                {
                    BufferTrades.Post(new Trades()
                    {
                        Id = trades.Data.Id,
                        IsSell = trades.Data.BuyerIsMaker,
                        Price = trades.Data.Price,
                        Quantity = trades.Data.Quantity,
                        Symbol = trades.Data.Symbol,
                        TradeTime = trades.Data.TradeTime
                    });
                });

                if (tradesSubscribe.Success)
                    tradesSubscriptions.Add(symbol);
                else
                    BufferTradesFailed.Post(symbol);
            }

            if (_typeMarket.Equals(TypeMarket.CoinFutures))
            {
                var tradesSubscribe = await _binanceSocketClient.CoinFuturesStreams.SubscribeToAggregatedTradeUpdatesAsync(symbol, trades =>
                {
                    BufferTrades.Post(new Trades()
                    {
                        Id = trades.Data.Id,
                        IsSell = trades.Data.BuyerIsMaker,
                        Price = trades.Data.Price,
                        Quantity = trades.Data.Quantity,
                        Symbol = trades.Data.Symbol,
                        TradeTime = trades.Data.TradeTime
                    });
                });

                if (tradesSubscribe.Success)
                    tradesSubscriptions.Add(symbol);
                else
                    BufferTradesFailed.Post(symbol);
            }
        }
    }
}
