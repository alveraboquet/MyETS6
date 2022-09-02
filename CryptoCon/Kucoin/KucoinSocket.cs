using Adapter;
using CryptoCon.Kucoin.Enums;
using CryptoCon.Kucoin.Models;
using Kucoin.Net.Clients;
using Kucoin.Net.Objects;
using SourceEts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Kucoin
{
    public class KucoinSocket
    {
        private KucoinSocketClient _kucoinSocket { get; set; }

        private string Key { get; set; }
        private string SecretKey { get; set; }
        private string ApiPassPhrase { get; set; }
        private TypeMarket _typeMarket { get; set; }

        private AbstractTerminal _abstractTerminal { get; set; }

        #region Инструменты

        public BufferBlock<StreamTick> BufferStreamTick { get; set; }
        private BufferBlock<string> symbolsFailed { get; set; }
        private List<string> symbolsTrue { get; set; }

        #endregion

        public KucoinSocket(string key, string secretKey, string apiPassPhrase, TypeMarket typeMarket, AbstractTerminal abstractTerminal)
        {
            Key = key;
            SecretKey = secretKey;
            ApiPassPhrase = apiPassPhrase;

            _typeMarket = typeMarket;
            _abstractTerminal = abstractTerminal;

            _kucoinSocket = new KucoinSocketClient(new KucoinSocketClientOptions()
            {
                ApiCredentials = new KucoinApiCredentials(key, secretKey, apiPassPhrase)
            });

            #region Инструменты

            symbolsTrue = new List<string>();
            symbolsFailed = new BufferBlock<string>();
            BufferStreamTick = new BufferBlock<StreamTick>();

            #endregion
        }

        public async Task SubscribeToInstrumentsAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            var newSubscribeIntrumetns = avalibleInstrumentsModel.Select(x => x.Symbol).Except(symbolsTrue).ToList();
            if (newSubscribeIntrumetns.Any())
            {
                newSubscribeIntrumetns.ForEach(async symbol => await SubscribeToTickerAsync(symbol));
            }

            var failedSub = new ActionBlock<string>(async x =>
            {
                //_abstractTerminal.BaseMon.Raise_OnSomething($"Kucoin SubscribeToInstrumentsAsync: переподписка на инструмент { x }");
                await SubscribeToTickerAsync(x);
            });

            symbolsFailed.LinkTo(failedSub);
        }

        public async Task SubscribeToTickerAsync(string symbol)
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var subscribe = await _kucoinSocket.SpotStreams.SubscribeToTickerUpdatesAsync(symbol, tick =>
                {
                    BufferStreamTick.Post(new StreamTick()
                    {
                        BestAskPrice = tick.Data.BestAskPrice,
                        BestBidPrice = tick.Data.BestBidPrice,
                        LastPrice = tick.Data.LastPrice,
                        Symbol = tick.Data.Symbol,
                        Timestamp = tick.Data.Timestamp
                    });
                });

                if (!subscribe.Success)
                    symbolsFailed.Post(symbol);
                else
                    symbolsTrue.Add(symbol);
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var subscribe = await _kucoinSocket.FuturesStreams.SubscribeToTickerUpdatesAsync(symbol, tick =>
                {
                    BufferStreamTick.Post(new StreamTick()
                    {
                        BestAskPrice = tick.Data.BestAskPrice,
                        BestBidPrice = tick.Data.BestBidPrice,
                        Symbol = tick.Data.Symbol,
                        Timestamp = tick.Data.Timestamp
                    });
                });

                if (!subscribe.Success)
                    symbolsFailed.Post(symbol);
                else
                    symbolsTrue.Add(symbol);
            }
        }
    }
}
