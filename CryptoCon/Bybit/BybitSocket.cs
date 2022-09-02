using Adapter.Config;
using Bybit.Net.Clients;
using Bybit.Net.Enums;
using Bybit.Net.Objects;
using Bybit.Net.Objects.Models;
using Bybit.Net.Objects.Models.Socket;
using Bybit.Net.Objects.Models.Socket.Spot;
using CryptoCon.Binance.Models;
using CryptoCon.FTX.Models;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using SourceEts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Trades = CryptoCon.Binance.Models.Trades;

namespace CryptoCon.Bybit
{
    public class BybitSocket
    {
        public BufferBlock<BybitSpotAccountUpdate> BufferSpotAccountUpdate { get; set; }
        public BufferBlock<BybitSpotOrderUpdate> BufferSpotOrderUpdate { get; set; }
        public BufferBlock<IEnumerable<BybitUsdPerpetualOrderUpdate>> BufferUsdFuturesOrderUpdate { get; set; }
        public BufferBlock<BybitSpotUserTradeUpdate> BufferSpotUserTradeUpdate { get; set; }

        public BufferBlock<BybitSpotTickerUpdate> BufferSpotTickers { get; set; }
        public BufferBlock<BybitTickerUpdate> BufferUsdFuturesTickers { get; set; }
        public BufferBlock<IEnumerable<BybitBalanceUpdate>> BufferBalanceUpdateUsdFutures { get; set; }

        public BufferBlock<BybitSpotOrderBookUpdate> BufferSpotGlass { get; set; }
        public BufferBlock<IEnumerable<BybitOrderBookEntry>> BufferUsdFuturesGlass { get; set; }


        public BufferBlock<Trades> BufferTrades { get; set; }
        private BufferBlock<string> BufferTradesFailed { get; set; }



        private BybitSocketClient _socketClient;
        private List<AvalibleInstrumentsModel> _subcribedIntrumets;
        private List<AvalibleInstrumentsModel> _subcribedGlasses;
        private List<string> _tradesSubscriptions;
        public BybitSocket()
        {
            _socketClient = new BybitSocketClient();
            Init();
        }

        public BybitSocket(BybitSocketClientOptions options)
        {
            _socketClient = new BybitSocketClient(options);
            Init();
        }

        private void Init()
        {
            _subcribedIntrumets = new List<AvalibleInstrumentsModel>();
            _subcribedGlasses = new List<AvalibleInstrumentsModel>();
            BufferSpotTickers = new BufferBlock<BybitSpotTickerUpdate>();
            BufferUsdFuturesTickers = new BufferBlock<BybitTickerUpdate>();
            BufferSpotAccountUpdate = new BufferBlock<BybitSpotAccountUpdate>();
            BufferSpotOrderUpdate = new BufferBlock<BybitSpotOrderUpdate>();
            BufferSpotUserTradeUpdate = new BufferBlock<BybitSpotUserTradeUpdate>();
            BufferBalanceUpdateUsdFutures = new BufferBlock<IEnumerable<BybitBalanceUpdate>>();
            BufferUsdFuturesOrderUpdate = new BufferBlock<IEnumerable<BybitUsdPerpetualOrderUpdate>>();
            BufferSpotGlass = new BufferBlock<BybitSpotOrderBookUpdate>();
            BufferUsdFuturesGlass = new BufferBlock<IEnumerable<BybitOrderBookEntry>>();
            BufferTrades = new BufferBlock<Trades>();
            BufferTradesFailed = new BufferBlock<string>();
            _tradesSubscriptions = new List<string>();
        }

        public void SubscribeToTradesAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel, string typeMarket)
        {
            var newSubscribeTrades = avalibleInstrumentsModel.Select(x => x.Symbol).Except(_tradesSubscriptions).ToList();

            if (typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                if (newSubscribeTrades.Any())
                    newSubscribeTrades.ForEach(async x => await SubscribeToTradeUpdatesAsync(x));

                var subFailedToTrades = new ActionBlock<string>(async symbolFailed =>
                {
                    await SubscribeToTradeUpdatesAsync(symbolFailed);
                });

                BufferTradesFailed.LinkTo(subFailedToTrades); 
            }
            else if (typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                if (newSubscribeTrades.Any())
                    newSubscribeTrades.ForEach(async x => await SubscribeToUsdFuturesTradeUpdatesAsync(x));

                var subFailedToTrades = new ActionBlock<string>(async symbolFailed =>
                {
                    await SubscribeToUsdFuturesTradeUpdatesAsync(symbolFailed);
                });

                BufferTradesFailed.LinkTo(subFailedToTrades);
            }
        }

