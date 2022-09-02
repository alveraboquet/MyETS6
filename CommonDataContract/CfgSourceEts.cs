using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace CommonDataContract
{
    public static class CfgSourceEts
    {


        public static string TypeTimeActionOpen = "Открытие позиции";
        public static string TypeTimeActionClose = "Закрытие позиции";
        public static string TypeTimeActionOpenAndClose = "Открытие и закрытие позиции";
        public static string TypeTimeActionKillAndClose = "Снятие и принудительное закрытие";
        public static List<string> TypeTimeActions = new List<string> { TypeTimeActionOpenAndClose, TypeTimeActionOpen, TypeTimeActionClose, TypeTimeActionKillAndClose };

        public static string OrderLimit = "Лимитная";
        public static string OrderMarket = "Рыночная";
        public static List<string> TypeOrdersOpenClose = new List<string> { OrderMarket, OrderLimit };

        public static string OrderRemoveTypePricePercent = "Проценты";
        public static string OrderRemoveTypePricePoint = "Шагах цены";
        public static List<string> OrderRemoveTypePriceList = new List<string> { OrderRemoveTypePricePoint, OrderRemoveTypePricePercent };

        public static string OrderByCloseFixTime = "Исполнение в указанное время";
        public static string OrderByClosePeriodTime = "Исполнение за указанное время";
        public static string OrderByCloseCurCandle = "Исполнение на текущей свече";

        public static string AllMarkets = "Все рынки";
        public static string AllInsrumetns = "Все инструменты";
        public static string ComissionTypeFix = "Фиксированная";
        public static string ComissionTypePercents = "Проценты";
        public static string ComissionMaxTypeFromVolume = "Процент от сделки";
        public static string ComissionMaxTypeNotUse = "Не использовать";
        public static List<string> ComissionMaxTypeLis = new List<string> { ComissionMaxTypeNotUse, ComissionMaxTypeFromVolume };

        public static double MyEpsilon = 0.000000001;
        public static int RoundTo = 9;

        /// <summary>
        /// Возвращаем количество лотов с кратностью лотам
        /// </summary>
        /// <param name="curQty"></param>
        /// <param name="lotSize"></param>
        /// <returns></returns>
        public static double GetLotAccuracy(double curQty, double lotSize)
        {
            if (lotSize >= 1)
                return Math.Round(curQty, 0);
            if (lotSize > 0 && lotSize < 1)
            {
                //Получаем кратные лоты
                double lot = Math.Round(curQty / lotSize, 0) * lotSize;
                //Округляем до нужного знака
                var lotInt = Math.Truncate(lotSize);
                int count = 0;
                if (lotSize.ToString().Contains('.'))
                {
                    var arr = (lotSize - lotInt).ToString().Split('.');
                    count = arr[1].Length;
                }
                else
                {
                    if (lotSize.ToString().Contains(','))
                    {
                        var arr = (lotSize - lotInt).ToString().Split(',');
                        count = arr[1].Length;
                    }
                }

                return Math.Round(lot, count);
            }

            return curQty;
        }

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
        public static String DirectionOnlyCalc = "не открывать";

        public static string SizeMmvbStock = "Бумага";
        public static string SizeMmvbLot = "Лот";
        public static List<string> SizeMmvb = new List<string> { SizeMmvbLot, SizeMmvbStock };

        public static string TypeTorgLong = "Лонг";
        public static string TypeTorgShort = "Шорт";
        public static string TypeTorgLongAndShort = "Лонг и шорт";
        public static List<string> TypeTorg = new List<string> { TypeTorgLong, TypeTorgShort, TypeTorgLongAndShort };

        public static string TypeRobotRobot = "Автоматический";
        public static string TypeRobotRobotSovetnic = "Полуавтоматический";
        public static string TypeRobotSovetnic = "Ручной";
        public static string TypeRobotTest = "Тест";
        public static string TypeRobotDebug = "Debug";
        public static List<string> TypeRobot = new List<string> { TypeRobotSovetnic, TypeRobotRobotSovetnic, TypeRobotRobot };

        public static string TypeCalculationTrader = "Автоматически";
        public static string TypeCalculationFromEnterMoney = "От входящих ДС";
        public static string TypeCalculationFixSumm = "Фикс. сумма";
        public static string TypeCalculationFromFromScript = "Из скрипта";
        public static List<string> TypeCalculationList = new List<string> { TypeCalculationTrader, TypeCalculationFixSumm, TypeCalculationFromFromScript };

        //цены, от которых рассчитываются стоп-заявки
        public static string StopsTypePriceStrartCalculationMiddle = "Средняя цена";
        public static string StopsTypePriceStrartCalculationSignal = "Цена сигнала";
        public static List<string> StopsTypePriceStrartCalculationList = new List<string> { StopsTypePriceStrartCalculationSignal, StopsTypePriceStrartCalculationMiddle };

        public static string StopsTypePricePercent = "Проценты";
        public static string StopsTypePricePoint = "Шагах цены";
        public static List<string> StopsTypePriceList = new List<string> { StopsTypePricePoint, StopsTypePricePercent };

        public static string TypeSumPercent = "Проценты";
        public static string TypeSumCurrency = "Рубли";
        public static List<string> TypePercentsOrCurrencyList = new List<string> { TypeSumCurrency, TypeSumPercent };

        public static string Buy = "Купля";
        public static string Sell = "Продажа";


        #region Типы инструментов для тестирования
        public static string SymbolTypeUnknow = "Не определено";
        public static string SymbolTypeShare = "Акция";
        public static string SymbolTypeFutures = "Фьючерс";

        public static List<string> SymbolTypeList = new List<string>
        {
            SymbolTypeUnknow,
            SymbolTypeShare,
            SymbolTypeFutures

        };

        public static String ClassCodeSpbopt = "SPBOPT";
        public static String ClasscodeSpbfut = "SPBFUT";
        public static String ClasscodeFut = "FUT";
        public static String ClassCodeQjsim = "QJSIM";
        public static String ClassCodeTqbr = "TQBR";
        public static String ClassCodeTqne = "TQNE";
        public static String ClassCodeTqnl = "TQNL";
        public static String ClassCodeTqns = "TQNS";
        public static String ClassCodeTqbs = "TQBS";
        public static String ClassCodeCets = "CETS";
        #endregion

        #region Портфели

        public static string PortfelTypeSourceFixSumm = "Фиксированная сумма";
        public static string PortfelTypeSourceOfSumEnterMomey = "Входящие денежные средства";
        public static string PortfelTypeSourceOfSumCurrentMomey = "Текущие денежные средства";
        public static string PortfelTypeSourceOfSumPortfel = "Расчет в портфеле"; //указывается в роботах, чтоб было ясно откуда ссума появилась

        public static List<string> PortfelTypeSourceOfSumList = new List<string> { PortfelTypeSourceFixSumm, PortfelTypeSourceOfSumEnterMomey, PortfelTypeSourceOfSumCurrentMomey };

        #endregion

        public static String Unknown = "Не выбран";
        public static String StopOrderNotUse = "Не использовать";
        public static String StopLimit = "Стоп-лимит";
        public static string TrallingProfit = "Треллинг-профит";
        public static string LinkedOrder = "Стоп-лимит и профит";
        public static string StopAndTrallinProfit = "Треллинг-профит и стоп-лимит";
        public static string TakeProfitLimitOrder = "Профит(Лимит. заявка)";


        #region Очередность исполнения стопов и профитов при тесте

        public static string PriorityPerfomedStopOrProfitOnTestProftiThenStop = "Профит потом стоп";
        public static string PriorityPerfomedStopOrProfitOnTestStopThenProfit = "Cтоп потом профит";
        public static string PriorityPerfomedStopOrProfitOnTestOnCandle = "В зависимости от свечи";

        public static List<string> PriorityPerfomedStopOrProfitOnTestList = new List<string>
        {
            PriorityPerfomedStopOrProfitOnTestStopThenProfit,
            PriorityPerfomedStopOrProfitOnTestProftiThenStop,
            PriorityPerfomedStopOrProfitOnTestOnCandle
        };

        #endregion


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


        public enum EnumStatusOrders
        {
            Null,
            Active,
            Performed,
            Cancel,
            AddToTs,
            RemoveFormTs,
            Unknow,
        }

        public enum EnumTypeOrders
        {
            NewOrder, //Получаем соотношения покупок к продажам за период
            StopLimit,
            TrallingProfit,
            StopLimitAndTralling,
            LinkedOrder,
            KillOrder,
            KillStop,
            //UserOrder
        }

        public static EnumStatusOrders GetStatusOrder(string statusRus)
        {
            switch (statusRus)
            {
                case "Активна": return EnumStatusOrders.Active;
                case "Исполнена": return EnumStatusOrders.Performed;
                case "Снята": return EnumStatusOrders.Cancel;
                case "Выставлена заявка в ТС": return EnumStatusOrders.AddToTs;
                case "Отвергнута ТС": return EnumStatusOrders.RemoveFormTs;
                case "Не выбран": return EnumStatusOrders.Unknow;
            }

            return EnumStatusOrders.Null;
        }

        public static string VolSaveDataInStoragesDay = "Дней";
        public static string VolSaveDataInStoragesCandle = "Свечей";
        public static string VolSaveDataInStoragesIsNotSave = "Не хранить в памяти";

        public static List<string> VolSaveDataInStoragesList = new List<string>
        {
            VolSaveDataInStoragesCandle,
            VolSaveDataInStoragesDay,
        };

        public static List<string> VolSaveDataInStoragesFullList = new List<string>
        {
            VolSaveDataInStoragesCandle,
            VolSaveDataInStoragesDay,
            VolSaveDataInStoragesIsNotSave
        };


        #region Тип интервала

        public static string TypeTimeFrameDomQsh = "DOM QSH";
        public static string TypeTimeFrameNotUse = "Не использовать";
        public static string TypeTimeFrameTicks = "Тик";
        public static string TypeTimeFrameSeconds = "Секунда";
        public static string TypeTimeFrameMinutes = "Минута";
        public static string TypeTimeFrameDays = "День";
        public static string TypeTimeFrameWeek = "Неделя";
        public static string TypeTimeFrameMonth = "Месяц";
        public static List<string> TypeTimeFrame = new List<string> { TypeTimeFrameTicks, TypeTimeFrameSeconds, TypeTimeFrameMinutes, TypeTimeFrameDays, TypeTimeFrameWeek, TypeTimeFrameMonth, TypeTimeFrameNotUse };
        public static List<string> TypeTimeFrameTest = new List<string> { TypeTimeFrameTicks, TypeTimeFrameSeconds, TypeTimeFrameMinutes, TypeTimeFrameDays, TypeTimeFrameWeek, TypeTimeFrameMonth };

        #endregion

        #region Тип стандартных интервалов

        public static string TimeFrame1Tick = "Тики";
        public static string TimeFrame1Minute = "1 минута";
        public static string TimeFrame5Minute = "5 минут";
        public static string TimeFrame10Minute = "10 минут";
        public static string TimeFrame15Minute = "15 минут";
        public static string TimeFrame30Minute = "30 минут";
        public static string TimeFrame1hour = "1 час";
        public static string TimeFrame2hour = "2 часа";
        public static string TimeFrame4hour = "4 часа";
        public static string TimeFrame1Day = "1 день";
        public static string TimeFrame1Week = "1 неделя";
        public static string TimeFrame1Month = "1 месяц";

        public static List<string> TimeFrameList = new List<string>
        {
            TimeFrame1Tick,
            TimeFrame1Minute,
            TimeFrame5Minute,
            TimeFrame10Minute,
            TimeFrame15Minute,
            TimeFrame30Minute,
            TimeFrame1hour,
            TimeFrame2hour,
            TimeFrame4hour,
            TimeFrame1Day,
            TimeFrame1Week,
            TimeFrame1Month,
        };

        public static string GetTimeFrame(int period, string typeTimeFrame)
        {
            if (period == 1 && typeTimeFrame == TypeTimeFrameTicks)
                return TimeFrame1Tick;
            if (period == 1 && typeTimeFrame == TypeTimeFrameMinutes)
                return TimeFrame1Minute;
            if (period == 5 && typeTimeFrame == TypeTimeFrameMinutes)
                return TimeFrame5Minute;
            if (period == 10 && typeTimeFrame == TypeTimeFrameMinutes)
                return TimeFrame10Minute;
            if (period == 15 && typeTimeFrame == TypeTimeFrameMinutes)
                return TimeFrame15Minute;
            if (period == 30 && typeTimeFrame == TypeTimeFrameMinutes)
                return TimeFrame30Minute;
            if (period == 60 && typeTimeFrame == TypeTimeFrameMinutes)
                return TimeFrame1hour;
            if (period == 120 && typeTimeFrame == TypeTimeFrameMinutes)
                return TimeFrame2hour;
            if (period == 240 && typeTimeFrame == TypeTimeFrameMinutes)
                return TimeFrame4hour;
            if (period == 1 && typeTimeFrame == TypeTimeFrameDays)
                return TimeFrame1Day;
            if (period == 1 && typeTimeFrame == TypeTimeFrameWeek)
                return TimeFrame1Week;
            if (period == 1 && typeTimeFrame == TypeTimeFrameMonth)
                return TimeFrame1Month;

            return TimeFrame1Month;

        }

        public static string GetTimeFrameFromSecond(int seconds)
        {
            if (seconds < 60)
                return TimeFrame1Tick;
            if (seconds == 60)
                return TimeFrame1Minute;
            if (seconds == 300)
                return TimeFrame5Minute;
            if (seconds == 600)
                return TimeFrame10Minute;
            if (seconds == 900)
                return TimeFrame15Minute;
            if (seconds == 1800)
                return TimeFrame30Minute;
            if (seconds == 3600)
                return TimeFrame1hour;
            if (seconds == 7200)
                return TimeFrame2hour;
            if (seconds == 14400)
                return TimeFrame4hour;
            if (seconds == 86400)
                return TimeFrame1Day;
            if (seconds == 604800)
                return TimeFrame1Week;

            return TimeFrame1Month;
        }

        /// <summary>
        /// Получаем заданный таймфрейм в секундах
        /// </summary>
        /// <param name="period"></param>
        /// <param name="typeTimeFrame"></param>
        /// <returns></returns>
        public static int GetTimeFrameSeconds(int period, string typeTimeFrame)
        {
            if (typeTimeFrame == TypeTimeFrameSeconds)
                return period;
            if (typeTimeFrame == TypeTimeFrameMinutes)
                return period * 60;

            return 0;

        }

        #endregion

        #region Типы свечей

        public static string TypeCandleHloc = "Свеча/бар";
        public static string TypeCandleQsh = "Данные Qscalp";
        public static string TypeCandleClasterTime = "Кластер по времени";
        public static string TypeCandleClasterHigh = "Кластера по высоте";
        public static string TypeCandleRangeBar = "RangeBar";

        #endregion




        #region Типы терминалов

        public static string AdapterUnknown = "Не определен";
        public static string AdapterCryptoMarket = "Крипторынки";


        public static List<string> AllAdapters = new List<string> {  AdapterCryptoMarket };

        #endregion


        #region Риски для роботов

        public static string RiskTypeChangeByAccount = "Изменение по счету";
        public static string RiskTypeDdByAccount = "Остаток на счете";
        public static string RiskTypeDrowDawn = "Убыток";
        public static string RiskTypeDrowDawnFix = "Убыток зафиксированный";
        public static string RiskTypeDrowDawnFact = "Убыток (Факт)";
        public static string RiskTypeProfit = "Прибыль";
        public static string RiskTypeProfitFix = "Прибыль зафиксированная";
        public static string RiskTypeProfitFact = "Прибыль (Факт)";
        public static string RiskTypeTrallingProfit = "Треллинг профит";
        public static string RiskTypeDefenseProfit = "Защита прибыли";
        public static string RiskTypeTrallingProfitFact = "Треллинг профит (Факт)";
        public static string RiskTypeDefenseProfitFact = "Защита прибыли (Факт)";
        public static string RiskTypeTrallingProfitFix = "Треллинг профит зафиксированная";
        public static string RiskTypeDefenseProfitFix = "Защита прибыли зафиксированный";
        public static string RiskTypeCountDeals = "Количество сделок"; //шт
        public static string RiskTypeCountLossDeals = "Количество убыточных сделок"; //шт
        public static string RiskTypeCountLossDealsSeries = "Убыточных сделок подряд"; //шт
        public static string RiskTypeCountProfitDealSeries = "Прибыльных сделок подряд"; //шт

        public static string RiskTypeLossByPeriod = "Убыток за период";
        public static string RiskTypeCountDealsSeriesByPeriod = "Убыт. сделок за период"; //шт
        public static string RiskTypeProfitByPeriod = "Прибыль за период";
        public static string RiskTypeCounProfitDealsByPeriod = "Приб. сделок за период"; //шт

        public static List<string> RiskTypeList = new List<string>
        {
            RiskTypeChangeByAccount,
            RiskTypeDdByAccount,
            RiskTypeDrowDawn,
            RiskTypeDrowDawnFix,
            RiskTypeProfit,
            RiskTypeProfitFix,
            RiskTypeTrallingProfit,
            RiskTypeDefenseProfit,
            RiskTypeCountDeals,
            RiskTypeCountLossDeals,
            RiskTypeCountLossDealsSeries,
            RiskTypeLossByPeriod,
            RiskTypeCountDealsSeriesByPeriod,
            RiskTypeProfitByPeriod,
            RiskTypeCounProfitDealsByPeriod,
            RiskTypeCountProfitDealSeries,
        };

        public static string RiskTypePointChangeMoney = "Деньги";
        public static string RiskTypePointChangeMoneyFact = "Деньги (факт)";
        public static string RiskTypePointChangeRubles = "Рубли";
        public static string RiskTypePointChangePercnet = "Проценты";
        public static string RiskTypePointChangeStep = "Пункты";
        public static string RiskTypePointChangeCount = "Количество";


        public static string RiskTypeActionPausaSeconds = "Пауза, сек."; //в барах для роботов со свечами, и в секундах для режима не использова
        public static string RiskTypeActionPausaBars = "Пауза, бар"; //в барах для роботов со свечами, и в секундах для режима не использова
        public static string RiskTypeActionStopEndDay = "Остановить до конца дня";
        public static string RiskTypeActionStopEndTimeRisk = "Остановить до \"Время окончания\"";

        #endregion

        #region Переменные для сканера

        public enum EnumScanerParamType
        {
            typeInt,
            typeDouble,
            typeBool,
            typeString,
            typeDateTime,
            typeTime
        }


        #endregion


        #region Поставщик данных

        #region Разделитель полей

        #region Формат ASCII
        public static string FormatFileTxt = "txt";
        public static string FormatFileCsv = "csv";


        public static List<string> FormatFileList = new List<string>
        {
            FormatFileTxt,
            FormatFileCsv
        };

        #endregion

        public static string FieldSeaparatorComma = "запятая (,)";
        public static string FieldSeaparatorPeriod = "точка (.)";
        public static string FieldSeaparatorPeriodAndComma = "точка с запятой (;)";
        public static string FieldSeaparatorSpace = "пробел ( )";
        public static string FieldSeaparatorTab = "табуляция";

        public static List<string> FieldSeparatorList = new List<string>
        {
            FieldSeaparatorComma,
            FieldSeaparatorPeriod,
            FieldSeaparatorPeriodAndComma,
            FieldSeaparatorSpace,

        };

        /// <summary>
        /// Получить символ разделителя полей
        /// </summary>
        /// <param name="separaotr"></param>
        /// <returns></returns>
        public static char GetSeparatorField(string separaotr)
        {
            if (separaotr == FieldSeaparatorComma)
                return ',';
            if (separaotr == FieldSeaparatorPeriod)
                return '.';
            if (separaotr == FieldSeaparatorPeriodAndComma)
                return ';';
            if (separaotr == FieldSeaparatorTab)
                return '\t';

            return ' ';
        }

        #endregion

        #region Формат даты
        public static string DateFormatDdSlashMmSlashYyyy = "дд/мм/гггг";//
        public static string DateFormatDdDashMmDashYyyy = "дд-мм-гггг";//
        public static string DateFormatDdMmYyyy = "ддммгггг";
        public static string DateFormatDdPointMmPointYyyy = "дд.мм.гггг";//

        public static string DateFormatYyyySlashMmSlashDd = "гггг/мм/дд";
        public static string DateFormatYyyyDashMmDashhDd = "гггг-мм-дд";
        public static string DateFormatYyyyMmDd = "ггггммдд";
        public static string DateFormatYyyyPointMmPointDd = "гггг.мм.дд";


        public static List<string> DateFormatList = new List<string>
         {
             DateFormatDdMmYyyy,
             DateFormatDdPointMmPointYyyy,
             DateFormatDdSlashMmSlashYyyy,
             DateFormatDdDashMmDashYyyy,

             DateFormatYyyyMmDd,
             DateFormatYyyyPointMmPointDd,
             DateFormatYyyySlashMmSlashDd,
             DateFormatYyyyDashMmDashhDd,
         };

        public static string GetDateFormat(DateTime date, string format)
        {
            string dateString = "";
            string day = date.Day > 9 ? date.Day.ToString() : "0" + date.Day.ToString();
            string month = date.Month > 9 ? date.Month.ToString() : "0" + date.Month.ToString();
            string year = date.Year.ToString();

            if (format == DateFormatDdSlashMmSlashYyyy)
                dateString = day + "/" + month + "/" + year;
            if (format == DateFormatDdDashMmDashYyyy)
                dateString = day + "-" + month + "-" + year;
            if (format == DateFormatDdMmYyyy)
                dateString = day + "" + month + "" + year;
            if (format == DateFormatDdPointMmPointYyyy)
                dateString = day + "" + month + "" + year;
            if (format == DateFormatYyyySlashMmSlashDd)
                dateString = year + "/" + month + "/" + day;
            if (format == DateFormatYyyyDashMmDashhDd)
                dateString = year + "-" + month + "-" + day;
            if (format == DateFormatYyyyMmDd)
                dateString = year + "" + month + "" + day;
            if (format == DateFormatYyyyPointMmPointDd)
                dateString = year + "." + month + "." + day;

            return dateString;

        }

        #endregion


        #region Формат время
        public static string DateFormatHhMmSs = "ччммсс";
        public static string DateFormatHhPointDoubleMmPointDoubleSs = "чч:мм:сс";


        public static List<string> TimeFormatList = new List<string>
         {
            DateFormatHhPointDoubleMmPointDoubleSs,
            DateFormatHhMmSs
         };

        public static string GetTimeFormat(DateTime date, string format)
        {
            string timeString = "";
            string hour = date.Hour > 9 ? date.Hour.ToString() : "0" + date.Hour.ToString();
            string min = date.Minute > 9 ? date.Minute.ToString() : "0" + date.Minute.ToString();
            string sec = date.Second > 9 ? date.Second.ToString() : "0" + date.Second.ToString();

            if (format == DateFormatHhMmSs)
                timeString = hour + "" + min + "" + sec;
            if (format == DateFormatHhPointDoubleMmPointDoubleSs)
                timeString = hour + ":" + min + ":" + sec;

            return timeString;

        }


        #endregion


        #region День очередной загрзуки данных
        public static string DayRefreshMonday = "Понедельник";
        public static string DayRefreshTuesday = "Вторник";
        public static string DayRefreshWensday = "Среда";
        public static string DayRefreshThursday = "Четверг";
        public static string DayRefreshFriday = "Пятница";
        public static string DayRefreshSaturday = "Суббота";
        public static string DayRefreshSunday = "Воскресенье";


        public static List<string> DayRefreshList = new List<string>
         {
            DayRefreshMonday,
            DayRefreshTuesday,
            DayRefreshWensday,
            DayRefreshThursday,
            DayRefreshFriday,
            DayRefreshSaturday,
            DayRefreshSunday,
         };

        #endregion

        #region Формат сохранения данных
        public static string FormatField1 = "Ticker Period Date Time Open High Low Close Volume Oi";
        public static string FormatField2 = "Date Time Open High Low Close Volume Oi";
        public static string FormatField3 = "Date Time Last Vol Id Oper Oi";


        public static List<string> FormatFieldList = new List<string>
         {
            FormatField1,
            FormatField2,
         };

        #endregion


        #region Интервал обновления
        public static string IntervalRefreshNot = "Не обновлять";
        public static string IntervalRefreshMonth = "Раз в месяц";
        public static string IntervalRefreshWeek = "Раз в неделю";
        public static string IntervalRefreshDay = "Раз в день";


        public static List<string> IntervalRefreshList = new List<string>
         {
            IntervalRefreshNot,
            IntervalRefreshMonth,
            IntervalRefreshWeek,
            IntervalRefreshDay,
         };

        #endregion

        #region Интервал обновления
        public static string RefreshIsSaveNullCandle = "Сохранять";
        public static string RefreshIsNotSaveNullCandle = "Не сохранять";


        public static List<string> RefreshIsSaveNullCandleList = new List<string>
         {
            RefreshIsNotSaveNullCandle,
            RefreshIsSaveNullCandle,
         };

        #endregion

        #endregion

        #region IB TWS

        public static List<string> IbTwsListCurrency = new List<string> { "USD", "GBP", "EUR" };
        public static List<string> IbTwsListTypeInstrument = new List<string> { "STK", "FUT", "OPT", "IND", "CASH" };
        public static List<string> IbTwsListExchange = new List<string> { "SMART", "NYSE", "AMEX", "ECBOT", "NYMEX", "GLOBEX", "NYBOT", "IDEALPRO" };

        #endregion

        #region Иностранные биржи, размер котировки

        public static string FormatCodesNotSelect = "Указывать не требуется";
        public static string FormatCodes1_4 = "1/4";
        public static string FormatCodes1_8 = "1/8";
        public static string FormatCodes1_32 = "1/32";

        public static List<string> FormatCodesList = new List<string> { FormatCodesNotSelect, FormatCodes1_4, FormatCodes1_8, FormatCodes1_32 };

        /// <summary>
        /// Возвращаем формат кода
        /// </summary>
        /// <param name="formatCode"></param>
        /// <returns></returns>
        public static double GetFormatCodesOnNumber(string formatCode)
        {
            if (FormatCodes1_4 == formatCode)
                return 4;
            if (FormatCodes1_8 == formatCode)
                return 8;
            if (FormatCodes1_32 == formatCode)
                return 32;

            return 0;
        }

        public static string GetStringFormatCode(int code)
        {
            if (4 == code)
                return FormatCodes1_4;
            if (8 == code)
                return FormatCodes1_8;
            if (32 == code)
                return FormatCodes1_32;

            return FormatCodesNotSelect;
        }

        #endregion

        #region Источники данных

        public static string TypeFormatXml = "XML";
        public static string TypeFormatTxt = "TXT";

        #endregion


        #region Индикаторы

        public enum EnumTypeLine
        {
            Line,
            LineDash,
            LineDot,
            Dotted,
            Histogram,
            StepLine,
            Triangle,
            InvertedTriangle
        }

        public static List<EnumTypeLine> EnumTypeLineList = new List<EnumTypeLine> { EnumTypeLine.Line, EnumTypeLine.LineDash, EnumTypeLine.LineDot, EnumTypeLine.Dotted, EnumTypeLine.Histogram };

        #endregion

        private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();


        /// <summary>
        /// Получения рандомной переменной
        /// </summary>
        public static int GetRandom(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];

            _generator.GetBytes(randomNumber);

            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

            // We are using Math.Max, and substracting 0.00000000001, 
            // to ensure "multiplier" will always be between 0.0 and .99999999999
            // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

            // We need to add one to the range, to allow for the rounding done with Math.Floor
            int range = maximumValue - minimumValue + 1;

            double randomValueInRange = Math.Floor(multiplier * range);

            return (int)(minimumValue + randomValueInRange);
        }


        #region DownLoader

        public static string DownLoaderTicks = "Тики";
        public static string DownLoader1Minutes = "1 минута";
        public static string DownLoader5Minutes = "5 минута";
        public static string DownLoader10Minutes = "10 минут";
        public static string DownLoader15Minutes = "15 минут";
        public static string DownLoader30Minutes = "30 минут";
        public static string DownLoader1Hour = "1 час";
        public static string DownLoader4Hour = "4 часа";
        public static string DownLoader1Day = "1 день";
        public static string DownLoader1Week = "1 неделя";
        public static string DownLoader1Month = "1 месяц";


        #endregion


        #region типы приказов для транзакций

        public static string TypeRequestForTransactionOrder = "NewOrder";
        public static string TypeRequestForTransactionKillOrder = "KillOrder";

        public static string TypeRequestForTransactionStopLimit = "StopLimit";
        public static string TypeRequestForTransactionTrallingProfit = "TrallingProfit";
        public static string TypeRequestForTransactionStopLimitAndtralling = "StopLimitAndTralling";
        public static string TypeRequestForTransactionKillStopOrder = "CancelStopOrder";
        public static string TypeRequestForTransactionLinkedOrder = "LinkedOrder";
        public static string TypeRequestForTransactionMoveOrder = "MoveOrder";
        public static string TypeRequestForTransactionStopOrderWithOtherElement = "StopOrderWithOtherElemetn";
        #endregion
    }


}
