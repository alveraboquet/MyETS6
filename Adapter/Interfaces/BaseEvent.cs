using CommonDataContract.AbstractDataTypes;
using SourceEts.Models.TimeFrameTransformModel;
using System.Collections.Generic;

namespace Adapter.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseEvent
    {

        #region Передача информации в самый главный лог
        public delegate void SomethingHappened(string something);

        public event SomethingHappened OnSomething;

        public void Raise_OnSomething(string something)
        {
            if (null != OnSomething)
                OnSomething(something);
        }
        #endregion

        #region Вывод сообщения

        public delegate void AddMessage(string something, string title = "");

        public event AddMessage OnAddMessage;

        public void Raise_AddMessage(string something, string title = "")
        {
            if (null != OnAddMessage)
                OnAddMessage(something, title);
        }
        #endregion

        #region Добавление только записи в текстовый файл-лог

        public delegate void AddWriteToLog(string something);

        public event AddWriteToLog OnWriteToLog;

        public void Raise_AddWriteToLog(string something)
        {
            if (null != OnWriteToLog)
                OnWriteToLog(something);
        }
        #endregion

        #region Изменение подключение, при ассинхронном ответе сервера

        public delegate void ConnectionResultEvent();

        public event ConnectionResultEvent OnConnectionResultEvent;

        /// <summary>
        /// Изменение подключение, при ассинхронном ответе сервера
        /// </summary>
        public void Raise_ConnectionResultEvent()
        {
            if (null != OnConnectionResultEvent)
                OnConnectionResultEvent();
        }
        #endregion


        #region Данные по инструментам на указанном типе терминала
        public delegate void AddNewInstr(ISecurity something);

        public event AddNewInstr OnAddNewInstr;

        /// <summary>
        /// Добаыление торгуемых инструментов в список
        /// </summary>
        /// <param name="something"></param>
        public void Raise_AddNewInstr(ISecurity something)
        {
            if (null != OnAddNewInstr)
                OnAddNewInstr(something);
        }
        #endregion


        #region События для таблиц


        #region Таблица текущих параметров
        public delegate void AddCurrentTable(ISecurity something, AbstractTerminal abstractTerminal);

        public event AddCurrentTable OnAddCurrentTable;

        /// <summary>
        /// Добаыление/изменине данный в таблице текущих парамтеров
        /// </summary>
        /// <param name="something"></param>
        public void Raise_AddCurrentTable(ISecurity something, AbstractTerminal abstractTerminal)
        {
            if (null != OnAddCurrentTable)
                OnAddCurrentTable(something, abstractTerminal);
        }
        #endregion

        #region Таблица текущих параметров добавление пачки данных
        public delegate void AddCurrentTableListSecurity(List<ISecurity> securities, AbstractTerminal abstractTerminal);

        public event AddCurrentTableListSecurity OnAddCurrentTableListSecurity;

        /// <summary>
        /// Добаыление/изменине данный в таблице текущих парамтеров (обновление пачки данных)
        /// </summary>
        /// <param name="securities"></param>
        /// <param name="abstractTerminal"></param>
        public void Raise_AddCurrentTableListSecurity(List<ISecurity> securities, AbstractTerminal abstractTerminal)
        {
            if (null != OnAddCurrentTableListSecurity)
                OnAddCurrentTableListSecurity(securities, abstractTerminal);
        }
        #endregion

        #region Тики, таблица всех сделок
        public delegate void AddAllTradeDealsOrTick(ITick something, AbstractTerminal abstractTerminal, ICandle candleUpdate);

        public event AddAllTradeDealsOrTick OnAddAllTradeDealsOrTick;

        /// <summary>
        /// Добаыление/изменине данный в таблице текущих парамтеров
        /// </summary>
        /// <param name="something"></param>
        /// <param name="abstractTerminal"></param>
        /// <param name="isUpdateCandle">Для обновления свечами, когда тики не загружаются. Сделано для IbTws</param>
        public void Raise_AddAllTradeDealsOrTick(ITick something, AbstractTerminal abstractTerminal, ICandle candleUpdate)
        {
            if (null != OnAddAllTradeDealsOrTick)
                OnAddAllTradeDealsOrTick(something, abstractTerminal, candleUpdate);
        }
        #endregion

        #region Позиции по акциям
        public delegate void AddPositionShares(IPositionShares something, AbstractTerminal abstractTerminal);

        public event AddPositionShares OnAddPositionShares;

        /// <summary>
        /// Добаыление/изменине данный в таблице позиций по акциям
        /// </summary>
        /// <param name="something"></param>
        public void Raise_AddPositionShares(IPositionShares something, AbstractTerminal abstractTerminal)
        {
            if (null != OnAddPositionShares)
                OnAddPositionShares(something, abstractTerminal);
        }
        #endregion

        #region Позиции по фьючерсам
        public delegate void AddPositionFutures(IPositionFutures something, AbstractTerminal abstractTerminal);

        public event AddPositionFutures OnAddPositionFutures;

        /// <summary>
        /// Добаыление/изменине данный в таблице позиций по фьючерсам
        /// </summary>
        /// <param name="something"></param>
        public void Raise_AddPositionFutures(IPositionFutures something, AbstractTerminal abstractTerminal)
        {
            if (null != OnAddPositionFutures)
                OnAddPositionFutures(something, abstractTerminal);
        }
        #endregion

        #region Денежные средства по акциям
        public delegate void AddLimitMoneyShares(IMoneyShares something, AbstractTerminal abstractTerminal);

        public event AddLimitMoneyShares OnAddLimitMoneyShares;

        /// <summary>
        /// Добаыление/изменине данный в таблице лимитов денежных средств по акциям
        /// </summary>
        /// <param name="something"></param>
        public void Raise_AddLimitMoneyShares(IMoneyShares something, AbstractTerminal abstractTerminal)
        {
            if (null != OnAddLimitMoneyShares)
                OnAddLimitMoneyShares(something, abstractTerminal);
        }
        #endregion

        #region Денежные средства по фьючерсам
        public delegate void AddLimitMoneyFutures(IMoneyFutures something, AbstractTerminal abstractTerminal);

        public event AddLimitMoneyFutures OnAddLimitMoneyFutures;

        /// <summary>
        /// Добаыление/изменине данный в таблице лимитов денежных средств по фьючерсам
        /// </summary>
        /// <param name="something"></param>
        public void Raise_AddLimitMoneyFutures(IMoneyFutures something, AbstractTerminal abstractTerminal)
        {
            if (null != OnAddLimitMoneyFutures)
                OnAddLimitMoneyFutures(something, abstractTerminal);
        }
        #endregion

        #region Сделки
        public delegate void AddDeals(IDeals something, AbstractTerminal abstractTerminal);

        public event AddDeals OnAddDeals;

        /// <summary>
        /// Добаыление/изменине данный в таблице сделок
        /// </summary>
        /// <param name="something"></param>
        public void Raise_AddDeals(IDeals something, AbstractTerminal abstractTerminal)
        {
            if (null != OnAddDeals)
                OnAddDeals(something, abstractTerminal);
        }


        public delegate void AddDealsList(List<IDeals> list);

        public event AddDealsList OnAddDealsList;

        /// <summary>
        /// Добавление листа сделок в общую таблицу
        /// </summary>
        /// <param name="something"></param>
        public void Raise_AddDealsList(List<IDeals> list)
        {
            if (null != OnAddDealsList)
                OnAddDealsList(list);
        }
        #endregion

        #region Заявки
        public delegate void AddOrders(IOrders something, AbstractTerminal abstractTerminal);

        public event AddOrders OnAddOrders;

        /// <summary>
        /// Добаыление/изменине данный в таблице заявок
        /// </summary>
        /// <param name="something"></param>
        public void Raise_AddOrders(IOrders something, AbstractTerminal abstractTerminal)
        {
            if (null != OnAddOrders)
                OnAddOrders(something, abstractTerminal);
        }

        public delegate void AddOrdersList(List<IOrders> list);

        public event AddOrdersList OnAddOrdersList;

        /// <summary>
        /// Добавление списка заявок
        /// </summary>
        /// <param name="something"></param>
        public void Raise_AddOrdersList(List<IOrders> list)
        {
            if (null != OnAddOrdersList)
                OnAddOrdersList(list);
        }
        #endregion

        #endregion

        #region Исторические данные с сервера по свечам

        public delegate void HistoryCandleFromServer(Candles something);

        public event HistoryCandleFromServer OnHistoryCandleFromServer;

        /// <summary>
        /// Исторические данные с сервера по свечам
        /// </summary>
        /// <param name="something"></param>
        public void Raise_HistoryCandleFromServer(Candles something)
        {
            if (null != OnHistoryCandleFromServer)
                OnHistoryCandleFromServer(something);
        }

        #endregion

        #region Обновление свечи

        public delegate void UpdateCandleFromServer(Candles candle);

        public event UpdateCandleFromServer OnUpdateCandleFromServer;

        /// <summary>
        /// Исторические данные с сервера по свечам
        /// </summary>
        /// <param name="candle">свечи</param>
        public void Raise_UpdateCandleFromServer(Candles candle)
        {
            if (null != OnUpdateCandleFromServer)
                OnUpdateCandleFromServer(candle);
        }

        #endregion


        #region Добавление инструментов в таблицу выбора инструментов
        /// <summary>
        /// 
        /// </summary>
        /// <param name="security"></param>
        /// <param name="bySort">группировка</param>
        public delegate void AddSortInstr(AbstractTerminal terminal, ISecurity security, string bySort);

        public event AddSortInstr OnAddSortInstr;

        public void Raise_AddSortInstr(AbstractTerminal terminal, ISecurity security, string bySort)
        {
            if (null != OnAddSortInstr)
                OnAddSortInstr(terminal, security, bySort);
        }
        #endregion


    }
}