        /// <summary>
        /// Подписка на обновления аккаунта
        /// </summary>
        /// <param name="typeMarket">тип инструмента (спот/фьючерс)</param>
        /// <param name="ct">токен отмены асинхронной операции</param>
        /// <returns>Подписка на поток данных. Данная подписка обновляется при разрыве соединения, переподключении</returns>
        public async Task SubscribeToAccountUpdatesAsync(string typeMarket, CancellationToken ct = default)
        {
            if (typeMarket == ConfigTermins.TypeCryptoAccountSpot)
                await _socketClient.SpotStreams.SubscribeToAccountUpdatesAsync(
                accountUpdateHandler =>
                {
                    BufferSpotAccountUpdate.Post(accountUpdateHandler.Data);
                },
                orderUpdateHandler =>
                {
                    BufferSpotOrderUpdate.Post(orderUpdateHandler.Data);
                },
                tradeUpdateHandler =>
                {
                    BufferSpotUserTradeUpdate.Post(tradeUpdateHandler.Data);
                }, ct);
            else if (typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                var balanceTask = _socketClient.UsdPerpetualStreams.SubscribeToBalanceUpdatesAsync(data => BufferBalanceUpdateUsdFutures.Post(data.Data), ct);
                var orderTask = _socketClient.UsdPerpetualStreams.SubscribeToOrderUpdatesAsync(data => BufferUsdFuturesOrderUpdate.Post(data.Data), ct);
                Task.WaitAll(balanceTask, orderTask);
            }
        }

