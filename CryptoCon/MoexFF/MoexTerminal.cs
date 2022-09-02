using System;
using System.Collections.Generic;

using Adapter;
using Adapter.Model;
using CommonDataContract.AbstractDataTypes;
using SourceEts;
using SourceEts.Models.TimeFrameTransformModel;
using SourceEts.Terminals;

namespace Moex.MoexFF
{
    public class MoexTerminal : AbstractTerminal
    {
        protected MoexTerminal() : base()
        {
        }

        // AbstractTerminal

        #region Обработка проверки данных о инструменте в таблице

        /// <summary>
        /// Обработка данных по инструментам на которые осуществелена подписка
        /// </summary>
        /// <param name="secItem">информация по инструменту от терминала или биржи</param>
        public override void AddSecurity(ISecurity secItem)
        {
            //
        }

        /// <summary>
        /// Обработка данных по позициям
        /// </summary>
        /// <param name="positionSharesTable">таблица с параметрами по всем позициям</param>
        /// <param name="posItem">информация по позиции от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public override bool AddPositionShare(List<IPositionShares> positionSharesTable, IPositionShares posItem)
        {
            return false;
        }

        /// <summary>
        /// Обработка данных по позициям
        /// </summary>
        /// <param name="positionFutureTable">таблица с параметрами по всем позициям фьючерсы</param>
        /// <param name="posItem">информация по позиции от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public override bool AddPositionFuture(List<IPositionFutures> positionFutureTable, IPositionFutures posItem)
        {
            return false;
        }

        /// <summary>
        /// Обработка данных по денежным средствам
        /// </summary>
        /// <param name="moneyShareTable">таблица с параметрами по всем денежным средствам</param>
        /// <param name="moneyItem">информация по денежным средствам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему добавить false</returns>
        public override bool AddMoneyShare(List<IMoneyShares> moneyShareTable, IMoneyShares moneyItem)
        {
            return false;
        }

        /// <summary>
        /// Обработка данных по денежным средствам
        /// </summary>
        /// <param name="moneyFutureTable">таблица с параметрами по всем денежным средствам</param>
        /// <param name="moneyItem">информация по денежным средствам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public override bool AddMoneyFuture(List<IMoneyFutures> moneyFutureTable, IMoneyFutures moneyItem)
        {
            return false;
        }

        /// <summary>
        /// Обработка данных по заявкам
        /// </summary>
        /// <param name="orderTable">таблица с параметрами по всем заявкам пользователя</param>
        /// <param name="orderItem">информация по заявкам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public override bool AddOrder(List<IOrders> orderTable, IOrders orderItem)
        {
            return false;
        }

        /// <summary>
        /// Обработка данных по сделкам
        /// </summary>
        /// <param name="dealTable">таблица с параметрами по всем сделкам</param>
        /// <param name="dealItem">информация по сделкам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public override bool AddDeal(List<IDeals> dealTable, IDeals dealItem)
        {
            return false;
        }

        /// <summary>
        /// Обработка данных по стоп-заявкам
        /// </summary>
        /// <param name="stopTable">таблица с параметрами по всем стоп-заявкам</param>
        /// <param name="stopItem">информация по стоп-заявке от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public override bool AddStop(List<IStop> stopTable, IStop stopItem)
        {
            return false;
        }

        #endregion

        /// <summary>
        /// Первичная инициализация пользовательского коннектора. Создаются все необходимые переменные.
        /// </summary>
        /// <param name="path"></param>
        public override void ExecuteUserConnector(string path)
        {
            //
        }

        /// <summary>
        /// Получить все настроенные соединения
        /// </summary>
        /// <returns></returns>
        public override List<ITerminalInfo> GetTerminalSetting()
        {
            return new List<ITerminalInfo>();
        }

        /// <summary>
        /// Удалить все настроенные соединения
        /// </summary>
        /// <returns></returns>
        public override void DelTerminalSetting(ITerminalInfo setiing)
        {
            //
        }

        /// <summary>
        /// Получить все настроенные соединения
        /// </summary>
        /// <returns></returns>
        public override void AddTerminalSetting(ITerminalInfo setiing)
        {
            //
        }

        /// <summary>
        /// Соединение с терминалом
        /// </summary>
        public override bool Connect()
        {
            return false;
        }

        /// <summary>
        /// Отсоединение от терминала
        /// </summary>
        public override void Disconnect()
        {
            //
        }

        #region Get data on security

        /// <summary>
        /// Get instance Security 
        /// </summary>
        /// <returns></returns>
        public override ISecurity GetSecurity(string instrument, string classCode)
        {
            return null;
        }

        #endregion

        #region GetOrderAndStop

        /// <summary>
        /// Get List All orders 
        /// </summary>
        /// <returns></returns>
        public override List<IOrders> GetOrders()
        {
            return new List<IOrders>();
        }

