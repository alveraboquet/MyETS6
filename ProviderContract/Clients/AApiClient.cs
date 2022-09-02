using ProviderContract.Data;
using ProviderContract.Data.Enums;
using ProviderContract.Data.Entities;
using ProviderContract.Exceptions;
using ProviderContract.Services;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using CommonDataContract.ReactData;
using System.Linq;

namespace ProviderContract.Clients
{
    /// <summary>
    /// Клиент для выполнения разовых запросов
    /// </summary>
    public abstract class AApiClient : IAPIClient
    {
        protected readonly EnumMarket market;
        protected readonly KeysData keys;
        protected MarketTypeConfiguration config;
        protected ABaseClient client;

        /// <summary>
        /// Клиент для выполнения разовых запросов
        /// </summary>
        /// <param name="pairsList">Список инструментов</param>
        /// <param name="marketName">Имя биржи</param>
        /// <param name="keysData">API-ключи</param>
        public AApiClient(EnumMarket marketName, KeysData keysData = null)
        {
            market = marketName;
            keys = keysData;
        }

        /// <summary>
        /// Имя рынка/конфигурации
        /// </summary>
        protected string ClassCode => config.MarketName;

        /// <summary>
        /// Проверка авторизации (наличия ключей) - Вызывать в начале приватного метода.
        /// Если ключи не переданы выбрасывает исключение 
        /// </summary>
        /// <exception cref="ApiAccessException"></exception>
        protected void CheckAuthorization([CallerMemberName] string method = null)
        {
            if (!client.IsAuthorized)
            {
                throw new ApiAccessException($"{ClassCode} {method}: User is not authorized");
            }
        }

        /// <summary>
        /// Выброс исключения при ошибке запроса
        /// </summary>
        /// <typeparam name="T">Определяется автоматически</typeparam>
        /// <param name="response">Ответ сервера</param>
        /// <param name="method">Определяется автоматически</param>
        /// <exception cref="ApiException"></exception>
        protected void ThrowResponseError<T>(Response<T> response, [CallerMemberName]string method = null)
        {
            throw new ApiException($"{ClassCode} {method} Error: {response.Error}; {response.RawJson}");
        }

        /// <summary>
        /// Список монет, задействованных пользователем
        /// </summary>
        /// <returns>Список, содержащий все монеты - { COIN1, COIN2, ... }</returns>
        protected List<string> GetCoins(IEnumerable<string> pairs)
        {
            var coins = new List<string>();

            foreach (var pair in pairs)
            {
                var coin1 = pair.Split('-')[0].ToUpper();
                var coin2 = pair.Split('-')[1].ToUpper();
                if (!coins.Contains(coin1))
                    coins.Add(coin1);
                if (!coins.Contains(coin2))
                    coins.Add(coin2);
            }

            return coins;
        }

        /// <summary>
        /// Отмена заявки
        /// </summary>
        /// <param name="id">Id ордера</param>
        /// <param name="pair">Название торгового инструмента</param>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        /// <exception cref="ApiAccessException">При вызове без Api-ключей</exception>
        public abstract void CancelAnOrder(string id, string pair);

        /// <summary>
        /// Выставление заявки
        /// </summary>
        /// <param name="pair">Торговый инструмент</param>
        /// <param name="side">Покупка или продажа</param>
        /// <param name="quantity">Количество</param>
        /// <param name="type">Тип заявки</param>
        /// <param name="timeInForce">Стратегия отмены заявки</param>
        /// <param name="price">Цена</param>
        /// <param name="icebergQty">Для создания айсберг-заявки</param>
        /// <param name="uid">Внутренний уникальный идентификатор заявки</param>
        /// <returns>Выставленная заявка</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        /// <exception cref="ApiAccessException">При вызове без Api-ключей</exception>
        public abstract Orders PlaceOrder(string pair, OrderSide side, double quantity, OrderType type = OrderType.LIMIT, TimeInForce timeInForce = TimeInForce.GTC, double price = 0, double icebergQty = 0, string uid = null);

        protected abstract List<Tick> GetAggregatedTrades (string pair, int limit);

