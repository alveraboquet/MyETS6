using CommonDataContract;
using CommonDataContract.AbstractDataTypes;
using System;
using System.Collections.Generic;

namespace SourceEts.Table.TableClass
{
    /// <summary>
    /// Учет загрузки данных по тикам и истории
    /// Необходимо чтоб данные прогрузились перед тем как робот начнет торговать
    /// </summary>
    public class LoadHisrotyTick
    {
        /// <summary>
        /// Код инструмента
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ISecurity Security { get; set; }

        /// <summary>
        /// Исторические данные загружены
        /// </summary>
        public bool IsHistoryLoad { get; set; }
        /// <summary>
        /// Данные по тикам загружены
        /// </summary>
        public bool IsTickLoad { get; set; }
        /// <summary>
        /// Номер последнего тика полученного с биржи
        /// </summary>
        public long LastNumberTickGet { get; set; }

        /// <summary>
        /// Время в формате Unix для первого запроса по тикам
        /// </summary>
        public long FirstDataTimeGetTicks { get; set; }

        /// <summary>
        /// Id последнего ордера по которому нужно запрашивать информацию
        /// </summary>
        public long LastIdOrders { get; set; }

        /// <summary>
        /// Id последнего ордера по которому нужно запрашивать информацию
        /// </summary>
        public long LastIdDeal { get; set; }

        /// <summary>
        /// Время последней сделки, в тех случаях когда Id не цифровой. Например Kucoin
        /// </summary>
        public DateTime LastIdDealDateTime { get; set; }

        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public DateTime LastTimeUpdateIndoDealsAndOrders { get; set; }



        #region Для учета и своевременного обновления данных


        /// <summary>
        /// Ордер был отрпавлен необходимо проверить
        /// </summary>
        public bool IsOrderTransaction { get; set; }

        /// <summary>
        /// Последнее время проверки при активных ордерах
        /// </summary>
        public DateTime LastTimeCheckActiveOrders { get; set; }

        private List<IOrders> _activeOrders;

        /// <summary>
        /// Таблица позиций по фьючерсам и опционам
        /// </summary>
        public List<IOrders> ActiveOrders
        {
            get { return _activeOrders ?? (_activeOrders = new List<IOrders>()); }
            set
            {
                if (_activeOrders != value)
                {
                    _activeOrders = value;
                }
            }
        }

        /// <summary>
        /// Последняя информация о балансе на счете по базовому активу
        /// </summary>
        public double LastBalanceBaseActive { get; set; }

        /// <summary>
        /// Последняя информация о балансе на счете
        /// </summary>
        public double LastBalance { get; set; }

        /// <summary>
        /// Позиция по активу
        /// </summary>
        public IPositionShares Position { get; set; }

        /// <summary>
        /// Позиция по базовому активу
        /// </summary>
        public IPositionShares PositionBaseActive { get; set; }

        /// <summary>
        /// Позиция по активу
        /// </summary>
        public IPos PositionAll { get; set; }

        /// <summary>
        /// Позиция по базовому активу
        /// </summary>
        public IPos PositionBaseActiveAll { get; set; }
        #endregion

        /// <summary>
        /// Сумма которая прошла по сделкам
        /// </summary>
        public double SummDeals { get; set; }

        /// <summary>
        /// Нужно загружать тики, один из роботов, начал работать
        /// </summary>
        public bool IsRequestTicks { get; set; }



        /// <summary>
        /// Был ли запрос на соответствующий таймфрейм и была ли по нему загруза
        /// </summary>
        public bool Is1MinutesLoad { get; set; }
        public bool Is5MinutesLoad { get; set; }
        public bool Is10MinutesLoad { get; set; }
        public bool Is15MinutesLoad { get; set; }
        public bool Is30MinutesLoad { get; set; }
        public bool Is60MinutesLoad { get; set; }
        public bool Is1DayLoad { get; set; }
        public bool Is1WeekLoad { get; set; }
        public bool Is1MonthLoad { get; set; }

        /// <summary>
        /// Загрузка однаминуток текущего дня, т.к. все остальные фреймы будут загружаться до текущего дня. Т.к. Тики загрузжаются за пять минут до налачала запроса
        /// </summary> 
        public bool Is1MinutesRequest { get; set; }
        public bool Is5MinutesRequest { get; set; }
        public bool Is10MinutesRequest { get; set; }
        public bool Is15MinutesRequest { get; set; }
        public bool Is30MinutesRequest { get; set; }
        public bool Is60MinutesRequest { get; set; }
        public bool Is1DayRequest { get; set; }
        public bool Is1WeekRequest { get; set; }
        public bool Is1MonthRequest { get; set; }

