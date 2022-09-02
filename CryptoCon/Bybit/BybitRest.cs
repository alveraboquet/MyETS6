using Adapter.Config;
using Binance.Net.Objects.Models.Spot;
using Bybit.Net.Clients;
using Bybit.Net.Enums;
using Bybit.Net.Interfaces.Clients;
using Bybit.Net.Objects;
using Bybit.Net.Objects.Models;
using Bybit.Net.Objects.Models.Spot;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CryptoCon.Bybit
{
    public class BybitRest
    {
        private BybitClient _bybitClient;

        public BufferBlock<BybitSpotOrderPlaced> BufferPlaceSpotOrders { get; set; }
        public BufferBlock<BybitUsdPerpetualOrder> BufferPlaceUsdFuturesOrders { get; set; }


        public BufferBlock<BybitSpotOrderPlaced> BufferCancelSpotOrders { get; set; }
        public BufferBlock<BybitOrderId> BufferCancelUsdFuturesOrders { get; set; }


        public BufferBlock<BybitSpotOrderBook> BufferSpotOrderBook { get; set; }
        public BufferBlock<IEnumerable<BybitOrderBookEntry>> BufferUsdFuturesOrderBook { get; set; }

        public BybitRest()
        {
            _bybitClient = new BybitClient();
            Init();
        }

        public BybitRest(BybitClientOptions options)
        {
            _bybitClient = new BybitClient(options);
            Init();
        }

        private void Init()
        {
            BufferPlaceSpotOrders = new BufferBlock<BybitSpotOrderPlaced>();
            BufferPlaceUsdFuturesOrders = new BufferBlock<BybitUsdPerpetualOrder>();
            BufferCancelSpotOrders = new BufferBlock<BybitSpotOrderPlaced>();
            BufferCancelUsdFuturesOrders = new BufferBlock<BybitOrderId>();
            BufferSpotOrderBook = new BufferBlock<BybitSpotOrderBook>();
            BufferUsdFuturesOrderBook = new BufferBlock<IEnumerable<BybitOrderBookEntry>>();
        }

        #region Spot

        public async Task<IEnumerable<BybitSpotBalance>> GetBalancesAccountAsync()
        {
            var balances = await _bybitClient.SpotApi.Account.GetBalancesAsync();

            if (balances.Success)
                return balances.Data;
            else
                return new List<BybitSpotBalance>();
        }

        public async Task<IEnumerable<BybitSpotOrder>> GetOrdersAsync(string symbol = default)
        {
            var orders = await _bybitClient.SpotApi.Trading.GetOrdersAsync(symbol);

            if (orders.Success)
                return orders.Data;
            else
                return new List<BybitSpotOrder>();
        }

        public async Task<IEnumerable<BybitSpotSymbol>> GetTickersAsync()
        {
            var result = await _bybitClient.SpotApi.ExchangeData.GetSymbolsAsync();

            if (result.Success)
                return result.Data;
            
            return new List<BybitSpotSymbol>();
        }

        public async Task GetOrderBookAsync(string symbol, int? limit = default, CancellationToken ct = default)
        {
            var result = await _bybitClient.SpotApi.ExchangeData.GetOrderBookAsync(symbol, limit, ct);
            BufferSpotOrderBook.Post(result.Data);
        }

        /// <summary>
        /// Установка ордера
        /// </summary>
        /// <param name="symbol">инструмент</param>
        /// <param name="side">направление сделки</param>
        /// <param name="type">тип ордера</param>
        /// <param name="quantity">объём</param>
        /// <param name="price">цена</param>
        /// <param name="timeInForce">время жизни ордера</param>
        /// <param name="clientOrderId">id ордера клиента</param>
        /// <param name="receiveWindow">время, в течении которого активен этот запрос, если для выполнения запроса требуется больше времени, чем указано, сервер отклонит запрос</param>
        /// <returns></returns>
        public async Task<WebCallResult<BybitSpotOrderPlaced>> PlaceOrderAsync(string symbol, OrderSide side, OrderType type, decimal quantity, decimal? price = null, 
            TimeInForce? timeInForce = null, string clientOrderId = null, long? receiveWindow = null)
        {
            return await _bybitClient.SpotApi.Trading.PlaceOrderAsync(symbol, side, type, quantity, price, timeInForce, clientOrderId, receiveWindow);
        }

        /// <summary>
        /// Снятие ордера
        /// </summary>
        /// <param name="orderId">id ордера</param>
        /// <param name="clientOrderId">id ордера клиента</param>
        /// <param name="receiveWindow">время, в течении которого активен этот запрос, если для выполнения запроса требуется больше времени, чем указано, сервер отклонит запрос</param>
        /// <returns></returns>
        public async Task<WebCallResult<BybitSpotOrderPlaced>> CancelOrderAsync(long? orderId = null, string clientOrderId = null, long? receiveWindow = null)
        {
            return await _bybitClient.SpotApi.Trading.CancelOrderAsync(orderId, clientOrderId, receiveWindow);
        }

        public async Task<IEnumerable<BybitSpotKline>> GetKlinesAsync(string symbol, KlineInterval interval, DateTime? startTime = default, DateTime? endTime = default, 
            int? limit = default, CancellationToken ct = default)
        {
            var result = await _bybitClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, interval, startTime, endTime, limit, ct);

            if (result.Success)
                return result.Data;

            return new List<BybitSpotKline>();
        }

        public async Task<WebCallResult<IEnumerable<BybitSpotOrder>>> GetOpenOrdersAsync(string? symbol = default, long? orderId = default, int? limit = default, long? receiveWindow = default, CancellationToken ct = default)
        {
            return await _bybitClient.SpotApi.Trading.GetOpenOrdersAsync(symbol, orderId, limit, receiveWindow, ct);
        }

        #endregion



        #region Futures

        public async Task<Dictionary<string, BybitBalance>> GetBalancesUsdFuturesAccountAsync(long? receiveWindow = default)
        {
            var balances = await _bybitClient.UsdPerpetualApi.Account.GetBalancesAsync(receiveWindow: receiveWindow);

            if (balances.Success)
                return balances.Data;
            else
                return new Dictionary<string, BybitBalance>();
        }

        public async Task<IEnumerable<BybitUsdPerpetualOrder>> GetUsdFuturesOrdersAsync(string symbol)
        {
            var orders = await _bybitClient.UsdPerpetualApi.Trading.GetOrdersAsync(symbol);

            if (orders.Success)
                return orders.Data.Data;
            else
                return new List<BybitUsdPerpetualOrder>();
        }

        public async Task<IEnumerable<BybitSymbol>> GetUsdFuturesTickersAsync()
        {
            int counter = 0;

            link1:
            var result = await _bybitClient.UsdPerpetualApi.ExchangeData.GetSymbolsAsync();

            if (result.Success)
                return result.Data;
            else if (counter <= 2)
            {
                counter++;
                goto link1;
            }

            return new List<BybitSymbol>();
        }

        public async Task GetUsdFuturesOrderBookAsync(string symbol, CancellationToken ct = default)
        {
            var result = await _bybitClient.UsdPerpetualApi.ExchangeData.GetOrderBookAsync(symbol, ct);
            BufferUsdFuturesOrderBook.Post(result.Data);
        }

        /// <summary>
        /// Установка ордера
        /// </summary>
        /// <param name="symbol">инструмент</param>
        /// <param name="side">направление сделки</param>
        /// <param name="type">тип ордера</param>
        /// <param name="quantity">объём</param>
        /// <param name="price">цена</param>
        /// <param name="timeInForce">время жизни ордера</param>
        /// <param name="clientOrderId">id ордера клиента</param>
        /// <param name="receiveWindow">время, в течении которого активен этот запрос, если для выполнения запроса требуется больше времени, чем указано, сервер отклонит запрос</param>
        /// <returns></returns>
        public async Task<WebCallResult<BybitUsdPerpetualOrder>> PlaceUsdFuturesOrderAsync(string symbol, OrderSide side, OrderType type, decimal quantity, TimeInForce timeInForce, 
            decimal? price = null, string clientOrderId = null, long? receiveWindow = null)
        {
            return await _bybitClient.UsdPerpetualApi.Trading.PlaceOrderAsync(symbol, side, type, quantity, timeInForce, false, false, price: price, clientOrderId: clientOrderId, receiveWindow: receiveWindow);
        }

        /// <summary>
        /// Снятие ордера
        /// </summary>
        /// <param name="orderId">id ордера</param>
        /// <param name="clientOrderId">id ордера клиента</param>
        /// <param name="receiveWindow">время, в течении которого активен этот запрос, если для выполнения запроса требуется больше времени, чем указано, сервер отклонит запрос</param>
        /// <returns></returns>
        public async Task<WebCallResult<BybitOrderId>> CancelUsdFuturesOrderAsync(string symbol, string orderId = null, string clientOrderId = null, long? receiveWindow = null)
        {
            return await _bybitClient.UsdPerpetualApi.Trading.CancelOrderAsync(symbol, orderId, clientOrderId, receiveWindow);
        }

        public async Task<IEnumerable<BybitKline>> GetUsdFuturesKlinesAsync(string symbol, KlineInterval interval, DateTime from, int? limit = default, CancellationToken ct = default)
        {
            var result = await _bybitClient.UsdPerpetualApi.ExchangeData.GetKlinesAsync(symbol, interval, from, limit, ct);

            if (result.Success)
                return result.Data;

            return new List<BybitKline>();
        }

        public async Task<WebCallResult<IEnumerable<BybitUsdPerpetualOrder>>> GetOpenOrdersRealTimeAsync(string symbol, long? receiveWindow = default, CancellationToken ct = default)
        {
            return await _bybitClient.UsdPerpetualApi.Trading.GetOpenOrdersRealTimeAsync(symbol, receiveWindow, ct);
        }

        #endregion
    }
}
