using CryptoCon.Kucoin.Enums;
using CryptoCon.Kucoin.Models;
using CryptoExchange.Net.CommonObjects;
using Kucoin.Net.Clients;
using Kucoin.Net.Objects;
using Kucoin.Net.Objects.Models.Futures;
using Kucoin.Net.Objects.Models.Spot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCon.Kucoin
{
    public class KucoinRestApi
    {
        private KucoinClient _kucoinClient { get; set; }

        private TypeMarket _typeMarket { get; set; }

        public KucoinRestApi(string key, string secretKey, string apiPassPhrase, TypeMarket typeMarket)
        {
            _kucoinClient = new KucoinClient(new KucoinClientOptions()
            {
                ApiCredentials = new KucoinApiCredentials(key, secretKey, apiPassPhrase)
            });

            _typeMarket = typeMarket;
        }

        public async Task<IEnumerable<KucoinTicker>> GetAllTickers()
        {
            if (_typeMarket.Equals(TypeMarket.Spot))
            {
                var tickersRequest = await _kucoinClient.SpotApi.ExchangeData.GetSymbolsAsync();
                if (tickersRequest.Success)
                {
                    return tickersRequest.Data.Select(x => new KucoinTicker()
                    {
                        BaseAsset = x.QuoteAsset,
                        QuoteAsset = x.BaseAsset,
                        LotSize = x.BaseMinQuantity,
                        LotSizeAccuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(x.BaseMinQuantity))),
                        Symbol = x.Symbol,
                        PriceSize = x.BaseIncrement,
                        PriceSizeAccuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(x.BaseIncrement))),
                        Status = x.EnableTrading ? true : false
                    });
                }
            }

            if (_typeMarket.Equals(TypeMarket.UsdFutures))
            {
                var tickersRequest = await _kucoinClient.FuturesApi.CommonFuturesClient.GetTickersAsync();
                if (tickersRequest.Success)
                {
                    return tickersRequest.Data.Select(x => new KucoinTicker()
                    {
                        Symbol = x.Symbol,
                        BaseAsset = ((KucoinContract)x.SourceObject).RootSymbol,
                        QuoteAsset = ((KucoinContract)x.SourceObject).BaseAsset,
                        LotSize = ((KucoinContract)x.SourceObject).LotSize,
                        LotSizeAccuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(((KucoinContract)x.SourceObject).LotSize))),
                        PriceSize = ((KucoinContract)x.SourceObject).IndexPriceTickSize,
                        PriceSizeAccuracy = (int)Math.Abs(Math.Log10(Convert.ToDouble(((KucoinContract)x.SourceObject).IndexPriceTickSize))),
                        Status = ((KucoinContract)x.SourceObject).Status.Equals("Open") ? true : false
                    });
                }
            }

            return new List<KucoinTicker>();
        }

    }
}