        /// <summary>
        /// Проверяем, загружался ли данный таймфрейм
        /// </summary>
        public bool CheckTimeFrameLoad(int timeFrame, string typeTimeFrame)
        {
            if (typeTimeFrame == CfgSourceEts.TypeTimeFrameNotUse)
                return true;

            if (!Is1MinutesRequest)
            {
                Is1MinutesRequest = true;
            }

            if (Is1MinutesLoad)
            {
                if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                    return Is1MinutesLoad;


                if (timeFrame == 5 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                {
                    if (!Is5MinutesRequest)
                        Is5MinutesRequest = true;

                    return Is5MinutesLoad;
                }

                if (timeFrame == 10 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                {
                    if (!Is10MinutesRequest)
                        Is10MinutesRequest = true;

                    return Is10MinutesLoad;
                }

                if (timeFrame == 15 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                {
                    if (!Is15MinutesRequest)
                        Is15MinutesRequest = true;

                    return Is15MinutesLoad;
                }

                if (timeFrame == 30 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                {
                    if (!Is30MinutesRequest)
                        Is30MinutesRequest = true;

                    return Is30MinutesLoad;
                }

                if (timeFrame == 60 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                {
                    if (!Is60MinutesRequest)
                        Is60MinutesRequest = true;

                    return Is60MinutesLoad;
                }

                if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameDays)
                {
                    if (!Is1DayRequest)
                        Is1DayLoad = true;

                    return Is1DayLoad;
                }

                if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameWeek)
                {
                    if (!Is1WeekRequest)
                        Is1WeekRequest = true;

                    return Is1WeekLoad;
                }

                if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMonth)
                {
                    if (!Is1MonthRequest)
                        Is1MonthRequest = true;

                    return Is1MonthLoad;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяем, загружался ли данный таймфрейм
        /// </summary>
        public bool CheckTimeFrameLoadCrypto(int timeFrame, string typeTimeFrame)
        {
            if (typeTimeFrame == CfgSourceEts.TypeTimeFrameNotUse ||
                typeTimeFrame == CfgSourceEts.TypeTimeFrameTicks)
                return true;

            if (!Is1MinutesRequest)
            {
                Is1MinutesRequest = true;
            }

            if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                return Is1MinutesLoad;


            if (timeFrame == 5 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
            {
                if (!Is5MinutesRequest)
                    Is5MinutesRequest = true;

                return Is5MinutesLoad;
            }

            if (timeFrame == 10 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
            {
                if (!Is10MinutesRequest)
                    Is10MinutesRequest = true;

                return Is10MinutesLoad;
            }

            if (timeFrame == 15 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
            {
                if (!Is15MinutesRequest)
                    Is15MinutesRequest = true;

                return Is15MinutesLoad;
            }

            if (timeFrame == 30 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
            {
                if (!Is30MinutesRequest)
                    Is30MinutesRequest = true;

                return Is30MinutesLoad;
            }

            if (timeFrame == 60 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
            {
                if (!Is60MinutesRequest)
                    Is60MinutesRequest = true;

                return Is60MinutesLoad;
            }

            if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameDays)
            {
                if (!Is1DayRequest)
                    Is1DayLoad = true;

                return Is1DayLoad;
            }

            if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameWeek)
            {
                if (!Is1WeekRequest)
                    Is1WeekRequest = true;

                return Is1WeekLoad;
            }

            if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMonth)
            {
                if (!Is1MonthRequest)
                    Is1MonthRequest = true;

                return Is1MonthLoad;
            }

            return false;
        }

        /// <summary>
        /// Таймфрейм загружен
        /// </summary>
        public void IsLoadTimeFrame(int timeFrame, string typeTimeFrame)
        {
            if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                Is1MinutesLoad = true;

            if (timeFrame == 5 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                Is5MinutesLoad = true;

            if (timeFrame == 10 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                Is10MinutesLoad = true;

            if (timeFrame == 15 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                Is15MinutesLoad = true;

            if (timeFrame == 30 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                Is30MinutesLoad = true;

            if (timeFrame == 60 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMinutes)
                Is60MinutesLoad = true;

            if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameDays)
                Is1DayLoad = true;

            if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameWeek)
                Is1WeekLoad = true;

            if (timeFrame == 1 && typeTimeFrame == CfgSourceEts.TypeTimeFrameMonth)
                Is1MonthLoad = true;
        }
    }
}
