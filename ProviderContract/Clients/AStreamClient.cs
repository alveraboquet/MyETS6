using CommonDataContract.ReactData;
using ProviderContract.Data;
using ProviderContract.Data.Entities;
using ProviderContract.Data.Enums;
using ProviderContract.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ProviderContract.Clients
{
    public delegate void WebSocketMessageHandler<T>(T data);
    public delegate void StreamEventHandler(string message);

    /// <summary>
    /// Клиент для получения данных в реальном времени
    /// </summary>
    public abstract class AStreamClient : IStreamClient, IDisposable
    {
        private const int SECONDS_IN_MINUTE = 60;
        protected MarketTypeConfiguration config;
        protected ABaseClient client;
        protected readonly EnumMarket market;
        protected KeysData keysData;

        private readonly Dictionary<string, Dictionary<KlineTimeframe, KlineData>> klines;
        private readonly Dictionary<KlineTimeframe, long> openTimes;

        protected readonly CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Клиент для получения данных в реальном времени
        /// </summary>
        /// <param name="marketName">Название биржи</param>
        /// <param name="keys">API-ключи</param>
        public AStreamClient(EnumMarket marketName, KeysData keys = null)
        {
            market = marketName;
            keysData = keys;

            cts = new CancellationTokenSource();
            klines = new Dictionary<string, Dictionary<KlineTimeframe, KlineData>>();
            openTimes = new Dictionary<KlineTimeframe, long>();
        }

        /// <summary>
        /// Имя рынка/конфигурации
        /// </summary>
        protected string ClassCode => config.MarketName;

        /// <summary>
        /// Проверка авторизации (наличия ключей) - Вызывать в начале приватного метода
        /// </summary>
        protected void CheckAuthorization([CallerMemberName] string method = null)
        {
            if (!client.IsAuthorized)
            {
                throw new ApiAccessException($"{ClassCode} {method}: User is not authorized");
            }
        }
        /// <summary>
        /// Начать получение данных в реальном времени
        /// </summary>
        public abstract void OpenStream();
        /// <summary>
        /// Прекратить получение данных в реальном времени
        /// </summary>
        public virtual void CloseStream() {}

        #region Subscribe Methods
        /// <summary>
        /// Подписка на все приватные данные
        /// </summary>
        /// <exception cref="ApiAccessException"></exception>
        public abstract void SubscribeUserData(IEnumerable<string> pairs);

        /// <summary>
        /// Подписка на аггрегированные публичные сделки
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        public abstract void SubscribeAggregatedTrades(IEnumerable<string> pairs);

        /// <summary>
        /// Подписка на лучшие Ask/Bid стакана
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        public abstract void SubscribeBookTickers(IEnumerable<string> pairs);

        /// <summary>
        /// Добавить подписку на свечи
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        /// <param name="timeframe">Период свечей</param>
        public void SubscribeKlines(IEnumerable<string> pairs, KlineTimeframe timeframe)
        {
            int timeframeInSeconds = (int)timeframe;
            int volumesLength = timeframeInSeconds / SECONDS_IN_MINUTE;

            List<string> inactivePairs = new List<string>();

            foreach (var pair in pairs)
            {
                if (!klines.ContainsKey(pair))
                {
                    klines.Add(pair, new Dictionary<KlineTimeframe, KlineData>());
                    // Если пары нет в словаре, то будем подписываться
                    inactivePairs.Add(pair);
                }
                if (!klines[pair].ContainsKey(timeframe))
                {
                    KlineData klineData = new KlineData()
                    {
                        Kline = null,
                        Volumes = new double[volumesLength]
                    };
                    klines[pair].Add(timeframe, klineData);
                }
            }

            // Вычисляем время старта первой свечи
            DateTime now = DateTime.UtcNow;
            DateTime firstDayOfCurrentYear = new DateTime(now.Year, 1, 1);
            DateTime unix = new DateTime(1970, 1, 1);

            //Начало текущего года 01.01 00:00
            long startTimestamp = (long)(firstDayOfCurrentYear - unix).TotalSeconds;
            //Текущее дата/время
            long nowTimeStamp = (long)(now - unix).TotalSeconds;
            //Количество таймфремов для пропуска
            int skippedTimeframes = (int)(nowTimeStamp - startTimestamp) / timeframeInSeconds;
            //Определяем актуальный таймфрейм
            long openTime = startTimestamp + timeframeInSeconds * skippedTimeframes;
            if (!openTimes.ContainsKey(timeframe))
            {
                openTimes.Add(timeframe, openTime);
            }
            /*
             * Допустим таймфрейм 4 часа, а сейчас 17:23 UTC
             * Тогда мы должны получить таймфрейм 16:00
             * Для таймфрейма в 5 минут - 17:20 и т.д.
             */

            if (inactivePairs.Count > 0)
            {
                // Осуществляем фактическую подписку на 1-минутные свечи
                // Только для тех пар, которых ещё нет
                SubscribeKlines(inactivePairs);
            }
        }

        /// <summary>
        /// Реализация подписки на 1-минутные свечи в коннекторе
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        protected abstract void SubscribeKlines(IEnumerable<string> pairs);

        /// <summary>
        /// Добавить подписку на обновление стакана
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        public abstract void SubscribeOrderBooks(IEnumerable<string> pairs);
        /// <summary>
        /// Добавить подписку на публичные сделки (тики)
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        public abstract void SubscribeTicks(IEnumerable<string> pairs);
        /// <summary>
        /// Добавить подписку на тикеры
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        public abstract void SubscribeTickers(IEnumerable<string> pairs);
        #endregion Subscribe Methods

        #region Unsubscribe Methods
        /// <summary>
        /// Отписка от приватных данных 
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        public abstract void UnsubscribeUserData(IEnumerable<string> pairs);
        /// <summary>
        /// Отписка от аггрегированных публичных сделок
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        public abstract void UnsubscribeAggregatedTrades(IEnumerable<string> pairs);
        /// <summary>
        /// Отписка от лучших цен Ask/Bid стакан
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        public abstract void UnsubscribeBookTickers(IEnumerable<string> pairs);
        /// <summary>
        /// Отписка от свечей
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        /// <param name="timeframe">Период свечей</param>
        public abstract void UnsubscribeKlines(IEnumerable<string> pairs);
        
        /// <summary>
        /// Отписка от стаканов
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        public abstract void UnsubscribeOrderBooks(IEnumerable<string> pairs);
        /// <summary>
        /// Отписка от публичных сделок (тиков)
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        public abstract void UnsubscribeTicks(IEnumerable<string> pairs);
        /// <summary>
        /// Отписка от тикеров
        /// </summary>
        /// <param name="pairs">Список инструментов</param>
        public abstract void UnsubscribeTickers(IEnumerable<string> pairs);
        #endregion Unsubscribe Methods

        #region Socket Events
        public event StreamEventHandler SocketConnected;
        public event StreamEventHandler SocketDisconnected;
        public event StreamEventHandler AuthError;
        public event StreamEventHandler StreamDisconnected;
        #endregion Socket Events

        #region DataReceived Events
        /// <summary>
        /// Событие изменения баланса аккаунта
        /// </summary>
        public event WebSocketMessageHandler<List<SecPosition>> BalanceMessageReceived;
        /// <summary>
        /// Событие получения данных по свечам
        /// </summary>
        public event WebSocketMessageHandler<Kline> KlineMessageReceived;
        /// <summary>
        /// Событие изменения заявок аккаунта
        /// </summary>
        public event WebSocketMessageHandler<Orders> OrderMessageReceived;
        /// <summary>
        /// Событие получения данных по стакану
        /// </summary>
        public event WebSocketMessageHandler<OrderBook> OrderBookMessageReceived;
        /// <summary>
        /// Событие получения данных о публичных сделках
        /// </summary>
        public event WebSocketMessageHandler<Tick> TickMessageReceived;
        /// <summary>
        /// Событие получения данных об аггрегированных сделках
        /// </summary>
        public event WebSocketMessageHandler<Tick> AggTradeMessageReceived;

        public event WebSocketMessageHandler<BookTicker> BookTickerMessageReceived;
        /// <summary>
        /// Событие получения данных по тикерам
        /// </summary>
        public event WebSocketMessageHandler<Securities> TickerMessageReceived;
        /// <summary>
        /// Событие изменения сделок аккаунта
        /// </summary>
        public event WebSocketMessageHandler<Deal> TradeMessageReceived;
        #endregion DataReceived Events

        protected void OnSocketConnected(string message)
        {
            SocketConnected?.Invoke(message);
        }
        protected void OnSocketDisconnected(string message)
        {
            SocketDisconnected?.Invoke(message);
        }
        protected void OnAuthError(string message)
        {
            AuthError?.Invoke(message);
        }
        protected void OnStreamDisconnected(string message)
        {
            StreamDisconnected?.Invoke(message);
        }

        /// <summary>
        /// Безопасный вызов события AggTradeMessageReceived
        /// </summary>
        /// <param name="aggTrade"></param>
        protected void OnAggTradeMessageReceived(Tick aggTrade)
        {
            AggTradeMessageReceived?.Invoke(aggTrade);
        }
        /// <summary>
        /// Безопасный вызов события BookTickerMessageReceived
        /// </summary>
        /// <param name="bookTicker"></param>
        protected void OnBookTickerMessageReceived(BookTicker bookTicker)
        {
            BookTickerMessageReceived?.Invoke(bookTicker);
        }
        /// <summary>
        /// Безопасный вызов события BalanceMessageReceived
        /// </summary>
        /// <param name="balances">Список балансов</param>
        protected void OnBalanceMessageReceived(List<SecPosition> balances)
        {
            BalanceMessageReceived?.Invoke(balances);
        }
        /// <summary>
        /// Безопасный вызов события KlineMessageReceived
        /// </summary>
        /// <param name="kline">Свеча</param>
        protected void OnKlineMessageReceived(Kline kline)
        {
            UpdateKline(kline);
        }

        /// <summary>
        /// Безопасный вызов события OrderMessageReceived
        /// </summary>
        /// <param name="order">Заявка</param>
        protected void OnOrderMessageReceived(Orders order)
        {
            OrderMessageReceived?.Invoke(order);
        }

        /// <summary>
        /// Безопасный вызов события OrderBookMessageReceived
        /// </summary>
        /// <param name="orderBook">Книга ордеров</param>
        protected void OnOrderBookMessageReceived(OrderBook orderBook)
        {
            OrderBookMessageReceived?.Invoke(orderBook);
        }

        /// <summary>
        /// Безопасный вызов события TickMessageReceived
        /// </summary>
        /// <param name="publicTrade">Публичная сделка</param>
        protected void OnTickMessageReceived(Tick publicTrade)
        {
            TickMessageReceived?.Invoke(publicTrade);
        }
        /// <summary>
        /// Безопасный вызов события TickerMessageReceived
        /// </summary>
        /// <param name="ticker">Тикер</param>
        protected void OnTickerMessageReceived(Securities ticker)
        {
            TickerMessageReceived?.Invoke(ticker);
        }
        /// <summary>
        /// Безопасный вызов события TradeMessageReceived
        /// </summary>
        /// <param name="deal">Сделка пользователя</param>
        protected void OnTradeMessageReceived(Deal deal)
        {
            TradeMessageReceived?.Invoke(deal);
        }

        public void Dispose()
        {
            CloseSocketsAndDispose();
            GC.SuppressFinalize(this);
        }

        ~AStreamClient()
        {
            CloseSocketsAndDispose();
        }
        /// <summary>
        /// Закрыть сокеты и освободить ресурсы
        /// </summary>
        protected abstract void CloseSocketsAndDispose();

        /// <summary>
        /// Конвертация свечи в нужный таймфрейм
        /// </summary>
        /// <param name="kline">Базовая свеча 1-мин</param>
        private void UpdateKline(Kline kline)
        {
            // Получили свечу 1м и для каждого таймфрейма на
            // который есть подписка у инструмента делаем обновление
            foreach (KlineTimeframe tf in klines[kline.Pair].Keys)
            {
                // Текущие данные по паре и таймфрейму
                KlineData storedData = klines[kline.Pair][tf];
                int timeframeInSeconds = (int)tf;

                // Текущая минута в рамках таймфрейма
                int timeDiff = (int)(kline.OpenTime - openTimes[tf]);

                int volumeIndex = ((int)(kline.OpenTime - openTimes[tf])) / SECONDS_IN_MINUTE;

                // Если свеча пришла впервые
                if (storedData.Kline == null)
                {
                    kline.OpenTime = openTimes[tf];
                    klines[kline.Pair][tf].Kline = kline;
                    klines[kline.Pair][tf].Volumes[volumeIndex] = kline.Volume;
                }
                else
                {
                    // Текущая свеча
                    if (timeDiff < timeframeInSeconds)
                    {
                        Kline storedKline = storedData.Kline;
                        volumeIndex = timeDiff / SECONDS_IN_MINUTE;

                        storedKline.Close = kline.Close;
                        if (kline.High > storedKline.High)
                        {
                            storedKline.High = kline.High;
                        }
                        if (kline.Low < storedKline.Low)
                        {
                            storedKline.Low = kline.Low;
                        }
                        klines[kline.Pair][tf].Volumes[volumeIndex] = kline.Volume;
                        storedKline.Volume = klines[kline.Pair][tf].Volumes.Sum();
                    }
                    // Новый период
                    else
                    {
                        openTimes[tf] += timeframeInSeconds;
                        volumeIndex = 0;

                        klines[kline.Pair][tf].Kline = kline;
                        klines[kline.Pair][tf].Kline.OpenTime = openTimes[tf];

                        int volumesCount = timeframeInSeconds / SECONDS_IN_MINUTE;

                        klines[kline.Pair][tf].Volumes = new double[volumesCount];
                        klines[kline.Pair][tf].Volumes[volumeIndex] = kline.Volume;
                    }
                }

                KlineMessageReceived?.Invoke(klines[kline.Pair][tf].Kline);
            }
        }
    }
}
