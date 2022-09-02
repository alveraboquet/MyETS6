using Adapter;
//using CryptoCon.Extension;
using CryptoCon.Huobi.Models;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Huobi.Net.Clients;
using Huobi.Net.Objects;
using Huobi.Net.Objects.Models;
using Huobi.Net.Objects.Models.Socket;
using SourceEts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Huobi.Clients
{
    public class HuobiSocket
    {
        private HuobiSocketClient _huobiSocketClient { get; set; }

        #region Буферы по обновлению ордеров

        public BufferBlock<HuobiSubmittedOrderUpdate> BufferSubmittedOrderUpdate { get; set; }
        public BufferBlock<HuobiCanceledOrderUpdate> BufferCancelledOrderUpdate { get; set; }
        public BufferBlock<HuobiMatchedOrderUpdate> BufferMatchedOrderUpdate { get; set; }

        #endregion

        #region Буферы совершенных сделок и тиков (удачных и не удачных)

        public BufferBlock<TradesUpdate> BufferTrades { get; set; }
        private BufferBlock<string> _bufferTradesFailed { get; set; }

        public BufferBlock<HuobiSymbolTick> BufferUpdateSubscription { get; set; }
        private BufferBlock<string> _bufferUpdateSubscriptionFailed { get; set; }

        #endregion

        public BufferBlock<HuobiGlass> BufferGlass { get; set; }


        #region Списки данных для таблицы

        private List<AvalibleInstrumentsModel> _subcribesIntrumets { get; set; }
        private List<AvalibleInstrumentsModel> _subcribesTrades { get; set; }
        private List<AvalibleInstrumentsModel> _subcribesGlasses { get; set; }

        #endregion

        public BufferBlock<HuobiAccountUpdate> BufferBalances { get; set; }

        public HuobiSocket(string key, string secretKey)
        {
            _huobiSocketClient = new HuobiSocketClient(new HuobiSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials(key, secretKey),
                AutoReconnect = true,
            });

            BufferSubmittedOrderUpdate = new BufferBlock<HuobiSubmittedOrderUpdate>();
            BufferCancelledOrderUpdate = new BufferBlock<HuobiCanceledOrderUpdate>();
            BufferMatchedOrderUpdate = new BufferBlock<HuobiMatchedOrderUpdate>();

            BufferBalances = new BufferBlock<HuobiAccountUpdate>();

            BufferTrades = new BufferBlock<TradesUpdate>();
            _bufferTradesFailed = new BufferBlock<string>();

            BufferUpdateSubscription = new BufferBlock<HuobiSymbolTick>();
            _bufferUpdateSubscriptionFailed = new BufferBlock<string>();

            _subcribesIntrumets = new List<AvalibleInstrumentsModel>();
            _subcribesTrades = new List<AvalibleInstrumentsModel>();
            _subcribesGlasses = new List<AvalibleInstrumentsModel>();

            BufferGlass = new BufferBlock<HuobiGlass>();
        }

        public async Task SubscribeToOrderUpdatesAsync(string? symbol = null)
        {
            for (int i = 0; i < int.MaxValue; i++)
            {
                var subscribe = await _huobiSocketClient.SpotStreams.SubscribeToOrderUpdatesAsync(
                    symbol,
                    data =>
                    {
                        // Handle order submitted update
                        BufferSubmittedOrderUpdate.Post(data.Data);
                    },
                    data =>
                    {
                        // Handle order matched update

                        BufferMatchedOrderUpdate.Post(data.Data);
                    },
                    data =>
                    {
                        // Handle order cancel update

                        BufferCancelledOrderUpdate.Post(data.Data);
                    },
                    data =>
                    {
                        // Handle conditional order trigger failure update
                    },
                    data =>
                    {
                        // Handle conditional order canceled update

                    });

                if (!subscribe.Success)
                {
                    //AbstractTerminal.BaseMon.Raise_OnSomething($"SubscribeToOrderUpdatesAsync is failed: {DateTime.UtcNow}");

                    await Task.Delay(1000);
                }
                else { /*AbstractTerminal.BaseMon.Raise_OnSomething($"SubscribeToOrderUpdatesAsync is true: {DateTime.UtcNow}");*/ break; }
            }
        }

        public async Task SubcribeToInstrumentsAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            List<AvalibleInstrumentsModel>? newSubscribeIntrumetns = avalibleInstrumentsModel
                .Except(_subcribesIntrumets, new AvalibleInstrumentsModelComparer()).ToList();

            foreach (var item in newSubscribeIntrumetns)
            {
                var subscribeInstruments = await _huobiSocketClient.SpotStreams.SubscribeToTickerUpdatesAsync(item.Symbol.Replace("_", String.Empty).ToLower(), data =>
                {
                    data.Data.Symbol = item.Symbol.ToUpper();
                    var res = BufferUpdateSubscription.Post(data.Data);

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
                var result = await _huobiSocketClient.SpotStreams.SubscribeToTickerUpdatesAsync(x.Replace("_", String.Empty).ToLower(), data =>
                {
                    BufferUpdateSubscription.Post(data.Data);
                });

                if (!result.Success)
                {
                    _bufferUpdateSubscriptionFailed.Post(x);
                }
            });

            _bufferUpdateSubscriptionFailed.LinkTo(reSubscribeToInstuments);

            #endregion
        }

        public async Task SubscribeToTradesAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            List<AvalibleInstrumentsModel>? newSubscribeToTrades = avalibleInstrumentsModel
                .Except(_subcribesIntrumets, new AvalibleInstrumentsModelComparer()).ToList();

            foreach (var item in newSubscribeToTrades)
            {
                var subscribesToTrades = await _huobiSocketClient.SpotStreams.SubscribeToTradeUpdatesAsync(item.Symbol.Replace("_", String.Empty).ToLower(), data =>
                {
                    TradesUpdate tradesUpdate = new TradesUpdate()
                    {
                        Symbol = item.Symbol,
                        Details = data.Data.Details
                    };

                    BufferTrades.Post(tradesUpdate);
                });

                if (subscribesToTrades.Success)
                    _subcribesTrades.Add(item);

                if (!subscribesToTrades.Success)
                {
                    _bufferTradesFailed.Post(item.Symbol);
                }
            }

            #region Переподписка на совершенные сделки, в случае ошибки

            var reSubscribeToTrades = new ActionBlock<string>(async x =>
            {
                var result = await _huobiSocketClient.SpotStreams.SubscribeToTradeUpdatesAsync(x.Replace("_", String.Empty).ToLower(), data =>
                {
                    TradesUpdate tradesUpdate = new TradesUpdate()
                    {
                        Symbol = x,
                        Details = data.Data.Details
                    };

                    BufferTrades.Post(tradesUpdate);
                });

                if (!result.Success)
                {
                    _bufferTradesFailed.Post(x);
                }
            });

            _bufferTradesFailed.LinkTo(reSubscribeToTrades);

            #endregion
        }

        public async Task SubcribeToBalancesAccountAsync()
        {
            for (int i = 0; i < int.MaxValue; i++)
            {
                var accountSocketSubscribe = await _huobiSocketClient.SpotStreams.SubscribeToAccountUpdatesAsync(account =>
                {
                    BufferBalances.Post(account.Data);
                });

                if (!accountSocketSubscribe.Success)
                {
                    await Task.Delay(200);
                }
                else { break; }
            }
        }

        public async Task SubscribeToOrderBookAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel, int levels = 20)
        {
            List<AvalibleInstrumentsModel>? newSubscribeIntrumetns = avalibleInstrumentsModel
                .Except(_subcribesGlasses, new AvalibleInstrumentsModelComparer()).ToList();

            newSubscribeIntrumetns.ForEach(async x => await OrderBookAsync(x.Symbol, levels));
        }

        private async Task<CallResult<UpdateSubscription>> OrderBookAsync(string symbol, int level = 20)
        {
            return await _huobiSocketClient.SpotStreams.SubscribeToPartialOrderBookUpdates100MilisecondAsync(symbol, level, updateGlass =>
            {
                BufferGlass.SendAsync(new HuobiGlass()
                {
                    Asks = updateGlass.Data.Asks,
                    Bids = updateGlass.Data.Bids,
                    Symbol = symbol.ToUpper(),
                    Timestamp = updateGlass.Data.Timestamp,
                });
            });
        }
    }
}
