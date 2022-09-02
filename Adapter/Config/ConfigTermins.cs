using System;
using System.Collections.Generic;

namespace Adapter.Config
{
    public class ConfigTermins
    {
        #region Типы криптосчетов
        public static string TypeCryptoAccountSpot = "Спот";
        public static string TypeCryptoAccountMargin = "Маржинальный";
        public static string TypeCryptoAccountFutures = "Фьючерсы";
        #endregion

        #region Хранилище данных

        public static string HistoryStragesTypeCandle = "Свечи";
        public static string HistoryStragesTypeAllTrades = "Тики (сделки)";
        public static string HistoryStragesTypeQuotes = "Стаканы";
        public static string HistoryStragesTypeOrdersLog = "OrdersLog";
        public static List<string> HistoryStragesTypeList = new List<string> { HistoryStragesTypeCandle, HistoryStragesTypeAllTrades, HistoryStragesTypeOrdersLog };

        #endregion


        public static string ColorStatusConnectionYellow = "yellow";
        public static string ColorStatusConnectionGreen = "green";
        public static string ColorStatusConnectionWhite = "white";

        public static string Buy = "Купля";
        public static string Sell = "Продажа";

        /// <summary>
        /// Для таблицы всех сделок, с целью уменьшения занимаемой памяти
        /// </summary>
        public static char BuyForAllTradeTable = 'B';
        public static char SellForAllTradeTable = 'S';

        public static string DirectionOpenLong = "Открытие лонг";
        public static string DirectionCloseLong = "Закрытие лонг";
        public static string DirectionOpenShort = "Открытие шорт";
        public static string DirectionCloseShort = "Закрытие шорт";
        public static string DirectionKill = "Снятие заявки/стоп-заявки";

        public static String DealOpen = "Open";
        public static String DealClose = "Close";

        public static String DirectionLong = "Длинная";
        public static String DirectionShort = "Короткая";

        #region Status Orders and StopOrders
        public static string Active = "Активна";
        public static string Performed = "Исполнена";
        public static string Cancel = "Снята";
        public static string AddToTs = "Выставлена заявка в ТС";
        public static string RemoveFormTs = "Отвергнута ТС";
        //Дополнительные значения для тарнзак
        public static string BrokerCancel = "Отклонена брокером";
        public static string Linkwait = "Выставлена в торговую систему";
        public static string SlExecuted = "Выставлена в торговую систему";
        public static string PerformedStop = "Исполнена по стопу";
        public static string PerformedProfit = "Исполнена по профиту";
        public static string LimitsCheckFailed = "Не прошла контроль лимитов";
        #endregion

        public static string TypeOptionPut = "Put";
        public static string TypeOptionCall = "Call";

        #region Тип открытой позиции в роботе

        /// <summary>
        /// Открытые позиции только реальные
        /// </summary>
        public static string TypeOpenPosIsReal = "real";
        /// <summary>
        /// Открытые позиции только виртуальные
        /// </summary>
        public static string TypeOpenPosIsVirtual = "virtual";
        /// <summary>
        /// Открытые позиции реальные и виртуальные
        /// </summary>
        public static string TypeOpenPosIsRealAndVirtual = "realAndVirtual";

        #endregion

        #region Table

        public static string MainTableOrder = "Таблица заявок";
        public static string MainTableDeal = "Таблица сделок";
        public static string MainTableStopOrder = "Таблица стоп-заявок";
        public static string MainTablePositionMmvb = "Таблица позиций по акциям";
        public static string MainTableFortsPosition = "Таблица позиций по фьючерсам";
        public static string MainTableLimitMoneyShares = "Таблица лимитов по акциям";
        public static string MainTableLimitMoneyFutures = "Таблица лимитов по фьючерсам";
        public static string MainTableTtp = "Таблица текущих параметров";
        public static string MainTableHistory = "История";
        public static string MainTableAllTrades = "Таблица всех сделок";
        public static string MainTableOption = "Таблица опционы";
        public static string MainTableGlasses = "Стаканы";

        #endregion

        /// <summary>
        /// Преобразование в double в зависимостии от знаказа разделителя
        /// </summary>
        /// <param name="tmp"></param>
        /// <returns></returns>
        public static double ConvertToDoubleMy(string tmp)
        {
            if (String.IsNullOrEmpty(tmp) ||
                tmp == "." || tmp == ",")
            {
                return 0;
            }

            Char pointChar =
                Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (pointChar == '.')
                return Convert.ToDouble(tmp.Replace(',', '.'));

            return Convert.ToDouble(tmp.Replace('.', ','));
        }

    }
}
