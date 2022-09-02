using Adapter;
using CommonDataContract.ReactData;
using CryptoCon.Kucoin.Enums;
using CryptoCon.Kucoin.Models;
using CryptoExchange.Net.CommonObjects;
using SourceEts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Kucoin
{

    public class KucoinWrapper
    {
        private KucoinRestApi _kucoinRestApi { get; set; }
        private KucoinSocket _kucoinSocket { get; set; }

        private string Key { get; set; }
        private string SecretKey { get; set; }
        private string ApiPassPhrase { get; set; }

        private TypeMarket _typeMarket { get; set; }

        public AbstractTerminal _abstractTerminal { get; set; }

        public KucoinWrapper(string key, string secretKey, string apiPassPhrase, TypeMarket typeMarket, AbstractTerminal abstractTerminal)
        {
            Key = key;
            SecretKey = secretKey;
            ApiPassPhrase = apiPassPhrase;

            _abstractTerminal = abstractTerminal;

            _kucoinRestApi = new KucoinRestApi(key, secretKey, apiPassPhrase, typeMarket);
            _kucoinSocket = new KucoinSocket(key, secretKey, apiPassPhrase, typeMarket, abstractTerminal);
        }

        public async Task<IEnumerable<Securities>> GetAllTickersAsync()
        {
            var tickers = await _kucoinRestApi.GetAllTickers();

            return tickers.Where(st => st.Status)
                .Select(x => new Securities()
                {
                    Seccode = x.Symbol.ToUpper(),
                    ClassCode = "Kucoin",
                    IsCrypto = true,
                    BaseActive = x.BaseAsset,
                    ShortName = x.QuoteAsset,
                    TradingStatus = "Торгуется",
                    Status = "Открыта",
                    Accuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(x.PriceSizeAccuracy))),
                    LotSize = Convert.ToDouble(x.LotSize),
                    MinStep = Convert.ToDouble(x.PriceSize),
                    PointCost = Convert.ToDouble(x.PriceSize),
                    IsTrade = true,
                }).Where(ticker => !String.IsNullOrEmpty(ticker.Seccode) && !String.IsNullOrEmpty(ticker.BaseActive)).ToList();
        }

        public async Task SubcribeToInstrumentsAsync(List<AvalibleInstrumentsModel> avalibleInstrumentsModel)
        {
            await _kucoinSocket.SubscribeToInstrumentsAsync(avalibleInstrumentsModel);

            var transferTradesToTerminal = new ActionBlock<StreamTick>(data =>
            {
                Securities securities = new Securities()
                {
                    LastPrice = Convert.ToDouble(data.LastPrice),
                    Bid = Convert.ToDouble(data.BestBidPrice),
                    Offer = Convert.ToDouble(data.BestAskPrice),
                    TimeLastChange = data.Timestamp,
                    Seccode = data.Symbol
                };

                _abstractTerminal.AddSecurity(securities);
            });

            _kucoinSocket.BufferStreamTick.LinkTo(transferTradesToTerminal);
        }
    }
}