        /// <summary>
        /// Запрос списка публичных сделок по всем инструментам клиента
        /// </summary>
        /// <param name="limit">Ограничение количества сделок по каждому инструменту</param>
        /// <returns>Список публичных сделок</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        public virtual List<Tick> GetAggregatedTrades(IEnumerable<string> pairs, int limit = 100)
        {
            List<Tick> aggTradesResult = new List<Tick>();

            foreach (var pair in pairs)
            {
                List<Tick> publicTrades = GetAggregatedTrades(pair, limit);
                aggTradesResult.AddRange(publicTrades);
            }

            return aggTradesResult;
        }

        /// <summary>
        /// Запрос баланса по монете
        /// </summary>
        /// <param name="coin">Монета</param>
        /// <returns>Баланс аккаунта по монете</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        /// <exception cref="ApiAccessException">При вызове без Api-ключей</exception>
        protected abstract SecPosition GetBalance(string coin);

        /// <summary>
        /// Запрос всех балансов аккаунта
        /// </summary>
        /// <returns>Список балансов аккаунта</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        /// <exception cref="ApiAccessException">При вызове без Api-ключей</exception>
        public virtual List<SecPosition> GetBalance(IEnumerable<string> pairs)
        {
            List<SecPosition> balances = new List<SecPosition>();
            List<string> coins = GetCoins(pairs);

            foreach (var coin in coins)
            {
                SecPosition balance = GetBalance(coin);
                balances.Add(balance);
            }

            return balances;
        }

        /// <summary>
        /// Возвращает свечи в базовых таймфреймах (1 мин, 1 час, 1 день)
        /// </summary>
        /// <param name="pair">Торговый инструмент</param>
        /// <param name="timeframe">Таймфрейм</param>
        /// <param name="from">Начало периода</param>
        /// <param name="to">Конец периода</param>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        protected abstract List<Kline> GetKlines(string pair, KlineTimeframe timeframe, DateTime from, DateTime to);

        /// <summary>
        /// Запрос свечей за период по всем инструментам клиента
        /// </summary>
        /// <param name="timeframe">Таймфрейм</param>
        /// <param name="from">Начало периода</param>
        /// <param name="to">Конец периода</param>
        /// <returns>Список свечей</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        public virtual List<Kline> GetKlines(IEnumerable<string> pairs, KlineTimeframe timeframe, DateTime from, DateTime to)
        {
            List<Kline> klinesResult = new List<Kline>();

            KlineTimeframe baseTimeframe = KlineTimeframe._1MIN;
            int multiplier = 1;

            switch (timeframe)
            {
                case KlineTimeframe._1MIN:
                    break;
                case KlineTimeframe._5MIN:
                    multiplier = 5;
                    break;
                case KlineTimeframe._10MIN:
                    multiplier = 10;
                    break;
                case KlineTimeframe._15MIN:
                    multiplier = 15;
                    break;
                case KlineTimeframe._30MIN:
                    multiplier = 30;
                    break;
                case KlineTimeframe._1HOUR:
                    baseTimeframe = KlineTimeframe._1HOUR;
                    break;
                case KlineTimeframe._4HOUR:
                    baseTimeframe = KlineTimeframe._1HOUR;
                    multiplier = 4;
                    break;
                case KlineTimeframe._12HOUR:
                    baseTimeframe = KlineTimeframe._1HOUR;
                    multiplier = 12;
                    break;
                case KlineTimeframe._1DAY:
                    baseTimeframe = KlineTimeframe._1DAY;
                    break;
                case KlineTimeframe._3DAY:
                    baseTimeframe = KlineTimeframe._1DAY;
                    multiplier = 3;
                    break;
                case KlineTimeframe._1WEEK:
                    baseTimeframe = KlineTimeframe._1DAY;
                    multiplier = 7;
                    break;
                default:
                    break;
            }

            foreach (var pair in pairs)
            {
                List<Kline> klines = GetKlines(pair, baseTimeframe, from, to);
                klines = KlinesService.GetMultipliedTimeframe(klines, multiplier);
                klinesResult.AddRange(klines);
            }

            return klinesResult;
        }

        /// <summary>
        /// Запрос текущего состояния стакана по указанному инструменту
        /// </summary>
        /// <param name="pair">Торговый инструмент</param>
        /// <returns>Объект книги ордеров со списками асков и бидов</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        protected abstract OrderBook GetOrderBooks(string pair, int depth = 50);

