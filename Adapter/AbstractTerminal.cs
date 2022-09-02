using Adapter.Config;
using Adapter.Interfaces;
using Adapter.Logic.LogicServerHistory;
using Adapter.Model;
using CommonDataContract.AbstractDataTypes;
using SourceEts;
using SourceEts.Models.TimeFrameTransformModel;
using SourceEts.Terminals;
using SourceEts.UserConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace Adapter
{
    public abstract class AbstractTerminal
    {
        //public List<IOrders> OrdersListAll { get; private set; }
        //public List<IStop> StopOrderListAll { get; private set; }
        //public List<IDeals> DealListAll { get; private set; }
        public Dispatcher MainDispatcher { get; private set; }
        private static readonly DataStorage dataStorage = DataStorage.Instance;

        AllTerminals _allTerminals;

        public void SetTableAllTerminals(AllTerminals allTerminals)
        {
            //OrdersListAll = dataStorage.OrdersList;
            //StopOrderListAll = allTerminals.StopOrders;
            //DealListAll = allTerminals.Deals;
            _allTerminals = allTerminals;
            MainDispatcher = _allTerminals.MainDispatcher;
        }

        #region Для пользовательского коннектора

        #region Свойства пользовательского коннектора


        public string NameUserAdapter { get; set; }
        /// <summary>
        /// Путь сохранения данных
        /// </summary>
        public string PathSave { get; set; }

        private BaseListTableModel _baseTable;

        /// <summary>
        /// Базовые таблицы 
        /// </summary>
        public BaseListTableModel BaseTable
        {
            get { return _baseTable ?? (_baseTable = new BaseListTableModel()); }
            set
            {
                if (_baseTable != null && _baseTable != value)
                {
                    _baseTable = value;
                }
            }
        }

        private List<ITerminalInfo> _listTerminalInfo;

        /// <summary>
        /// Настроенные соединения
        /// </summary>
        public List<ITerminalInfo> ListTerminalInfo
        {
            get { return _listTerminalInfo ?? (_listTerminalInfo = new List<ITerminalInfo>()); }
            set
            {
                if (_listTerminalInfo != null && _listTerminalInfo != value)
                {
                    _listTerminalInfo = value;
                }
            }
        }



        private List<SettingUserCon> _userConSetting;

        /// <summary>
        /// Настройки для коннектора, выводимые в интерфейсе
        /// </summary>
        public List<SettingUserCon> UserConSetting
        {
            get { return _userConSetting ?? (_userConSetting = new List<SettingUserCon>()); }
            set
            {
                if (_userConSetting != null && _userConSetting != value)
                {
                    _userConSetting = value;
                }
            }
        }

        private List<SettingUserCon> _commonConSetting;

        /// <summary>
        /// Общие настройки для терминалов данного типа
        /// </summary>
        public List<SettingUserCon> CommonConSetting
        {
            get { return _commonConSetting ?? (_commonConSetting = new List<SettingUserCon>()); }
            set
            {
                if (_commonConSetting != null && _commonConSetting != value)
                {
                    _commonConSetting = value;
                }
            }
        }

        #endregion

        #region Обработка проверки данных о инструменте в таблице
        /// <summary>
        /// Обработка данных по инструментам на которые осуществелена подписка
        /// </summary>
        /// <param name="securityTable">таблица с параметрами по всем инструментам</param>
        /// <param name="secItem">информация по инструменту от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        //public abstract void AddSecurity(List<ISecurity> securityTable, ISecurity secItem);
        public abstract void AddSecurity(ISecurity secItem);
        /// <summary>
        /// Обработка данных по позициям
        /// </summary>
        /// <param name="positionSharesTable">таблица с параметрами по всем позициям</param>
        /// <param name="posItem">информация по позиции от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public abstract bool AddPositionShare(List<IPositionShares> positionSharesTable, IPositionShares posItem);
        /// <summary>
        /// Обработка данных по позициям
        /// </summary>
        /// <param name="positionFutureTable">таблица с параметрами по всем позициям фьючерсы</param>
        /// <param name="posItem">информация по позиции от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public abstract bool AddPositionFuture(List<IPositionFutures> positionFutureTable, IPositionFutures posItem);
        /// <summary>
        /// Обработка данных по денежным средствам
        /// </summary>
        /// <param name="moneyShareTable">таблица с параметрами по всем денежным средствам</param>
        /// <param name="moneyItem">информация по денежным средствам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему добавить false</returns>
        public abstract bool AddMoneyShare(List<IMoneyShares> moneyShareTable, IMoneyShares moneyItem);
        /// <summary>
        /// Обработка данных по денежным средствам
        /// </summary>
        /// <param name="moneyFutureTable">таблица с параметрами по всем денежным средствам</param>
        /// <param name="moneyItem">информация по денежным средствам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public abstract bool AddMoneyFuture(List<IMoneyFutures> moneyFutureTable, IMoneyFutures moneyItem);
        /// <summary>
        /// Обработка данных по заявкам
        /// </summary>
        /// <param name="orderTable">таблица с параметрами по всем заявкам пользователя</param>
        /// <param name="orderItem">информация по заявкам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public abstract bool AddOrder(List<IOrders> orderTable, IOrders orderItem);
        /// <summary>
        /// Обработка данных по сделкам
        /// </summary>
        /// <param name="dealTable">таблица с параметрами по всем сделкам</param>
        /// <param name="dealItem">информация по сделкам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public abstract bool AddDeal(List<IDeals> dealTable, IDeals dealItem);
        /// <summary>
        /// Обработка данных по стоп-заявкам
        /// </summary>
        /// <param name="stopTable">таблица с параметрами по всем стоп-заявкам</param>
        /// <param name="stopItem">информация по стоп-заявке от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public abstract bool AddStop(List<IStop> stopTable, IStop stopItem);

        #endregion

        /// <summary>
        /// Первичная инициализация пользовательского коннектора. Создаются все необходимые переменные.
        /// </summary>
        /// <param name="path"></param>
        public abstract void ExecuteUserConnector(string path);

        #region Загрузка
        public void LoadTerminalInfo()
        {
            var d = (List<TerminalInfo>)GetXmlData();
            foreach (var terminalInfo in d)
            {
                ListTerminalInfo.Add(terminalInfo);
            }
        }
        /// <summary>
        /// Обычный терминал, сохранятся будет в папку connectors
        /// </summary>
        public bool IsCommonTerminl;

        public object GetXmlData()
        {
            String path = PathSave;

            if (!Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                dir.Create();
            }

            if (!IsCommonTerminl)
                path = path + "\\Crypto";
            else
                path = path + "\\ConnectorSettings";
            if (!Directory.Exists(path))
            {
                var dir = new DirectoryInfo(path);
                dir.Create();
            }

            try
            {
                if (File.Exists(path + "\\Common" + NameUserAdapter + ".xml"))
                {
                    var reader = new XmlSerializer(typeof(List<SettingUserCon>));
                    StreamReader file = new StreamReader(path + "\\Common" + NameUserAdapter + ".xml");
                    var tmp = (List<SettingUserCon>)reader.Deserialize(file);
                    foreach (var item in tmp)
                    {
                        foreach (var itemCom in CommonConSetting)
                        {
                            if (item.NameParam == itemCom.NameParam)
                            {
                                itemCom.ValueBool = item.ValueBool;
                                itemCom.ValueString = item.ValueString;
                                itemCom.ValueDigit = item.ValueDigit;
                            }
                        }
                    }


                }
            }
            catch (Exception)
            {
            }

            try
            {
                if (File.Exists(path + "\\Instr" + NameUserAdapter + ".xml"))
                {
                    var reader = new XmlSerializer(typeof(List<AvalibleInstrumentsModel>));
                    StreamReader file = new StreamReader(path + "\\Instr" + NameUserAdapter + ".xml");
                    AvalibleInstruments = (List<AvalibleInstrumentsModel>)reader.Deserialize(file);

                }
            }
            catch (Exception)
            {
            }


            try
            {
                if (File.Exists(path + "\\" + NameUserAdapter + ".xml"))
                {
                    var reader = new XmlSerializer(typeof(List<TerminalInfo>));
                    StreamReader file = new StreamReader(path + "\\" + NameUserAdapter + ".xml");
                    var tmp = (List<TerminalInfo>)reader.Deserialize(file);

                    foreach (var itemTerminal in tmp)
                    {
                        foreach (var itemCom in UserConSetting)
                        {
                            bool add = false;
                            foreach (var item in itemTerminal.UserConSettings)
                            {
                                if (item.NameParam == itemCom.NameParam)
                                {
                                    add = true;
                                }
                            }

                            if (!add)
                            {
                                itemTerminal.UserConSettings.Add(new SettingUserCon
                                {
                                    Description = itemCom.Description,
                                    IsBool = itemCom.IsBool,
                                    IsChangeAfterConnect = itemCom.IsChangeAfterConnect,
                                    IsDigit = itemCom.IsDigit,
                                    IsDontSave = itemCom.IsDontSave,
                                    IsListString = itemCom.IsListString,
                                    IsStirng = itemCom.IsStirng,
                                    IsUseInstrumentSort = itemCom.IsUseInstrumentSort,
                                    NameParam = itemCom.NameParam,
                                    ValueBool = itemCom.ValueBool,
                                    ValueDigit = itemCom.ValueDigit,
                                    ValueListString = itemCom.ValueListString,
                                    ValueString = itemCom.ValueString,

                                });
                            }

                        }

                    }

                    return tmp;

                }
                return new List<TerminalInfo>();
            }
            catch (Exception)
            {
                return new List<TerminalInfo>();
            }



        }
        #endregion

        #region Сохранение

        /// <summary>
        /// Сохранение пользовательских настроек для коннекотора
        /// </summary>
        public void SaveTerminalInfo()
        {
            var listT = new List<TerminalInfo>();
            foreach (var terminalInfo in ListTerminalInfo)
            {
                listT.Add(terminalInfo as TerminalInfo);
            }

            object list = (object)listT;
            string path = PathSave;

            if (!Directory.Exists(path))
            {
                var dir = new DirectoryInfo(path);
                dir.Create();
            }

            if (!IsCommonTerminl)
                path = path + "\\Crypto";
            else
                path = path + "\\ConnectorSettings";
            if (!Directory.Exists(path))
            {
                var dir = new DirectoryInfo(path);
                dir.Create();
            }


            try
            {
                XmlSerializer write = new XmlSerializer(typeof(List<TerminalInfo>));
                using (StreamWriter file = new StreamWriter(path + "\\" + NameUserAdapter + ".xml"))
                    write.Serialize(file, list);
            }
            catch
            {

            }

            //сохранения общих настроек 
            try
            {
                object list2 = (object)CommonConSetting;
                XmlSerializer write = new XmlSerializer(typeof(List<SettingUserCon>));
                using (StreamWriter file = new StreamWriter(path + "\\Common" + NameUserAdapter + ".xml"))
                    write.Serialize(file, list2);
            }
            catch
            {

            }



            //сохранения инструментов
            try
            {
                object list3 = (object)AvalibleInstruments;
                XmlSerializer write = new XmlSerializer(typeof(List<AvalibleInstrumentsModel>));
                using (StreamWriter file = new StreamWriter(path + "\\Instr" + NameUserAdapter + ".xml"))
                    write.Serialize(file, list3);
            }
            catch
            {

            }
        }


        #endregion

        #endregion


        /// <summary>
        /// Список инструментов для триВью
        /// </summary>
        public List<AllInstrForTrwModel> AllInstrForTrwModelList = new List<AllInstrForTrwModel>();
        public List<AvalibleInstrumentsModel> AvalibleInstruments = new List<AvalibleInstrumentsModel>();
        /// <summary>
        /// Произошло изменение количества или замена инструментов
        /// </summary>
        public bool IsUpdateInstrument { get; set; }

        public bool IsPushBtnConnect { get; set; }

        public BaseEvent BaseMon = new BaseEvent();


        public QouteSubscribe QouteSubscribeClass = new QouteSubscribe();

        /// <summary>
        /// Получить все настроенные соединения
        /// </summary>
        /// <returns></returns>
        public abstract List<ITerminalInfo> GetTerminalSetting();
        /// <summary>
        /// Удалить все настроенные соединения
        /// </summary>
        /// <returns></returns>
        public abstract void DelTerminalSetting(ITerminalInfo setiing);
        /// <summary>
        /// Получить все настроенные соединения
        /// </summary>
        /// <returns></returns>
        public abstract void AddTerminalSetting(ITerminalInfo setiing);

        /// <summary>
        /// Завершение загрузки данных таблицы всех сделок
        /// </summary>
        public bool IsLoad { get; set; }
        /// <summary>
        /// Соединение с сервером установлено
        /// </summary>
        public bool IsConnect { get; set; }
        /// <summary>
        /// Время сервера
        /// </summary>
        public DateTime TimeServer { get; set; }
        ///// <summary>
        ///// Требуется очистка, значение переходит в труе, когда меняется дата сервера
        ///// </summary>
        //public bool ClearTable { get; set; }
        /// <summary>
        /// Соединение с терминалом
        /// </summary>
        public abstract bool Connect();

        public abstract void Disconnect();


        #region Get data on security

        /// <summary>
        /// Get instance Security 
        /// </summary>
        /// <returns></returns>
        public abstract ISecurity GetSecurity(string instrument, string classCode);



        #endregion

        #region GetOrderAndStop

        /// <summary>
        /// Get List All orders 
        /// </summary>
        /// <returns></returns>
        public abstract List<IOrders> GetOrders();


        /// <summary>
        /// Get List orders 
        /// </summary>
        /// <param name="clientCode">счет клиента</param>
        /// <param name="instrument">инструмента</param>
        /// <returns></returns>
        public abstract List<IOrders> GetOrders(string clientCode, string instrument);



        /// <summary>
        /// Get List All  stop-orders
        /// </summary>
        /// <returns></returns>
        public abstract IList<IStop> GetStops();


        ///// <summary>
        ///// Get List stop-orders
        ///// </summary>
        ///// <param name="clientCode">счет клиента</param>
        ///// <returns></returns>
        //public abstract IList<IStop> GetStops(string clientCode);


        /// <summary>
        /// Get List stop-orders
        /// </summary>
        /// <param name="clientCode">счет клиента</param>
        /// <param name="account">инструмента</param>
        /// <returns></returns>
        public abstract IList<IStop> GetStops(string clientCode, string account);



        #endregion



        #region Сохраненине настроенных соединений

        /// <summary>
        /// Сохранение настроенных квиков
        /// </summary>
        public abstract void SaveSettings();


        /// <summary>
        /// Загрузка настроенных квиков
        /// </summary>
        public abstract void LoadSettings();


        #endregion


        #region Работа с транзакциями


        /// <summary>
        /// Открыть позицию 
        /// </summary>
        public abstract string TransactionOrder(ParamOfTransactionModel paramOfTransaction);

        #region Table

        /// <summary>
        /// Возвращаем таблицу текущих параметров
        /// </summary>
        /// <returns></returns>
        public abstract List<ISecurity> GetTableCurrentParam();

        /// <summary>
        /// Возвращаем таблицу всех сделок или тиков
        /// </summary>
        /// <returns></returns>
        public abstract List<ITick> GetTableAllTradesOrTick();

        /// <summary>
        /// Возвращаем таблицу заявок
        /// </summary>
        /// <returns></returns>
        public abstract List<IOrders> GetTableOrders();

        /// <summary>
        /// Возвращаем таблицу стоп-заявок
        /// </summary>
        /// <returns></returns>
        public abstract List<IStop> GetTableStopOrders();

        /// <summary>
        /// Возвращаем таблицу сделок
        /// </summary>
        /// <returns></returns>
        public abstract List<IDeals> GetTableDeals();
        /// <summary>
        /// Возвращаем таблицу параметров опционов
        /// </summary>
        /// <returns></returns>
        public abstract List<ISecurity> GetTableOptions();
        /// <summary>
        /// Возвращаем таблицу денежные средства по акциям
        /// </summary>
        /// <returns></returns>
        public abstract List<IMoneyShares> GetTableLimitMoneyShares();
        /// <summary>
        /// Возвращаем таблицу позиции на ММВБ
        /// </summary>
        /// <returns></returns>
        public abstract List<IPositionShares> GetTablePositionShares();
        /// <summary>
        /// Возвращаем таблицу денежные средства по фьючерсам
        /// </summary>
        /// <returns></returns>
        public abstract List<IMoneyFutures> GetTableLimitMoneyFurures();
        /// <summary>
        /// Возвращаем таблицу позиции на фортс
        /// </summary>
        /// <returns></returns>
        public abstract List<IPositionFutures> GetTablePositionFutures();


        #endregion

        #endregion

        #region Сервисные функции

        /// <summary>
        /// Серверное время
        /// </summary>
        public abstract DateTime GetServerTime();

        #endregion



        public void SetColor(ITerminalInfo item)
        {
            item.ColorStatusConnection = ConfigTermins.ColorStatusConnectionWhite;
            if (IsPushBtnConnect)
            {
                item.ColorStatusConnection = ConfigTermins.ColorStatusConnectionGreen;
                if (!item.IsConnect)
                    item.ColorStatusConnection = ConfigTermins.ColorStatusConnectionYellow;
            }
        }

        /// <summary>
        /// Может ли соединение использоваться как поставщик данных
        /// </summary>
        public bool IsSupplyData { get; set; }

        /// <summary>
        /// Полчучить список таймфреймов, по которым можно подгружать информацию
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetTimeFramesSupplyData();

        /// <summary>
        /// Загрузака данных по указанному таймфрейму
        /// </summary>
        /// <param name="timeFrame"></param>
        /// <param name="dateStart"></param>
        /// <param name="deep"></param>
        /// <param name="instr">данные по инструментам которые вводятся руками как в IB TWS</param>
        /// <returns></returns>
        public abstract Candles GetHistoryData(AvalibleInstrumentsModel symbol, string timeFrame, DateTime dateStart, DateTime dateEnd, int deep, ForeignInstrumentModel instr = null);

        public abstract void GetSymbolsSupplyData();

        #region Функции добавления данных в общие таблицы (Новая реализация)

        ///// <summary>
        ///// Добавление данных в таблицу текущих параметров
        ///// </summary>
        ///// <param name="sec"></param>
        //public void AddISecurityCommonTable(List<ISecurity> secList)
        //{
        //    if (_allTerminals.MainDispatcher.CheckAccess())
        //    {
        //        AddSec(secList);
        //        //MessageBox.Show(message, "Внимание");
        //    }
        //    else
        //    {
        //        _allTerminals.MainDispatcher.BeginInvoke(new Action(() =>
        //            AddSec(secList)
        //            ), DispatcherPriority.Normal, null);
        //    }
        //}

        //private void AddSec(List<ISecurity> secList)
        //{
        //    foreach (var item in secList)
        //    {
        //        _allTerminals.CurrentList.Add(item);
        //    }
        //}

        ///// <summary>
        ///// Добавление данных в таблицу текущих параметров
        ///// </summary>
        ///// <param name="sec"></param>
        //internal void AddIMoneyFuturesCommonTable(IMoneyFutures tmp)
        //{
        //    if (_allTerminals.MainDispatcher.CheckAccess())
        //    {
        //        _allTerminals.LimitMoneyFuturesList.Add(tmp);
        //    }
        //    else
        //    {
        //        _allTerminals.MainDispatcher.BeginInvoke(new Action(() =>
        //           _allTerminals.LimitMoneyFuturesList.Add(tmp)
        //            ), DispatcherPriority.Normal, null);
        //    }
        //}

        ///// <summary>
        ///// Добавление данных в таблицу текущих параметров
        ///// </summary>
        ///// <param name="sec"></param>
        //internal void AddIPositionFuturesCommonTable(IPositionFutures tmp)
        //{
        //    if (_allTerminals.MainDispatcher.CheckAccess())
        //    {
        //        _allTerminals.PositionFuturesList.Add(tmp);
        //    }
        //    else
        //    {
        //        _allTerminals.MainDispatcher.BeginInvoke(new Action(() =>
        //           _allTerminals.PositionFuturesList.Add(tmp)
        //            ), DispatcherPriority.Normal, null);
        //    }
        //}

        #endregion
    }
}
