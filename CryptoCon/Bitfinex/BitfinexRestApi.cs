using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using CommonDataContract;
using CryptoCon.Bitfinex.Enums;
using CryptoCon.Bitfinex.Models;
using CryptoCon.FTX.Models;
using CryptoExchange.Net.Authentication;
using FTX.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Bitfinex
{
    public class BitfinexRestApi
    {
        private BitfinexClient _restClient { get; set; }
        private TypeMarket _typeMarket { get; set; }

        //public BufferBlock<IEnumerable<Kline>> BubberKlines { get; set; }
        //private BufferBlock<string> _bufferKlineFailed { get; set; }


        //public BufferBlock<FTXOrder> BufferPlaceOrders { get; set; }
        //public BufferBlock<CancelOrder> BufferCancelOrders { get; set; }
        public BufferBlock<CancelOrder> BufferCancelOrders { get; set; }
        public BufferBlock<BitfinexWriteResult<BitfinexOrder>> BufferPlaceOrders { get; set; }
        public BitfinexRestApi(string key, string secretKey, TypeMarket typeMarket)
        {
            _restClient = new BitfinexClient(new BitfinexClientOptions()
            {
                ApiCredentials = new ApiCredentials(key, secretKey)
            });

            _typeMarket = typeMarket;
            BufferPlaceOrders = new BufferBlock<BitfinexWriteResult<BitfinexOrder>>();
            BufferCancelOrders = new BufferBlock<CancelOrder>();
        }
        public async Task<List<Assets>> GetTickersAsync()
        {
            var instruments = new List<Assets>();
            var instrumentsRequest = await _restClient.SpotApi.ExchangeData.GetSymbolsAsync();

            var getTickers = await _restClient.SpotApi.CommonSpotClient.GetTickersAsync();
            var allSymbols = getTickers.Data.Where(x => !x.Symbol.Substring(0, 1).Contains("f")).ToList();

            BitfinexSymbolDetails details = new();
            if (_typeMarket.ToString().Equals("UsdFutures"))
            {
                var futures = getTickers.Data.Where(x => x.Symbol.Contains("F0")).ToList();

                //var res = assest.Data.Where(x => x.Symbol.Contains(""));
                foreach (var future in futures)
                {
                    details = await FindSymboldetails(future.Symbol);
                    instruments.Add(new Assets
                    {
                        Symbol = future.Symbol,
                        Expiration = details.Expiration,
                        InitialMargin = details.InitialMargin,
                        Margin = details.Margin,
                        MaximumOrderQuantity = details.MaximumOrderQuantity,
                        MinimumMargin = details.MinimumMargin,
                        MinimumOrderQuantity = details.MinimumOrderQuantity,
                        PricePrecision = details.PricePrecision
                    });
                }


                if (instrumentsRequest.Success)
                {
                    return instruments;
                }
            }
            if (_typeMarket.ToString().Equals("Spot"))
            {
                var spots = allSymbols.Where(x => x.Symbol.Contains("t") && x.Symbol.Contains(":") && !x.Symbol.Contains("F0")).ToList();
                var ms = allSymbols.Where(x=>x.Symbol == "tDUSK:USD").FirstOrDefault();
                var spo = allSymbols.Where(x => x.Symbol.Substring(0, 1).Contains("t") && x.Symbol.Contains(":") && !x.Symbol.Contains("F0")).ToList();
                foreach (var spot in spo)
                {
                    details = await FindSymboldetails(spot.Symbol);
                    instruments.Add(new Assets
                    {
                        Symbol = spot.Symbol,
                        Expiration = details.Expiration,
                        InitialMargin = details.InitialMargin,
                        Margin = details.Margin,
                        MaximumOrderQuantity = details.MaximumOrderQuantity,
                        MinimumMargin = details.MinimumMargin,
                        MinimumOrderQuantity = details.MinimumOrderQuantity,
                        PricePrecision = details.PricePrecision
                    });
                }
                if (instrumentsRequest.Success)
                {
                    return instruments;
                }
            }

            return new List<Assets>();
        }
        private async Task<BitfinexSymbolDetails> FindSymboldetails(string Symbol)
        {
            var asset = await _restClient.SpotApi.ExchangeData.GetSymbolDetailsAsync();
            //foreach (var item in asset.Data)
            //{
            //    if (item.Symbol.Contains("DUSK:BTC".ToLower()))
            //    {

            //    }
            //    if (!item.Symbol.Contains("DUSK:BTC".ToLower()))
            //    {

            //    }
            //}

            var val = asset.Data.ToList().Where(x => x.Symbol == Symbol.Replace("t", "").ToLower()).FirstOrDefault();
            return val;
        }

        public async Task<IEnumerable<BitfinexKline>> GetKlinesAsync(string symbol, string interval, DateTime? startTime = null, DateTime? endTime = null)
        {
            KlineInterval klineInterval = KlineInterval.OneMinute;

            if (interval.Equals(CfgSourceEts.DownLoader1Minutes))
                klineInterval = KlineInterval.OneMinute;

            if (interval.Equals(CfgSourceEts.DownLoader5Minutes))
                klineInterval = KlineInterval.FiveMinutes;

            if (interval.Equals(CfgSourceEts.DownLoader15Minutes))
                klineInterval = KlineInterval.FifteenMinutes;

            if (interval.Equals(CfgSourceEts.DownLoader1Hour))
                klineInterval = KlineInterval.OneHour;

            if (interval.Equals(CfgSourceEts.DownLoader1Month))
                klineInterval = KlineInterval.OneMonth;

            var klineRequest = await _restClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, klineInterval, null, null, startTime, endTime);
            if (klineRequest.Success)
            {
                return klineRequest.Data;
            }

            return new List<BitfinexKline>();
        }
        public async Task PlaceOrderAsync(string symbol, OrderSide orderSide, OrderType orderType, decimal quantity, decimal price, string clientId)
        {
            var placedOrder = await _restClient.SpotApi.Trading.PlaceOrderAsync(symbol, orderSide, orderType, quantity: quantity, price: price, clientOrderId: Convert.ToInt16(clientId));
            if (placedOrder.Success)
                BufferPlaceOrders.Post(placedOrder.Data);

        }

        public async Task CancelOrderAsync(long orderId)
        {
            var cancelledOrder = await _restClient.SpotApi.Trading.CancelOrderAsync(orderId);
            if (cancelledOrder.Success)
                BufferCancelOrders.Post(new CancelOrder() { CancelOrderId = orderId });
        }
    }
}