        /// <summary>
        /// Подписка на обновления по указанному инструменту
        /// </summary>
        /// <param name="symbol">тикер инструмента</param>
        /// <param name="ct">токен отмены асинхронной операции</param>
        /// <returns>Подписка на поток данных. Данная подписка обновляется при разрыве соединения, переподключении</returns>
        private async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, CancellationToken ct = default)
        {
            return await _socketClient.SpotStreams.SubscribeToTickerUpdatesAsync(symbol, data => BufferSpotTickers.Post(data.Data), ct);
        }

        /// <summary>
        /// Подписка на обновления инструмента
        /// </summary>
        /// <param name="avalibleInstrumentsModel">коллекция тикеров инструментов</param>
        /// <param name="ct">токен отмены асинхронной операции</param>
        /// <returns>Подписка на поток данных. Данная подписка обновляется при разрыве соединения, переподключении</returns>
        public async void SubcribeToInstrumentsAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel, string typeMarket, CancellationToken ct = default)
        {
            var instrs = avalibleInstrumentsModel.Select(x => x.Symbol).ToList();
            List<AvalibleInstrumentsModel>? newSubscribeIntrumetns = avalibleInstrumentsModel.Except(_subcribedIntrumets, new AvalibleInstrumentsModelComparer()).ToList();

            Dictionary<string, Task<CallResult<UpdateSubscription>>> tasks = new Dictionary<string, Task<CallResult<UpdateSubscription>>>();

            if (typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                instrs.ForEach(x => tasks.Add(x, SubscribeToTickerUpdatesAsync(x, ct)));

                Task.WaitAll(tasks.Values.ToArray());

                List<string> resubscribeInstrs = new List<string>();

                foreach (var item in tasks)
                {
                    if (!item.Value.Result.Success)
                    {
                        var subscribed = _subcribedIntrumets.Select(x => x.Symbol).ToList();
                        if (!subscribed.Contains(item.Key))
                        {
                            AvalibleInstrumentsModel model = avalibleInstrumentsModel.Find(x => x.Symbol == item.Key);

                            if (model != null)
                                _subcribedIntrumets.Add(model);
                        }
                    }
                    else
                        resubscribeInstrs.Add(item.Key);
                }

                tasks.Clear();

                resubscribeInstrs.ForEach(x => tasks.Add(x, SubscribeToTickerUpdatesAsync(x, ct)));

                Task.WaitAll(tasks.Values.ToArray());
            }
            else if (typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                var result = await SubscribeToUsdFuturesTickerUpdatesAsync(instrs);

                if (result.Success)
                {
                    instrs.ForEach(x =>
                    {
                        AvalibleInstrumentsModel model = avalibleInstrumentsModel.Find(y => y.Symbol == x);

                        if (model != null)
                            _subcribedIntrumets.Add(model);
                    });
                }
            }
        }

        /// <summary>
        /// Подписка на обновления стакана. Глубина стакана 40 строк, без возможности настраивать
        /// </summary>
        /// <param name="symbol">тикер инструмента</param>
        /// <param name="ct">токен отмены асинхронной операции</param>
        /// <returns>Подписка на поток данных. Данная подписка обновляется при разрыве соединения, переподключении</returns>
        private async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, CancellationToken ct = default)
        {
            return await _socketClient.SpotStreams.SubscribeToOrderBookUpdatesAsync(symbol, updateGlass => BufferSpotGlass.Post(updateGlass.Data), ct);
        }

        /// <summary>
        /// Подписка на обновление стаканов
        /// </summary>
        /// <param name="limit">глубина стакана от 25 до 200 строк.</param>
        /// <param name="snapshotHandler">Обработчик событий для исходных данных моментального снимка</param>
        /// <param name="updateHandler">Обработчик событий для сообщений об обновлении</param>
        /// <param name="ct">токен отмены асинхронной операции</param>
        /// <returns>Подписка на поток данных. Данная подписка обновляется при разрыве соединения, переподключении</returns>
        private async Task<CallResult<UpdateSubscription>> SubscribeToOrderBooksUpdatesAsync(int limit, Action<DataEvent<IEnumerable<BybitOrderBookEntry>>> snapshotHandler, Action<DataEvent<BybitDeltaUpdate<BybitOrderBookEntry>>> updateHandler, CancellationToken ct = default)
        {
            var result = await _socketClient.UsdPerpetualStreams.SubscribeToOrderBooksUpdatesAsync(limit, snapshotHandler, updateHandler, ct);
            return result;
        }

        /// <summary>
        /// Подписка на обновление стакана по указанному инструменту
        /// </summary>
        /// <param name="symbol">тикер инструмента</param>
        /// <param name="limit">глубина стакана от 25 до 200 строк.</param>
        /// <param name="snapshotHandler">Обработчик событий для исходных данных моментального снимка</param>
        /// <param name="updateHandler">Обработчик событий для сообщений об обновлении</param>
        /// <param name="ct">токен отмены асинхронной операции</param>
        /// <returns>Подписка на поток данных. Данная подписка обновляется при разрыве соединения, переподключении</returns>
        private async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int limit, Action<DataEvent<IEnumerable<BybitOrderBookEntry>>> snapshotHandler, Action<DataEvent<BybitDeltaUpdate<BybitOrderBookEntry>>> updateHandler, CancellationToken ct = default)
        {
            return await _socketClient.UsdPerpetualStreams.SubscribeToOrderBookUpdatesAsync(symbol, limit, snapshotHandler, updateHandler, ct);
        }

        /// <summary>
        /// Подписка на обновление стаканов по указанныи инструментам
        /// </summary>
        /// <param name="symbols">коллекция тикеров инструментов</param>
        /// <param name="limit">глубина стакана от 25 до 200 строк.</param>
        /// <param name="snapshotHandler">Обработчик событий для исходных данных моментального снимка</param>
        /// <param name="updateHandler">Обработчик событий для сообщений об обновлении</param>
        /// <param name="ct">токен отмены асинхронной операции</param>
        /// <returns>Подписка на поток данных. Данная подписка обновляется при разрыве соединения, переподключении</returns>
        private async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(IEnumerable<string> symbols, int limit, Action<DataEvent<IEnumerable<BybitOrderBookEntry>>> snapshotHandler, Action<DataEvent<BybitDeltaUpdate<BybitOrderBookEntry>>> updateHandler, CancellationToken ct = default)
        {
            return await _socketClient.UsdPerpetualStreams.SubscribeToOrderBookUpdatesAsync(symbols, limit, snapshotHandler, updateHandler, ct);
        }

        /// <summary>
        /// Подписка на обновления стакана.
        /// </summary>
        /// <param name="typeMarket">тип инструмента (спот, фьючерсы и маржинальный)</param>
        /// <param name="avalibleInstrumentsModel">коллекция инструментов</param>
        /// <returns>Подписка на поток данных. Данная подписка обновляется при разрыве соединения, переподключении</returns>
        public async Task SubcribeToGlasses(string typeMarket, List<AvalibleInstrumentsModel> avalibleInstrumentsModel, int levels)
        {
            List<AvalibleInstrumentsModel>? newSubscribeGlases = avalibleInstrumentsModel.Except(_subcribedGlasses, new AvalibleInstrumentsModelComparer()).ToList();

            if (typeMarket == ConfigTermins.TypeCryptoAccountSpot)
            {
                Dictionary<string, Task<CallResult<UpdateSubscription>>> tasks = new Dictionary<string, Task<CallResult<UpdateSubscription>>>();
                newSubscribeGlases.ForEach(x => tasks.Add(x.Symbol, SubscribeToOrderBookUpdatesAsync(x.Symbol)));

                Task.WaitAll(tasks.Values.ToArray());

                List<string> resubscribeGlasses = new List<string>();

                foreach (var item in tasks)
                {
                    if (item.Value.Result.Success)
                    {
                        var subscribed = _subcribedGlasses.Select(x => x.Symbol).ToList();
                        if (!subscribed.Contains(item.Key))
                        {
                            AvalibleInstrumentsModel model = avalibleInstrumentsModel.Find(x => x.Symbol == item.Key);

                            if (model != null)
                                _subcribedGlasses.Add(model);
                        }
                    }
                    else
                        resubscribeGlasses.Add(item.Key);
                }

                tasks.Clear();
                resubscribeGlasses.ForEach(x => tasks.Add(x, SubscribeToOrderBookUpdatesAsync(x)));

                Task.WaitAll(tasks.Values.ToArray());
            }
            else if (typeMarket == ConfigTermins.TypeCryptoAccountFutures)
            {
                var resuilt = await SubscribeToUsdFuturesOrderBookUpdatesAsync(avalibleInstrumentsModel.Select(x => x.Symbol), levels);

                if (resuilt.Success)
                    _subcribedGlasses.AddRange(avalibleInstrumentsModel);
            }
        }

        public async Task SubscribeToTradeUpdatesAsync(string symbol, CancellationToken ct = default)
        {
            var result = await _socketClient.SpotStreams.SubscribeToTradeUpdatesAsync(symbol, trades =>
            {
                BufferTrades.Post(new Trades()
                {
                    Id = trades.Data.Id,
                    IsSell = trades.Data.Buy == true ? false : true,
                    Price = trades.Data.Price,
                    Quantity = trades.Data.Quantity,
                    Symbol = symbol,
                    TradeTime = trades.Data.Timestamp
                });
            }, ct);

            if (result.Success)
                _tradesSubscriptions.Add(symbol);
            else
                BufferTradesFailed.Post(symbol);
        }

        #region UsdFutures functions

        private async Task<CallResult<UpdateSubscription>> SubscribeToUsdFuturesTickersUpdatesAsync(CancellationToken ct = default)
        {
            return await _socketClient.UsdPerpetualStreams.SubscribeToTickersUpdatesAsync(data =>
            {

            }, ct);
        }

        private async Task<CallResult<UpdateSubscription>> SubscribeToUsdFuturesTickerUpdatesAsync(string symbol, CancellationToken ct = default)
        {
            return await _socketClient.UsdPerpetualStreams.SubscribeToTickerUpdatesAsync(symbol, data =>
            {

            }, ct);
        }

        private async Task<CallResult<UpdateSubscription>> SubscribeToUsdFuturesTickerUpdatesAsync(IEnumerable<string> symbols, CancellationToken ct = default)
        {
            return await _socketClient.UsdPerpetualStreams.SubscribeToTickerUpdatesAsync(symbols, data =>
            {
                BufferUsdFuturesTickers.Post(data.Data);
            }, ct);
        }

        public async Task<CallResult<UpdateSubscription>> SubscribeToUsdFuturesOrderBookUpdatesAsync(IEnumerable<string> symbols, int limit, CancellationToken ct = default)
        {
            //var instrs = await new BybitClient().UsdPerpetualApi.ExchangeData.GetSymbolsAsync();
            var result = await _socketClient.UsdPerpetualStreams.SubscribeToOrderBookUpdatesAsync(symbols, limit, snapshotHandler => { },
                data => BufferUsdFuturesGlass.Post(data.Data.Update));
            return result;
        }

        public async Task SubscribeToUsdFuturesTradeUpdatesAsync(string symbol, CancellationToken ct = default)
        {
            var result = await _socketClient.UsdPerpetualStreams.SubscribeToTradeUpdatesAsync(symbol, trades =>
            {
                foreach (var item in trades.Data)
                {
                    BufferTrades.Post(new Trades()
                    {
                        Id = !string.IsNullOrEmpty(item.Id) ? Convert.ToInt64(item.Id) : 0,
                        IsSell = item.Side == OrderSide.Buy ? false : true,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Symbol = symbol,
                        TradeTime = item.Timestamp
                    });
                }
            }, ct);

            if (result.Success)
                _tradesSubscriptions.Add(symbol);
            else
                BufferTradesFailed.Post(symbol);
        }

        #endregion
    }
}