        /// <summary>
        /// Get List orders 
        /// </summary>
        /// <param name="clientCode">счет клиента</param>
        /// <param name="instrument">инструмента</param>
        /// <returns></returns>
        public override List<IOrders> GetOrders(string clientCode, string instrument)
        {
            return new List<IOrders>();
        }

        /// <summary>
        /// Get List All  stop-orders
        /// </summary>
        /// <returns></returns>
        public override IList<IStop> GetStops()
        {
            return new List<IStop>();
        }

        /// <summary>
        /// Get List stop-orders
        /// </summary>
        /// <param name="clientCode">счет клиента</param>
        /// <param name="account">инструмента</param>
        /// <returns></returns>
        public override IList<IStop> GetStops(string clientCode, string account)
        {
            return new List<IStop>();
        }

        #endregion

        #region Сохраненине настроенных соединений

        /// <summary>
        /// Сохранение настроенных квиков
        /// </summary>
        public override void SaveSettings()
        {
            //
        }

        /// <summary>
        /// Загрузка настроенных квиков
        /// </summary>
        public override void LoadSettings()
        {
            //
        }

        #endregion

        #region Работа с транзакциями

        /// <summary>
        /// Открыть позицию 
        /// </summary>
        public override string TransactionOrder(ParamOfTransactionModel paramOfTransaction)
        {
            return "";
        }

        #region Table

        /// <summary>
        /// Возвращаем таблицу текущих параметров
        /// </summary>
        /// <returns></returns>
        public override List<ISecurity> GetTableCurrentParam()
        {
            return new List<ISecurity>();
        }

        /// <summary>
        /// Возвращаем таблицу всех сделок или тиков
        /// </summary>
        /// <returns></returns>
        public override List<ITick> GetTableAllTradesOrTick()
        {
            return new List<ITick>();
        }

        /// <summary>
        /// Возвращаем таблицу заявок
        /// </summary>
        /// <returns></returns>
        public override List<IOrders> GetTableOrders()
        {
            return new List<IOrders>();
        }

        /// <summary>
        /// Возвращаем таблицу стоп-заявок
        /// </summary>
        /// <returns></returns>
        public override List<IStop> GetTableStopOrders()
        {
            return new List<IStop>();
        }

        /// <summary>
        /// Возвращаем таблицу сделок
        /// </summary>
        /// <returns></returns>
        public override List<IDeals> GetTableDeals()
        {
            return new List<IDeals>();
        }

        /// <summary>
        /// Возвращаем таблицу параметров опционов
        /// </summary>
        /// <returns></returns>
        public override List<ISecurity> GetTableOptions()
        {
            return new List<ISecurity>();
        }

        /// <summary>
        /// Возвращаем таблицу денежные средства по акциям
        /// </summary>
        /// <returns></returns>
        public override List<IMoneyShares> GetTableLimitMoneyShares()
        {
            return new List<IMoneyShares>();
        }

        /// <summary>
        /// Возвращаем таблицу позиции на ММВБ
        /// </summary>
        /// <returns></returns>
        public override List<IPositionShares> GetTablePositionShares()
        {
            return new List<IPositionShares>();
        }

        /// <summary>
        /// Возвращаем таблицу денежные средства по фьючерсам
        /// </summary>
        /// <returns></returns>
        public override List<IMoneyFutures> GetTableLimitMoneyFurures()
        {
            return new List<IMoneyFutures>();
        }

        /// <summary>
        /// Возвращаем таблицу позиции на фортс
        /// </summary>
        /// <returns></returns>
        public override List<IPositionFutures> GetTablePositionFutures()
        {
            return new List<IPositionFutures>();
        }

        #endregion

        #endregion

        #region Сервисные функции

        /// <summary>
        /// Серверное время
        /// </summary>
        public override DateTime GetServerTime()
        {
            return DateTime.Now;
        }

        #endregion

        /// <summary>
        /// Полчучить список таймфреймов, по которым можно подгружать информацию
        /// </summary>
        /// <returns></returns>
        public override List<string> GetTimeFramesSupplyData()
        {
            return new List<string>();
        }

        /// <summary>
        /// Загрузака данных по указанному таймфрейму
        /// </summary>
        /// <param name="timeFrame"></param>
        /// <param name="dateStart"></param>
        /// <param name="deep"></param>
        /// <param name="instr">данные по инструментам которые вводятся руками как в IB TWS</param>
        /// <returns></returns>
        public override Candles GetHistoryData(AvalibleInstrumentsModel symbol, string timeFrame, DateTime dateStart, DateTime dateEnd, int deep, ForeignInstrumentModel instr = null)
        {
            return new Candles();
        }

        public override void GetSymbolsSupplyData()
        {
            //
        }
    }
}