        /// <summary>
        /// Запрос стаканов по заданным инструментам
        /// </summary>
        /// <param name="pairs">Список торговых инструментов</param>
        /// <returns>Список стаканов</returns>
        public virtual List<OrderBook> GetOrderBooks(IEnumerable<string> pairs, int depth = 50)
        {
            List<OrderBook> orderBooks = new List<OrderBook>();

            foreach (var pair in pairs)
            {
                OrderBook orderBook = GetOrderBooks(pair, depth);
                orderBooks.Add(orderBook);
            }

            return orderBooks;
        }

        /// <summary>
        /// Запрос заявок аккаунта по инструменту
        /// </summary>
        /// <param name="pair">Торговый инструмент</param>
        /// <returns>Список ордеров по торговому инструменту</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        /// <exception cref="ApiAccessException">При вызове без Api-ключей</exception>
        protected abstract List<Orders> GetOrders(string pair);

        /// <summary>
        /// Запрос всех заявок аккаунта
        /// </summary>
        /// <returns>Список заявок</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        /// <exception cref="ApiAccessException">При вызове без Api-ключей</exception>
        public virtual List<Orders> GetOrders(IEnumerable<string> pairs)
        {
            List<Orders> ordersResult = new List<Orders>();

            foreach (var pair in pairs)
            {
                List<Orders> orders = GetOrders(pair);
                ordersResult.AddRange(orders);
            }

            return ordersResult;
        }

        /// <summary>
        /// Запрос списка активных торговых инструментов биржи
        /// </summary>
        /// <returns>Список торговых пар</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        public abstract List<Securities> GetPairs(IEnumerable<string> pairs = null);

        public abstract long GetServerTime();

        /// <summary>
        /// Запрос списка публичных сделок по инструменту
        /// </summary>
        /// <param name="pair">Торговый инструмент</param>
        /// <param name="limit">Ограничение количества сделок в ответе</param>
        /// <returns>Список публичных сделок</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        protected abstract List<Tick> GetTicks(string pair, int limit);

        /// <summary>
        /// Запрос списка публичных сделок по всем инструментам клиента
        /// </summary>
        /// <param name="limit">Ограничение количества сделок по каждому инструменту</param>
        /// <returns>Список публичных сделок</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        public virtual List<Tick> GetTicks(IEnumerable<string> pairs, int limit = 100)
        {
            List<Tick> publicTradesResult = new List<Tick>();

            foreach (var pair in pairs)
            {
                List<Tick> publicTrades = GetTicks(pair, limit);
                publicTradesResult.AddRange(publicTrades);
            }

            return publicTradesResult;
        }

        /// <summary>
        /// Запрос тикера по инструменту
        /// </summary>
        /// <param name="pair">Торговый инструмент</param>
        /// <returns>Тикер по инструменту</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        protected abstract Securities GetTickers(string pair);

        public virtual List<Securities> GetTickers(IEnumerable<string> pairs)
        {
            List<Securities> tickersResult = new List<Securities>();

            foreach (var pair in pairs)
            {
                Securities ticker = GetTickers(pair);
                tickersResult.Add(ticker);
            }

            return tickersResult;
        }

        /// <summary>
        /// Запрос сделок аккаунта по инструменту
        /// </summary>
        /// <param name="pair">Торговый инструмент</param>
        /// <returns>Список сделок аккаунта по инструменту</returns>
        /// <exception cref="ApiException">При ошибке запроса</exception>
        /// <exception cref="ApiAccessException">При вызове без Api-ключей</exception>
        protected abstract List<Deal> GetTrades(string pair);

        public virtual List<Deal> GetTrades(IEnumerable<string> pairs)
        {
            List<Deal> tradesResult = new List<Deal>();

            foreach (var pair in pairs)
            {
                List<Deal> trades = GetTrades(pair);
                tradesResult.AddRange(trades);
            }

            return tradesResult;
        }

        /// <summary>
        /// Задержка выполнения запроса. Для лимита 30 запросов за 3 секунды, будет ждать 1/10 секунды
        /// </summary>
        /// <param name="queries">Количество запросов</param>
        /// <param name="seconds">Количество секунд</param>
        protected static void RateLimit(int queries, int seconds)
        {
            int delayMilliSeconds = Convert.ToInt32(((seconds / queries) * 1000));
            Task.Delay(delayMilliSeconds).Wait();
        }
    }
}
