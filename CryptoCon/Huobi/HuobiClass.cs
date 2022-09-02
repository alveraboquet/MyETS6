using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Timers;
using System.Windows.Threading;
using Adapter;
using Adapter.Config;
using Adapter.Logic;
using Adapter.Model;
using CommonDataContract;
using CommonDataContract.AbstractDataTypes;
using CommonDataContract.ReactData;
//using CryptoCon.Extension;
using CryptoCon.Huobi.Clients;
using CryptoCon.Huobi.Enums;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Huobi.Net.Clients;
using Huobi.Net.Enums;
using Huobi.Net.Objects;
using Huobi.Net.Objects.Models;
using ScriptSolution.Model;
using ScriptSolution.Model.OrderAndStopOrderModels;
using SourceEts;
using SourceEts.Config;
using SourceEts.Models.TimeFrameTransformModel;
using SourceEts.Table;
using SourceEts.Table.TableClass;
using SourceEts.Terminals;
using SourceEts.UserConnector;

namespace CryptoCon.Huobi
{
    public class HuobiClass : AbstractTerminal
    {
        public string NameExchange = "Huobi";
        //https://github.com/huobiapi/API_Docs_en/wiki

        private static readonly DataStorage dataStorage = DataStorage.Instance;

        private HuobiWrapper HuobiWrapper { get; set; }

        #region Constructor


        public HuobiClass()
        {
            NameUserAdapter = NameExchange;

            SettingUserCon setting = new SettingUserCon();
            setting.Description = "Публичный ключ";
            setting.NameParam = "Public key";
            setting.IsStirng = true;
            setting.ValueString = "";
            UserConSetting.Add(setting);

            setting = new SettingUserCon();
            setting.Description = "Секретный ключ";
            setting.NameParam = "Secret key";
            setting.IsStirng = true;
            setting.ValueString = "";
            UserConSetting.Add(setting);

            setting = new SettingUserCon();
            setting.Description = "Режим работы коннектора, получение публичной информации - котировки, т.е. данные не требующие ключей, публичный и приватный - запрашивается информация о позициях, сделках, заявках и позвоялет отправлять транакции на биржу.";
            setting.NameParam = "Режим работы коннектора";
            setting.IsListString = true;
            setting.ValueListString = new List<string> { "Публичный", "Публичный и приватный" };//, "Публичный и приватный" 
            setting.ValueString = "Публичный";
            setting.IsChangeAfterConnect = false;
            UserConSetting.Add(setting);

            #region Общие настройки коннектора для всех созданных соединений
            //Данный контрол только для общих настроек терминала
            setting = new SettingUserCon();
            setting.NameParam = "SortInstr";
            setting.IsUseInstrumentSort = true;
            CommonConSetting.Add(setting);

            setting = new SettingUserCon();
            setting.Description = "";
            setting.NameParam = "Тип счета";
            setting.IsListString = true;
            setting.ValueListString = new List<string> { ConfigTermins.TypeCryptoAccountSpot, ConfigTermins.TypeCryptoAccountMargin, ConfigTermins.TypeCryptoAccountFutures };
            setting.ValueString = "Спот";
            setting.IsChangeAfterConnect = true;
            UserConSetting.Add(setting);

            //setting = new SettingUserCon();
            //setting.Description = "Количество заявок на покупку и продажу в orderbook (стакана), в случае установки 0 данные по orderbook не будут поступать";
            //setting.NameParam = "Глубина orderbook (стакана)";
            //setting.IsListString = true;
            //setting.ValueListString = new List<string> { "Не запрашивать", "5", "10", "20" };
            //setting.ValueString = "Не запрашивать";
            //setting.IsChangeAfterConnect = true;
            //CommonConSetting.Add(setting);


            //setting = new SettingUserCon();
            //setting.Description = "Глубина запроса тиковых данных. Тиковые данные идут по всем инструментам отмеченными пользователем. 0 - данные по истории не запрашиваются";
            //setting.NameParam = "Глубина запроса тиков";
            //setting.IsDigit = true;
            //setting.ValueDigit = 0;
            //setting.IsChangeAfterConnect = false;
            //CommonConSetting.Add(setting);

            //setting = new SettingUserCon();
            //setting.Description = "Глубина запроса исторических данных в свечах. Исторические данные запрашиваются один раз при подключении. Рекоммендуюется запросить данные один раз, а потом уже получать только тики при последующей работе ETS. Либо уменьшить глубину запрашиваемой истории, чтоб уменьшить время ожидания на получение информации. 0 - данные по истории не запрашиваются";
            //setting.NameParam = "Глубина истории, свечи";
            //setting.IsDigit = true;
            //setting.ValueDigit = 0;
            //setting.IsChangeAfterConnect = false;
            //CommonConSetting.Add(setting);


            //setting = new SettingUserCon();
            //setting.Description = "Использовать прокси для ускорения получения данных. Перед включением данной возможности, информации по прокси должны быть добавлены";
            //setting.NameParam = "Исп. Proxy";
            //setting.IsBool = true;
            //setting.IsChangeAfterConnect = false;
            //CommonConSetting.Add(setting);


            #endregion

            //IsSupplyData = true;
        }

        BackgroundWorker woker;
        DispatcherTimer _dsTimer = new DispatcherTimer();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">путь сохранения данных</param>
        public override void ExecuteUserConnector(string path)
        {
            //CommonLogic = new CommonLogic(this);
        }

        //private bool _isGetAllSymbols = false;
        /// <summary>
        /// Получаем список тикеров для формирования списка доступных инструментов
        /// </summary>
        private void SetTicker()
        {
            var tickers = HuobiWrapper.GetAllTickers().GetAwaiter().GetResult();
            if (tickers.Any())
            {
                foreach (var ticker in tickers)
                {
                    AddSortInst(this, ticker, ticker.BaseActive);
                }
            }
            else
            {
                BaseMon.Raise_OnSomething($"Huboi SetTicker: не удалось получить инструменты");
            }
        }

        /// <summary>
        /// Формируем список инструментов
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="terminal"></param>
        public void AddSortInst(AbstractTerminal terminal, ISecurity sec, string sortBy)
        {

            List<AllInstrForTrwModel> allInstrForTrwModelList = terminal.AllInstrForTrwModelList;
            List<AvalibleInstrumentsModel> avalibleInstruments = terminal.AvalibleInstruments;
            bool selected = false;
            foreach (var item in avalibleInstruments)
            {
                if (item.Security == null)
                    if (sec.Seccode == item.Symbol && sec.ClassCode == item.ClassCode)
                    {
                        item.ShortName = sec.ShortName;
                        item.Security = sec;
                        selected = true;
                    }
            }

            bool add = true;
            string name = sec.Seccode;
            string classCodeVis = sortBy;


            foreach (var classCode in allInstrForTrwModelList)
            {
                if (classCode.ClassCodeVisible == classCodeVis)
                {
                    int insertIndex = 0;
                    for (int i = 0; i < classCode.SeccodeList.Count; i++)
                    {
                        var secCode = classCode.SeccodeList[i];
                        string nameSecCode = secCode.Symbol;
                        if (secCode != null)
                        {
                            if (nameSecCode == name)
                            {
                                add = false;
                                if (secCode.Security != null)
                                {
                                    secCode.Security.Accuracy = sec.Accuracy;
                                    secCode.Security.MinAmount = sec.MinAmount;
                                    secCode.Security.MinNational = sec.MinNational;
                                    secCode.Security.MinPrice = sec.MinPrice;
                                    secCode.Security.MinStep = sec.MinStep;
                                    secCode.Security.LotSize = sec.LotSize;
                                    secCode.Security.MaxAmount = sec.MaxAmount;
                                    secCode.Security.MaxPrice = sec.MaxPrice;
                                    secCode.Security.PointCost = sec.PointCost;
                                }

                                break;
                            }

                            if (nameSecCode.CompareTo(name) > 0)
                                break;
                        }
                        insertIndex = i + 1;
                    }

                    if (add)
                        classCode.SeccodeList.Insert(insertIndex,
                            new AvalibleInstrumentsModel
                            {
                                ClassCodeVisible = classCodeVis,
                                Symbol = sec.Seccode,
                                ClassCode = sec.ClassCode,
                                ShortName = sec.ShortName,
                                IsSelected = selected,
                                Security = sec
                            });

                    add = false;
                }
            }
            if (add)
            {
                var mod = new AvalibleInstrumentsModel
                {
                    ClassCodeVisible = classCodeVis,

                    Symbol = sec.Seccode,
                    ClassCode = sec.ClassCode,
                    ShortName = sec.ShortName,
                    IsSelected = selected,
                    Security = sec
                };
                int index = 0;
                if (sec.ClassCode == "MCT")
                    for (int i = 0; i < allInstrForTrwModelList.Count; i++)
                    {
                        if (classCodeVis.CompareTo(allInstrForTrwModelList[i].ClassCodeVisible) <= 0)
                            break;
                        index = i + 1;
                    }

                allInstrForTrwModelList.Insert(index, new AllInstrForTrwModel
                {
                    ClassCode = sec.ClassCode,
                    ClassCodeVisible = classCodeVis,
                    SeccodeList = new List<AvalibleInstrumentsModel> { mod }
                });
            }
        }



        #region Соединение и отключение

        bool _isConnectorPrivate = false;
        public override bool Connect()
        {
            _isConnectorPrivate = false;
            IsConnect = false;

            foreach (var item in ListTerminalInfo)
            {
                if (item.IsUse)
                {
                    item.IsConnect = true;
                    item.Comment = "";
                    SetColor(item);
                    //_useTrItems.Add(item as TerminalInfo);

                    foreach (var itemUserCon in (item as TerminalInfo).UserConSettings)
                    {
                        if (itemUserCon.NameParam == "Public key" &&
                            !String.IsNullOrEmpty(itemUserCon.ValueString))
                        {
                            //item.AccountsPairs = new ObservableCollection<AccountsPair>();
                            //item.AccountsString = item.Name + " (" + itemUserCon.ValueString + ")";

                            //item.AccountsPairs.Add(new AccountsPair
                            //{
                            //    Account = itemUserCon.ValueString,
                            //    ClientCode = itemUserCon.ValueString,
                            //    AccountClientCode = NameExchange + ": " + item.Name + " (" + itemUserCon.ValueString + ")",
                            //    TypeClass = NameExchange
                            //});

                            item.AccountsPairs = new ObservableCollection<AccountsPair>();
                            item.AccountsString = item.Name + " (" + itemUserCon.ValueString + ")";

                            string pub = (item as TerminalInfo).UserConSettings.First(a => a.NameParam == "Public key").ValueString;
                            string sec = (item as TerminalInfo).UserConSettings.First(a => a.NameParam == "Secret key").ValueString;

                            string typeMarket = (item as TerminalInfo).UserConSettings.First(a => a.NameParam == "Тип счета").ValueString;
                            string typeRegim = (item as TerminalInfo).UserConSettings.First(a => a.NameParam == "Режим работы коннектора").ValueString;

                            TypeMarket typeMarketUser = TypeMarket.Spot;

                            if (typeMarket.Equals(ConfigTermins.TypeCryptoAccountSpot))
                            {
                                typeMarketUser = TypeMarket.Spot;
                            }

                            if (typeMarket.Equals(ConfigTermins.TypeCryptoAccountFutures))
                            {
                                typeMarketUser = TypeMarket.UsdFutures;
                            }

                            HuobiWrapper = new HuobiWrapper(pub, sec, typeMarketUser, this);
                            if (typeRegim != "Публичный")
                            {
                                _isConnectorPrivate = (item as TerminalInfo).UserConSettings.First(a => a.NameParam == "Режим работы коннектора").ValueString == "Публичный и приватный";
                            }
                            item.AccountsPairs.Add(new AccountsPair
                            {
                                Account = pub,
                                ClientCode = sec,
                                AccountClientCode = NameExchange + ": " + item.Name + " (" + itemUserCon.ValueString + ")",
                                TypeClass = NameExchange
                            });
                        }
                    }
                }
            }

            #region Запуск таймера для обработки запросов

            //var periodRequest = CommonConSetting.First(a => a.NameParam == "Интервал запроса, млсек");
            //int period = 5000;
            //if (periodRequest.ValueDigit > 0)
            //    period = Convert.ToInt32(periodRequest.ValueDigit);
            //if (period == 0)
            //    period = 5000;


            //_historyDeep = (int)CommonConSetting.First(a => a.NameParam == "Глубина истории, свечи").ValueDigit;
            //_tickDeep = (int)CommonConSetting.First(a => a.NameParam == "Глубина запроса тиков").ValueDigit;
            //var glassDeep = (CommonConSetting.First(a => a.NameParam == "Глубина orderbook (стакана)").ValueString);
            //_glassDeep = 0;
            //if (!String.IsNullOrWhiteSpace(glassDeep) && glassDeep != "Не запрашивать")
            //    _glassDeep = Convert.ToInt32(glassDeep);


            woker = new BackgroundWorker();
            woker.DoWork += Work;

            int timeReapet = 2000;


            _dsTimer.Interval = TimeSpan.FromMilliseconds(timeReapet);//Обработка данных 1 раз в 2 секунды
            _dsTimer.Start();
            _dsTimer.Tick += (_dsTimer_Tick);


            #endregion



            return true;
        }



        public void _dsTimer_Tick(Object sender, EventArgs args)
        {

            if (!woker.IsBusy)
                woker.RunWorkerAsync();

        }


        DateTime _lastDateUpdate = new DateTime();
        int _lastCount = 0;
        string _instr = "";
        int _historyDeep = 0;
        int _tickDeep = 0;
        int _glassDeep = 0;
        /// <summary>
        /// Обработка данных всех запросов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Work(object sender, DoWorkEventArgs e)
        {
            //Максимальное количество запросов в секунду 6
            try
            {
                #region Работа с публичными данными

                //Время сервера
                //_data.GetServerTime();
                if (Error.ToUpper().Contains("MAINTENANCE") ||
                    Error.ToUpper().Contains("(502)"))
                {
                    return;
                }

                //var countRequest = CommonConSetting.First(a => a.NameParam == "Кол-во запросов в секунду");
                //if (!String.IsNullOrEmpty(countRequest.ValueString))
                //    _data.CountRequest = Convert.ToInt32(countRequest.ValueString);

                #region Информация по инструментам

                if (//!_isGetAllSymbols ||
                    _lastDateUpdate.Date != DateTime.Now.Date || AvalibleInstruments.Count != _lastCount)
                {
                    if (_lastDateUpdate.Date != DateTime.Now.Date)
                        SetTicker();

                    _lastDateUpdate = DateTime.Now;

                    _lastCount = AvalibleInstruments.Count;

                    _instr = "";
                    bool update = false;
                    foreach (var item in AvalibleInstruments)
                    {
                        bool add = true;
                        //foreach (var itemLoad in _data.DataLoadHisrotyTicks)
                        //{
                        //    if (itemLoad.Symbol == item.Symbol)
                        //        add = false;
                        //}
                        //if (add)
                        //{
                        //    var history = new LoadHisrotyTick { Symbol = item.Symbol, Security = item.Security };
                        //    if (_historyDeep <= 0)
                        //        history.IsHistoryLoad = true;

                        //    if (_tickDeep <= 0)
                        //        history.IsTickLoad = true;

                        //    //_data.DataLoadHisrotyTicks.Add(history);
                        //    update = true;
                        //}

                        //составляем список запрашиваемых инструментов
                        if (String.IsNullOrEmpty(_instr))
                            _instr = _instr + item.Symbol;
                        else
                            _instr = _instr + "," + item.Symbol;
                    }


                    if (!String.IsNullOrEmpty(_instr))
                        for (int i = AvalibleInstruments.Count - 1; i >= 0; i--)
                        {
                            if (AvalibleInstruments[i].Security != null)
                                AddSecurity(AvalibleInstruments[i].Security);
                            //BaseMon.Raise_AddCurrentTable(AvalibleInstruments[i].Security, this);
                        }

                    BaseMon.Raise_OnSomething("hello");

                    //HuobiWrapper.GetHistoryKlinesAsync(new AvalibleInstrumentsModel() { Symbol = "BTC-USDT"}, CfgSourceEts.DownLoader1Minutes, 
                    //    new DateTime(2022, 1, 1), 
                    //    new DateTime(2022, 8, 30))
                    //    .GetAwaiter().GetResult();

                    //HuobiWrapper.PlaceOrderAsync("btcusdt", OrderSide.Buy, OrderType.Limit, quantity: 0.000611m, price: 16500.0m, clientOrderId: "Hello").GetAwaiter().GetResult();

                    List<Task> tasks = new List<Task>();

                    tasks.Add(HuobiWrapper.SubscribeToGlasses(AvalibleInstruments));

                    #region Получаем по Rest API исторические ордера пользователя

                    //HuobiWrapper.GetHistoryOrdersAsync().GetAwaiter().GetResult();

                    tasks.Add(HuobiWrapper.GetHistoryOrdersAsync());

                    #endregion

                    BaseMon.Raise_OnSomething("hello 2");

                    #region Получаем по Rest API открытые ордера пользователя

                    //HuobiWrapper.GetOpenOrdersAsync().GetAwaiter().GetResult();

                    tasks.Add(HuobiWrapper.GetOpenOrdersAsync());

                    #endregion

                    BaseMon.Raise_OnSomething("hello 3");

                    #region Подписка на совершенные сделки, выбранные пользователем

                    //HuobiWrapper.SubscribeToTradesAsync(AvalibleInstruments).GetAwaiter().GetResult();

                    tasks.Add(HuobiWrapper.SubscribeToTradesAsync(AvalibleInstruments));

                    #endregion

                    BaseMon.Raise_OnSomething("hello 4");


                    #region Удаление подписок, если пользователь удалил инструмент

                    #region Подписка на обновление ордеров

                    //HuobiWrapper.SubscribeToUpdateOrder().GetAwaiter().GetResult();

                    tasks.Add(HuobiWrapper.SubscribeToUpdateOrder());

                    #endregion

                    #region Подписка на инстументы, выбранные пользователем

                    //HuobiWrapper.SubcribeToInstrumentsAsync(AvalibleInstruments).GetAwaiter().GetResult();

                    tasks.Add(HuobiWrapper.SubcribeToInstrumentsAsync(AvalibleInstruments));

                    #endregion

                    BaseMon.Raise_OnSomething("hello 5");


                    #region Удаление подписок, если пользователь удалил инструмент

                    //foreach (var item in updateSubscriptions)
                    //{
                    //    socket.UnsubscribeAsync(item);
                    //    break;
                    //}

                    #endregion

                    #endregion

                    #region Получение изменение баланса по сокету

                    //HuobiWrapper.SubscribeToBalanceAsync().GetAwaiter().GetResult();

                    tasks.Add(HuobiWrapper.SubscribeToBalanceAsync());

                    #endregion

                    Task.WaitAll(tasks.ToArray());

                }


                #endregion

                //if (!String.IsNullOrEmpty(_instr))
                //{
                //    foreach (var itemHistory in _data.DataLoadHisrotyTicks)
                //    {
                //        _data.UpdateSecuritiesInfo(itemHistory);
                //    }
                //}

                //#region Исторические данные

                //var isLoadHistoryData = CommonConSetting.First(a => a.NameParam == "Получать истор. данные");
                //var historyDeep = CommonConSetting.First(a => a.NameParam == "Глубина истории, свечи");
                //if (!String.IsNullOrEmpty(historyDeep.ValueString) && isLoadHistoryData.ValueBool)
                //{
                //    int deep = Convert.ToInt32(historyDeep.ValueString);

                //    try
                //    {
                //        foreach (var itemHistory in _dataBinance.DataLoadHisrotyTicks)
                //        {
                //            if (deep > 0 && !itemHistory.IsHistoryLoad)
                //            {
                //                _dataBinance.GetHistoricalData(itemHistory, deep);
                //                itemHistory.IsHistoryLoad = true;
                //                BaseMon.Raise_OnSomething(NameExchange + ". Загрузка исторических данных по " + itemHistory.Symbol + " завершена");

                //            }
                //        }

                //    }
                //    catch (Exception ex)
                //    {
                //        BaseMon.Raise_OnSomething("Ошибка получения исторических данных с " + NameExchange);
                //    }
                //}


                //#endregion


                #region Тики


                //if (_tickDeep > 0)
                //    foreach (var itemHistory in _data.DataLoadHisrotyTicks)
                //    {
                //        _data.GetTiks(itemHistory, _tickDeep);
                //        itemHistory.IsTickLoad = true;

                //    }

                #endregion

                #region Стаканы

                //try
                //{
                //    if (_glassDeep > 0)
                //        foreach (var itemHistory in _data.DataLoadHisrotyTicks)
                //        {
                //            _data.GetOrderBook(itemHistory, _glassDeep);
                //        }
                //}
                //catch (Exception ex)
                //{
                //    BaseMon.Raise_OnSomething("Ошибка определения глубины OrderBook (стакана) с " + NameExchange);
                //}

                #endregion
                #endregion
                //WorkPrivate();

                #region Загрузка данных завершена после первого подключения или после потери связи
                if (!IsConnect)
                {
                    IsConnect = true; //Соединение установлено, только после того как получим всю информацию с биржи
                    IsLoad = true;
                    BaseMon.Raise_OnSomething("Загрузка данных с " + NameExchange + " завершена");
                    //_useTrItems = new List<TerminalInfo>();
                    foreach (var item in ListTerminalInfo)
                    {
                        if (item.IsUse)
                        {
                            item.IsConnect = true;
                            item.Comment = "";
                            SetColor(item);
                        }
                    }

                    Error = "";
                    SendMes = false;
                    BaseMon.Raise_ConnectionResultEvent();
                    //CommonLogic._commonFunction.Alert(ConfigTermins.NoticeConnect, null);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ConnectError(ex);
            }

        }

        private void Data_Exception1(Exception obj)
        {
            BaseMon.Raise_OnSomething($"Data_Exception1: {obj.Message}");
        }

        private void Data_ConnectionLost()
        {
            BaseMon.Raise_OnSomething("Huobi Socket (Data_ConnectionLost), соединение потеряно!");
        }

        private void Data_Exception(Exception obj)
        {
            BaseMon.Raise_OnSomething($"Huobi Socket Data_Exception: { obj.Message }");
        }

        private void Data_ConnectionClosed()
        {
            BaseMon.Raise_OnSomething("Huobi Socket (Data_ConnectionClosed), соединение закрыто!");
        }

        /// <summary>
        /// Инофрмация о потере связи с биржей передана
        /// </summary>
        public bool SendMes;
        public string Error = "";
        /// <summary>
        /// Ошибка получения данных с биржи
        /// </summary>
        /// <param name="ex"></param>
        private void ConnectError(Exception ex)
        {
            if (!SendMes)
            {
                BaseMon.Raise_OnSomething(NameExchange + ". Ошибка: " + ex.Message);
                if (IsConnect)
                {
                    foreach (var terminalInfo in ListTerminalInfo)
                    {
                        if (terminalInfo.IsUse)
                        {
                            terminalInfo.IsConnect = false;
                            terminalInfo.Comment = Error;
                            SetColor(terminalInfo);
                        }
                    }
                    BaseMon.Raise_ConnectionResultEvent();
                    //CommonLogic._commonFunction.Alert(ConfigTermins.NoticeDisconnect, null);
                    IsConnect = false;
                }

                SendMes = true;
            }
        }

        public override void Disconnect()
        {

            foreach (var item in ListTerminalInfo)
            {
                if (item.IsUse)
                {
                    item.IsConnect = false;
                    SetColor(item);
                    //_useTrItems.Add(item as TerminalInfo);
                }
            }
            _dsTimer.Stop();

            BaseMon.Raise_ConnectionResultEvent();
            //CommonLogic._commonFunction.Alert(ConfigTermins.NoticeDisconnect, null);
            IsConnect = false;
            IsLoad = false;
            SendMes = false;
            Error = "";
        }


        #endregion


        #region Проверка и изменение данных в таблицах

        DateTime _lastTimeUpdateSecurity = new DateTime();

        /// <summary>
        /// Проверка наличия данного инструмента в таблице 
        /// </summary>
        /// <param name="securityTable">таблица всех имющихся инструментов</param>
        /// <param name="secItem">информация по инструменту</param>
        /// <returns>в случае отсутствия данных по инструменту в таблице вернуть true - необходимо дабоавить, 
        /// в случае присутствия - false (необходимо обновить информацию по инструменту) </returns>
        public override void AddSecurity(ISecurity secItem)
        {
            var securityTable = BaseTable.CurrentParamModelList;
            try
            {
                if (secItem is Securities) //Обязательная строка проверки данных
                {
                    bool result = true;

                    for (int i = 0; i < securityTable.Count; i++)
                    {
                        var item = securityTable[i];

                        var tmpItem = item as Securities;

                        if (tmpItem != null &&
                            (tmpItem.Seccode == secItem.Seccode))
                        {
                            result = false;
                            if (tmpItem.TimeLastChange > secItem.TimeLastChange)
                                continue;

                            if (secItem.Bid > 0)
                                tmpItem.Bid = secItem.Bid;
                            if (secItem.Accuracy > 0)
                                tmpItem.Accuracy = secItem.Accuracy;
                            if (secItem.GoBuy > 0)
                                tmpItem.GoBuy = secItem.GoBuy;
                            if (secItem.GoSell > 0)
                                tmpItem.GoSell = secItem.GoSell;
                            if (secItem.LastPrice > 0 &&
                                tmpItem.LastPrice != secItem.LastPrice)
                                tmpItem.TimeLastChange = secItem.TimeLastChange;


                            if (secItem.LastPrice > 0)
                            {
                                tmpItem.PrevLastPrice = tmpItem.LastPrice;
                                tmpItem.LastPrice = secItem.LastPrice;
                            }
                            if (secItem.MaxPrice > 0)
                                tmpItem.MaxPrice = secItem.MaxPrice;
                            if (secItem.MinPrice > 0)
                                tmpItem.MinPrice = secItem.MinPrice;
                            if (secItem.Offer > 0)
                                tmpItem.Offer = secItem.Offer;

                            if (secItem.ClosePrice > 0)
                                tmpItem.ClosePrice = secItem.ClosePrice;
                            if (secItem.OpenPrice > 0)
                                tmpItem.OpenPrice = secItem.OpenPrice;
                            if (secItem.MinStep > 0)
                                tmpItem.MinStep = secItem.MinStep;
                            if (secItem.LotSize > 0)
                                tmpItem.LotSize = secItem.LotSize;
                            if (secItem.PointCost > 0)
                                tmpItem.PointCost = secItem.PointCost;
                            if (!String.IsNullOrEmpty(secItem.Status))
                                tmpItem.Status = secItem.Status;
                            if (!String.IsNullOrEmpty(secItem.TradingStatus))
                            {
                                tmpItem.TradingStatus = secItem.TradingStatus;
                                tmpItem.IsTrade = secItem.IsTrade;
                            }
                            break;
                        }
                    }

                    if (result)
                    {
                        for (int i = 0; i < AllInstrForTrwModelList.Count; i++)
                        {
                            bool brek = false;
                            for (int j = 0; j < AllInstrForTrwModelList[i].SeccodeList.Count; j++)
                            {
                                if (AllInstrForTrwModelList[i].SeccodeList[j].Symbol == secItem.Seccode)
                                {
                                    securityTable.Add(AllInstrForTrwModelList[i].SeccodeList[j].Security);
                                    secItem.ShortName = AllInstrForTrwModelList[i].SeccodeList[j].Security.ShortName;
                                    secItem.BaseActive = AllInstrForTrwModelList[i].SeccodeList[j].Security.BaseActive;
                                    dataStorage.AddISecurityCommonTable(MainDispatcher, AllInstrForTrwModelList[i].SeccodeList[j].Security);
                                    brek = true;
                                    break;
                                }
                            }
                            if (brek)
                                break;
                        }
                    }

                }

            }
            catch (Exception ex)
            {

            }

        }



        /// <summary>
        /// Обработка данных по позициям
        /// </summary>
        /// <param name="positionSharesTable">таблица с параметрами по всем позициям</param>
        /// <param name="posItem">информация по позиции от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему и вернуть false</returns>
        public override bool AddPositionShare(List<IPositionShares> positionSharesTable, IPositionShares posItem)
        {
            bool result = false;
            try
            {
                if (posItem is SecPosition) //Обязательная строка проверки данных
                {
                    result = true;
                    var pos = posItem as SecPosition;
                    foreach (var item in positionSharesTable)
                    {
                        var tmpItem = item as SecPosition;
                        if (tmpItem != null && (tmpItem.Symbol == pos.Symbol && tmpItem.ClientCode == pos.ClientCode))
                        {
                            result = false;
                            tmpItem.EnterOst = pos.EnterOst;
                            tmpItem.Bought = pos.Bought;
                            tmpItem.Sold = pos.Sold;
                            tmpItem.Ordbuy = pos.Ordbuy;
                            tmpItem.Ordsell = pos.Ordsell;
                            tmpItem.Saldomin = pos.Saldomin;
                            tmpItem.Balance = pos.Balance;
                            tmpItem.NameSymbol = pos.NameSymbol;
                            tmpItem.Symbol = pos.Symbol;
                            tmpItem.ClientCode = pos.ClientCode;
                            tmpItem.LastTimeUpdate = DateTime.Now;
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                BaseMon.Raise_OnSomething(NameExchange + ". Ошибка при проверке информации по позиции: " + ex.Message);
            }

            return result;
        }



        /// <summary>
        /// Обработка данных по позициям
        /// </summary>
        /// <param name="positionFutureTable">таблица с параметрами по всем позициям 
        /// ы</param>
        /// <param name="posItem">информация по позиции от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему и вернуть false</returns>
        public override bool AddPositionFuture(List<IPositionFutures> positionFutureTable, IPositionFutures posItem)
        {
            bool result = false;
            //try
            //{
            //    if (posItem is FortsPosition) //Обязательная строка проверки данных
            //    {
            //       result = true;
            //        var pos = posItem as FortsPosition;
            //        #region Проверка инструмента в массиве
            //        foreach (var item in positionFutureTable)
            //        {
            //            var tmpItem = item as FortsPosition;
            //            if (tmpItem != null && (tmpItem.Symbol == pos.Symbol && tmpItem.ClientCode == pos.ClientCode))
            //            {
            //                result = false;
            //                tmpItem.Symbol = pos.Symbol;
            //                tmpItem.ClientCode = pos.ClientCode;
            //                tmpItem.EnterEmptyPos = pos.EnterEmptyPos;
            //                tmpItem.CurrentLongPos = pos.CurrentLongPos;
            //                tmpItem.CurrentShortPos = pos.CurrentShortPos;
            //                tmpItem.Balance = pos.Balance;
            //                tmpItem.ActiveBuy = pos.ActiveBuy;
            //                tmpItem.ActiveSell = pos.ActiveSell;
            //                tmpItem.VariableMarga = pos.VariableMarga;
            //                break;
            //            }
            //        }
            //        #endregion

            //    }
            //}
            //catch (Exception ex)
            //{
            //    BaseMon.Raise_OnSomething("Ошибка при проверке информации по позиции фьючерсы: " + ex.Message);
            //}

            return result;
        }

        /// <summary>
        /// Обработка данных по денежным средствам
        /// </summary>
        /// <param name="moneyShareTable">таблица с параметрами по всем денежным средствам</param>
        /// <param name="moneyItem">информация по денежным средствам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему добавить false</returns>
        public override bool AddMoneyShare(List<IMoneyShares> moneyShareTable, IMoneyShares moneyItem)
        {
            bool result = false;
            try
            {
                if (moneyItem is MoneyPosition) //Обязательная строка проверки данных
                {
                    result = true;
                    var money = moneyItem as MoneyPosition;
                    #region Проверка счета в массиве
                    foreach (var item in moneyShareTable)
                    {
                        var tmpItem = item as MoneyPosition;
                        if (tmpItem != null && (tmpItem.ClientCode == money.ClientCode && tmpItem.Currency == money.Currency))
                        {
                            tmpItem.Balance = money.Balance;
                            tmpItem.OpenBalance = money.OpenBalance;

                            result = false;
                            break;

                        }
                    }
                    #endregion
                }

                if (result)
                {
                    dataStorage.AddMoneySharesCommonTable(MainDispatcher, moneyItem);
                    moneyShareTable.Add(moneyItem);
                }
            }
            catch (Exception ex)
            {
                BaseMon.Raise_OnSomething(NameExchange + ". Ошибка при проверке информации по денежным средствам: " + ex.Message);
            }

            return result;
        }



        /// <summary>
        /// Обработка данных по денежным средствам
        /// </summary>
        /// <param name="moneyFutureTable">таблица с параметрами по всем денежным средствам</param>
        /// <param name="moneyItem">информация по денежным средствам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public override bool AddMoneyFuture(List<IMoneyFutures> moneyFutureTable, IMoneyFutures moneyItem)
        {
            bool result = false;
            //try
            //{
            //    if (moneyItem is FortsMoney) //Обязательная строка проверки данных
            //    {
            //       result = true;
            //        var money = moneyItem as FortsMoney;
            //        #region Проверка счета в массиве
            //        foreach (var item in moneyFutureTable)
            //        {
            //            var tmpItem = item as FortsMoney;
            //            if (tmpItem != null && (tmpItem.Account == moneyItem.Account && tmpItem.TypeLimit == money.TypeLimit))
            //            {
            //                result = false;
            //                tmpItem.BirgaSbor = money.BirgaSbor;
            //                tmpItem.BonusByOption = money.BonusByOption;
            //                tmpItem.CountGo = money.CountGo;
            //                tmpItem.CurrentEmptyOpen = money.CurrentEmptyOpen;
            //                tmpItem.CurrentEmptyOrder = money.CurrentEmptyOrder;
            //                tmpItem.CurrentEmptyPosition = money.CurrentEmptyPosition;
            //                tmpItem.Dohod = money.Dohod;
            //                tmpItem.Kmolotiluas = money.Kmolotiluas;
            //                tmpItem.LastLimitOpenPosition = money.LastLimitOpenPosition;
            //                tmpItem.LimitOpenPosition = money.LimitOpenPosition;
            //                tmpItem.PlanEmptyPosition = money.PlanEmptyPosition;
            //                tmpItem.VariableMarga = money.VariableMarga;
            //                break;
            //            }
            //        }
            //        #endregion
            //    }
            //}
            //catch (Exception ex)
            //{
            //    BaseMon.Raise_OnSomething("Ошибка при проверке информации по денежным средствам фьючерсы: " + ex.Message);
            //}

            return result;
        }

        /// <summary>
        /// Обработка данных по заявкам
        /// </summary>
        /// <param name="orderTable">таблица с параметрами по всем заявкам пользователя</param>
        /// <param name="orderItem">информация по заявкам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public override bool AddOrder(List<IOrders> orderTable, IOrders orderItem)
        {
            //var foundedItem = orderTable.Where(x => (x.Number == orderItem.Number && !String.IsNullOrWhiteSpace(orderItem.Number)) 
            //    || (x.Id == orderItem.Id && !String.IsNullOrWhiteSpace(orderItem.Id))).ToList();
            //if (foundedItem.Any())
            //{
            //    dataStorage.AddOrdersCommonTable(MainDispatcher, orderItem);
            //    orderTable.Add(orderItem);

            //    result = true;
            //}
            //else
            //{
            //    dataStorage.AddOrdersCommonTable(MainDispatcher, orderItem);
            //    orderTable.Add(orderItem);

            //    result = true;
            //}


            bool result = false;
            try
            {
                if (orderItem is Orders) //Обязательная строка проверки данных
                {
                    result = true;
                    var order = orderItem as Orders;
                    for (int i = orderTable.Count - 1; i >= 0; i--)
                    {
                        var item = (Orders)orderTable[i];
                        if ((item.Number == order.Number && !String.IsNullOrWhiteSpace(order.Number)) ||
                            (item.Id == order.Id && !String.IsNullOrWhiteSpace(order.Id)))// != 0))
                        {
                            //if (order.Number != 0)
                            if (!String.IsNullOrWhiteSpace(order.Number))
                                item.Number = order.Number;
                            if (order.Status != item.Status && !String.IsNullOrEmpty(order.Status))
                                item.Status = order.Status;
                            if (order.Status == CfgSourceEts.Cancel || order.Status == CfgSourceEts.Performed)
                                // if (order.Quantity <= CfgSourceEts.MyEpsilon && order.Balance > 0 && item.Balance > (item.Quantity - order.Balance))
                                item.Balance = order.Balance;

                            result = false;
                            break;

                        }

                    }

                    //Добавления оредеров. Быстрая проверка после информации после добавления
                    if (result)
                    {
                        dataStorage.AddOrdersCommonTable(MainDispatcher, orderItem);
                        orderTable.Add(orderItem);
                    }
                }
            }
            catch (Exception ex)
            {
                BaseMon.Raise_OnSomething(NameExchange + ". Ошибка при проверке информации по таблице заявок: " + ex.Message);
            }

            return result;
        }



        /// <summary>
        /// Обработка данных по сделкам
        /// </summary>
        /// <param name="dealTable">таблица с параметрами по всем сделкам</param>
        /// <param name="dealItem">информация по сделкам от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public override bool AddDeal(List<IDeals> dealTable, IDeals dealItem)
        {
            bool result = false;
            try
            {
                if (dealItem is Deal) //Обязательная строка проверки данных
                {
                    result = true;
                    var deal = dealItem as Deal;
                    for (int i = dealTable.Count - 1; i >= 0; i--)
                    {
                        var item = (Deal)dealTable[i];
                        if ((item.NumberTrade == deal.NumberTrade && deal.NumberTrade != "0" && !String.IsNullOrWhiteSpace(deal.NumberTrade)
                            && item.Operation == deal.Operation))
                        {
                            result = false;
                            break;
                        }
                    }

                    if (result)
                    {
                        dataStorage.AddDealsCommonTable(MainDispatcher, deal);
                        dealTable.Add(deal);
                    }
                }
            }
            catch (Exception ex)
            {
                BaseMon.Raise_OnSomething("Ошибка при проверке информации по таблице сделок " + NameExchange + ":" + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Обработка данных по стоп-заявкам
        /// </summary>
        /// <param name="stopTable">таблица с параметрами по всем стоп-заявкам</param>
        /// <param name="stopItem">информация по стоп-заявке от терминала или биржи</param>
        /// <returns>в случае если его нет в таблице - вернуть true, в случае если он есть, обновить информацию по нему</returns>
        public override bool AddStop(List<IStop> stopTable, IStop stopItem)
        {
            bool result = false;
            //try
            //{
            //    if (stopItem is StopOrderTrConModel) //Обязательная строка проверки данных
            //    {
            //       result = true;
            //        var stop = stopItem as StopOrderTrConModel;
            //        #region Проверка инструмента в массиве

            //        for (int i = stopTable.Count - 1; i >= 0; i--)
            //        {
            //            var tmpItem = stopTable[i] as StopOrderTrConModel;
            //            if (tmpItem != null && (tmpItem.Number > 0 && tmpItem.Number == stop.Number && tmpItem.Symbol == stop.Symbol &&
            //                tmpItem.ClientCode == stop.ClientCode ||
            //                stop.Id > 0 && tmpItem.Id == stop.Id))
            //            {
            //                result = false;
            //                if (stop.NumberOrder != 0)
            //                    tmpItem.NumberOrder = stop.NumberOrder;
            //                if (stop.Canceller != tmpItem.Canceller && !String.IsNullOrEmpty(stop.Canceller))
            //                    tmpItem.Canceller = stop.Canceller;
            //                if (stop.Accepttime >= tmpItem.Accepttime)
            //                    tmpItem.Accepttime = stop.Accepttime;
            //                if (stop.Number != 0)
            //                    tmpItem.Number = stop.Number;
            //                if (Math.Abs(stop.TpActivationPrice) > Double.Epsilon)
            //                    tmpItem.TpActivationPrice = stop.TpActivationPrice;
            //                if (stop.Linkedorderno != 0)
            //                    tmpItem.Linkedorderno = stop.Linkedorderno;
            //                if (!String.IsNullOrEmpty(stop.Status))
            //                    tmpItem.Status = stop.Status;
            //                if (!String.IsNullOrEmpty(stop.Result))
            //                    tmpItem.Result = stop.Result;
            //                if (Math.Abs(stop.TpExtremum) > Double.Epsilon)
            //                    tmpItem.TpExtremum = stop.TpExtremum;

            //                break;
            //            }
            //        }
            //        #endregion

            //    }
            //}
            //catch (Exception ex)
            //{
            //    BaseMon.Raise_OnSomething("Ошибка при проверке информации по таблице сделок: " + ex.Message);
            //}

            return result;
        }
        #endregion




        #endregion










        ///// <summary>
        ///// Все счета клиентов доступных по данному терминалу/бирже
        ///// В случае если только счет, то код клиента равен счету
        ///// TypeClass - не обязательное поле
        ///// </summary>
        ///// <returns></returns>
        //public override List<AccountsPair> GetPairClienCodesAccount()
        //{
        //    List<AccountsPair> result = new List<AccountsPair>();
        //    foreach (var item in ListTerminalInfo)
        //    {
        //        if (item.IsUse)
        //            foreach (var acc in item.AccountsPairs)
        //            {
        //                result.Add(acc);
        //            }
        //    }
        //    return result;
        //}

        //public override IList<string> GetClienCodes()
        //{
        //    List<string> result = new List<string>();
        //    foreach (var item in ListTerminalInfo)
        //    {
        //        if (item.IsUse)
        //            result.Add(item.AccountsString);
        //    }
        //    return result;
        //}



        //private bool _addAdditionalLogOfTrasaction = true;

        public override string TransactionOrder(ParamOfTransactionModel paramOfTransaction)
        {
            if (paramOfTransaction.TypeOrder == CfgSourceEts.TypeRequestForTransactionOrder)
                BaseMon.Raise_OnSomething("Параметры транзакции." +
                       "Код клиента=" + paramOfTransaction.ClientCode +
                       " Счет=" + paramOfTransaction.Account +
                       " Код класса=" + paramOfTransaction.ClassCode +
                       " Инструмент=" + paramOfTransaction.Symbol +
                       " Объем=" + paramOfTransaction.Quantity +
                       " Цена=" + paramOfTransaction.Price);

            if (paramOfTransaction.TypeOrder != CfgSourceEts.TypeRequestForTransactionKillOrder)
                if (paramOfTransaction.IsMarketOrder != "M")
                {
                    if (paramOfTransaction.Price <= 0)
                    {
                        BaseMon.Raise_OnSomething("Price must be greater than zero.");
                        return "0";
                    }
                }


            if (paramOfTransaction.Quantity <= 0 && paramOfTransaction.TypeOrder != CfgSourceEts.TypeRequestForTransactionKillOrder
                   && paramOfTransaction.TypeOrder != CfgSourceEts.TypeRequestForTransactionKillStopOrder)
            {
                BaseMon.Raise_OnSomething("Отмена отправки транзакции, т.к. количестов лотов/контрактов 0 или меньше.");
                return "0";
            }

            //if (paramOfTransaction.TypeOrder == CfgSourceEts.TypeRequestForTransactionOrder)
            //{
            //    return _data.PostNewOrder( paramOfTransaction);
            //}

            //if (paramOfTransaction.TypeOrder == CfgSourceEts.TypeRequestForTransactionKillOrder)
            //{
            //    _data.CancelUserOrder( paramOfTransaction);
            //}

            return "0";
        }


        #region Проверка на загрузку всех данных для работы робота: тики, котировки и исторические данные

        ///// <summary>
        ///// Проверка на загрузку всех данных для работы робота: тики, котировки и исторические данные.
        ///// В случае необходимости осущетсвляется подписка на соответсвующие данные.
        ///// </summary>
        ///// <param name="brm"></param>
        //public override void SubScribeTickAndQuote(BaseRobotModel brm)
        //{
        //    brm.IsHistoryLoad = true;
        //    brm.IsTickLoad = true;
        //    //if (!brm.IsHistoryLoad || !brm.IsSubscibeQuote || !brm.IsTickLoad)
        //    //{
        //    //    bool tick = true;
        //    //    bool history = true; //переменные для определения все ли загрзилось для работы данного робота и по всем ли инструментам
        //    //    if (!brm.IsHistoryLoad || !brm.IsSubscibeQuote)
        //    //        Sibscribes(brm, brm.Instrument);
        //    //    // int interval = GetTimeFrameFromDic(brm);
        //    //    if (brm.TypeTimeframe != CfgSourceEts.TypeTimeFrameTicks &&
        //    //          brm.TypeTimeframe != CfgSourceEts.TypeTimeFrameSeconds &&
        //    //          brm.TypeTimeframe != CfgSourceEts.TypeTimeFrameNotUse)
        //    //        if (!brm.IsHistoryLoad)
        //    //            history = MainSettings.TerminalSettng.TrConSetting.DeepHistory > 0 ?
        //    //                QouteSubscribeClass.IsLoadHistory(brm.Instrument, brm.Timeframe, brm.TypeTimeframe, this) : true;

        //    //    if (!brm.IsTickLoad && brm.TypeTimeframe != CfgSourceEts.TypeTimeFrameNotUse)
        //    //        tick = QouteSubscribeClass.IsLoadTick(brm.Instrument);
        //    //    //Подписываемся на другие инструменты, если для работы в роботе они учавствуют в работе
        //    //    foreach (var item in brm.ParamOptimizationsRobot)
        //    //    {
        //    //        if (item.MethodName == Script.EnumTypeGetIEnumerable.GetSeccodeList.ToString())
        //    //        {
        //    //            if (!String.IsNullOrEmpty(item.ValueString))
        //    //            {
        //    //                Sibscribes(brm, item.ValueString);
        //    //                if (brm.TypeTimeframe != CfgSourceEts.TypeTimeFrameTicks &&
        //    //                    brm.TypeTimeframe != CfgSourceEts.TypeTimeFrameSeconds &&
        //    //                    brm.TypeTimeframe != CfgSourceEts.TypeTimeFrameNotUse)
        //    //                    if (history && !brm.IsHistoryLoad)
        //    //                        history = MainSettings.TerminalSettng.TrConSetting.DeepHistory > 0 ?
        //    //                            QouteSubscribeClass.IsLoadHistory(item.ValueString,
        //    //                            brm.Timeframe, brm.TypeTimeframe, this) : true;

        //    //                if (brm.TypeTimeframe != CfgSourceEts.TypeTimeFrameNotUse)
        //    //                    if (tick && !brm.IsTickLoad)
        //    //                        tick = QouteSubscribeClass.IsLoadTick(item.ValueString);
        //    //            }
        //    //        }
        //    //    }

        //    //    brm.IsHistoryLoad = history;
        //    //    brm.IsTickLoad = tick;
        //    //}
        //}


        #endregion

        #region Сервисные функции

        /// <summary>
        /// Время для периодической проверки вермени с сервером
        /// </summary>

        public double _difTime = 0;
        public override DateTime GetServerTime()
        {
            TimeServer = DateTime.UtcNow.AddSeconds(_difTime);
            return TimeServer;
        }


        #endregion

        #region Не требуют изменения в реализации.

        public override void SaveSettings()
        {
            SaveTerminalInfo();
        }

        public override void LoadSettings()
        {
            LoadTerminalInfo();
        }
        //public override void SendTransaction(BaseRobotModel brm, InnerClassForTranslateParam inn)
        //{


        //    if (brm.TypeRobot != ConfigTermins.TypeRobotTest)
        //        CommonLogic.Execute(brm, inn, this);
        //    else
        //    {
        //        var tmpCommon = new CommonLogic(this);
        //        tmpCommon.Execute(brm, inn, this);
        //    }

        //}

        public override List<ITerminalInfo> GetTerminalSetting()
        {
            return ListTerminalInfo;
        }
        /// <summary>
        /// Удалить настроенное сеодинение
        /// </summary>
        /// <param name="setiing"></param>
        public override void DelTerminalSetting(ITerminalInfo setiing)
        {
            ListTerminalInfo.Remove(setiing as TerminalInfo);
        }
        /// <summary>
        /// Добавить настроенное соединение
        /// </summary>
        /// <param name="setiing"></param>
        public override void AddTerminalSetting(ITerminalInfo setiing)
        {
            ListTerminalInfo.Add(setiing as TerminalInfo);

        }

        ///// <summary>
        ///// Проверка частичного исполнения заявок
        ///// </summary>
        ///// <param name="brmodel"></param>
        //public override void ExecutePartialOrders(BaseRobotModel brmodel)
        //{
        //    CommonLogic.ExecutePartialOrders(brmodel, this);
        //}

        //public override void SetDealTradeList(BaseRobotModel brm)
        //{
        //    CommonLogic.SetDeals(brm);
        //}

        ///// <summary>
        ///// Проверка статусов исполнения заявок
        ///// </summary>
        ///// <param name="brm"></param>
        //public override void GetStatusAndBalanceOrder(BaseRobotModel brm)
        //{
        //    CommonLogic.GetStatusAndBalance(brm, this);
        //    CommonLogic.GetStatusAndBalanceStopsNew(brm, this);


        //    if (brm.TypeRobot != ConfigTermins.TypeRobotSovetnic)
        //        CommonLogic.ExecutePartialOrders(brm, this);

        //    CommonLogic.WorkwithTradeList(brm, this);
        //    CommonLogic.TrallingStopFromScriptExecute(brm, this);
        //}

        //public override void GetPositionFromTradeList(BaseRobotModel brm)
        //{
        //    CommonLogic.GetPositionFromTradeList(brm);
        //}


        ///// <summary>
        ///// Получить денежные средства по счету
        ///// </summary>
        ///// <param name="clientCode"></param>
        ///// <param name="account"></param>
        ///// <returns></returns>
        //public override IMoneyShares GetImoneyItem(string clientCode, string account)
        //{

        //    foreach (var item in BaseTable.LimitMoneySharesList)
        //    {
        //        if (item.ClientCode == clientCode)
        //            return item;
        //    }
        //    return null;

        //}


        ///// <summary>
        ///// Получить денежные средства по счету фьючерсам
        ///// </summary>
        ///// <param name="clientCode"></param>
        ///// <returns></returns>
        //public override IMoneyFutures GetFutureImoneyItem(string clientCode)
        //{
        //    foreach (var item in BaseTable.LimitMoneyFuturesList)
        //    {
        //        if (item.Account == clientCode)
        //            return item;
        //    }

        //    return null;
        //}

        //public override IPositionFutures GetPositionFuturesItem(string clientCode, string symbol)
        //{
        //    foreach (var item in BaseTable.PositionFuturesList)
        //    {
        //        if (item.ClientCode == clientCode && item.Symbol == symbol)
        //            return item;
        //    }

        //    return null;
        //}

        //public override IPositionShares GetPositionSharesItem(string clientCode, string symbol)
        //{
        //    foreach (var item in BaseTable.PositionSharesList)
        //    {
        //        if (item.ClientCode == clientCode && item.Symbol == symbol)
        //            return item;
        //    }

        //    return null;
        //}

        //public override string GetAccount(string clientCode)
        //{
        //    return clientCode;
        //}



        #region Table

        /// <summary>
        /// Возвращаем таблицу текущих параметров
        /// </summary>
        /// <returns></returns>
        public override List<ISecurity> GetTableCurrentParam()
        {
            return BaseTable.CurrentParamModelList;
        }

        public override List<ITick> GetTableAllTradesOrTick()
        {
            return BaseTable.TickList;
        }

        public override List<IOrders> GetTableOrders()
        {
            return BaseTable.OrdersList;
        }

        public override List<IStop> GetTableStopOrders()
        {
            return BaseTable.StopsList;
        }

        public override List<IDeals> GetTableDeals()
        {
            return BaseTable.DealsList;
        }

        public override List<ISecurity> GetTableOptions()
        {
            return null;
        }

        public override List<IMoneyShares> GetTableLimitMoneyShares()
        {
            return BaseTable.LimitMoneySharesList;

        }

        public override List<IPositionShares> GetTablePositionShares()
        {
            return BaseTable.PositionSharesList;

        }

        public override List<IMoneyFutures> GetTableLimitMoneyFurures()
        {
            return BaseTable.LimitMoneyFuturesList;
        }

        public override List<IPositionFutures> GetTablePositionFutures()
        {
            return BaseTable.PositionFuturesList;

        }



        #endregion


        #region GetOrderAndStop
        /// <summary>
        /// Get List All orders 
        /// </summary>
        /// <returns></returns>
        public override List<IOrders> GetOrders()
        {
            return BaseTable.OrdersList;
        }

        ///// <summary>
        ///// Get List orders 
        ///// </summary>
        ///// <param name="clientCode">счет клиента</param>
        ///// <returns></returns>
        //public override List<IOrders> GetOrders(string clientCode)
        //{
        //    List<IOrders> list = new List<IOrders>();
        //    foreach (var item in BaseTable.OrdersList)
        //    {
        //        if (item.ClientCode == clientCode)
        //            list.Add(item);
        //    }

        //    return list;
        //}
        /// <summary>
        /// Get List orders 
        /// </summary>
        /// <param name="clientCode">счет клиента</param>
        /// <param name="instrument">инструмента</param>
        /// <returns></returns>
        public override List<IOrders> GetOrders(string clientCode, string instrument)
        {
            List<IOrders> list = new List<IOrders>();
            foreach (var item in BaseTable.OrdersList)
            {
                if (item.ClientCode == clientCode && item.Symbol == instrument)
                    list.Add(item);
            }

            return list;
        }
        ///// <summary>
        ///// Get List orders 
        ///// </summary>
        ///// <param name="clientCode">счет клиента</param>
        ///// <param name="instrument">инструмента</param>
        ///// <param name="type">статус ордеров</param>
        ///// <returns></returns>
        //public override List<IOrders> GetOrders(string clientCode, string instrument, string type)
        //{
        //    List<IOrders> list = new List<IOrders>();
        //    foreach (var item in BaseTable.OrdersList)
        //    {
        //        if (item.ClientCode == clientCode && item.Symbol == instrument && item.Status == type)
        //            list.Add(item);
        //    }

        //    return list;
        //}

        /// <summary>
        /// Get List All  stop-orders
        /// </summary>
        /// <returns></returns>
        public override IList<IStop> GetStops()
        {
            IList<IStop> list = new List<IStop>();
            foreach (var item in BaseTable.StopsList)
            {
                list.Add(item);
            }

            return list;
        }
        ///// <summary>
        ///// Get List stop-orders
        ///// </summary>
        ///// <param name="clientCode">код клиента</param>
        ///// <returns></returns>
        //public override IList<IStop> GetStops(string clientCode)
        //{
        //    IList<IStop> list = new List<IStop>();
        //    foreach (var item in BaseTable.StopsList)
        //    {
        //        if (item.ClientCode == clientCode)
        //            list.Add(item);
        //    }

        //    return list;
        //}
        /// <summary>
        /// Get List stop-orders
        /// </summary>
        /// <param name="clientCode">код клиента</param>
        /// <param name="account">счет клиента</param>
        /// <returns></returns>
        public override IList<IStop> GetStops(string clientCode, string account)
        {
            IList<IStop> list = new List<IStop>();
            foreach (var item in BaseTable.StopsList)
            {
                if (item.ClientCode == clientCode && item.Account == account)
                    list.Add(item);
            }

            return list;
        }
        ///// <summary>
        ///// Get List stop-orders
        ///// </summary>
        ///// <param name="clientCode">счет клиента</param>
        ///// <param name="account">инструмента</param>
        ///// <param name="type">статус ордеров</param>
        ///// <returns></returns>
        //public override IList<IStop> GetStops(string clientCode, string account, string type)
        //{
        //    IList<IStop> list = new List<IStop>();
        //    foreach (var item in BaseTable.StopsList)
        //    {
        //        if (item.ClientCode == clientCode && item.Symbol == account && item.Status == type)
        //            list.Add(item);
        //    }

        //    return list;
        //}

        //public override IList<IDeals> GetDeals()
        //{
        //    return BaseTable.DealsList;
        //}

        //public override IList<IDeals> GetDeals(string clientCode)
        //{
        //    IList<IDeals> list = new List<IDeals>();
        //    foreach (var item in BaseTable.DealsList)
        //    {
        //        if (item.ClientCode == clientCode)
        //            list.Add(item);
        //    }

        //    return list;
        //}

        //public override IList<IDeals> GetDeals(string clientCode, string instrument)
        //{
        //    IList<IDeals> list = new List<IDeals>();
        //    foreach (var item in BaseTable.DealsList)
        //    {
        //        if (item.ClientCode == clientCode && item.Symbol == instrument)
        //            list.Add(item);
        //    }

        //    return list;
        //}

        /// <summary>
        /// Вернуть весь список инструментов
        /// </summary>
        /// <returns></returns>
        public List<ISecurity> GetAllSecurity()
        {
            return BaseTable.AllCurrentParamModelList;
        }


        public override ISecurity GetSecurity(string instrument, string classCode)
        {
            for (int i = 0; i < BaseTable.CurrentParamModelList.Count; i++)
            {
                if (BaseTable.CurrentParamModelList[i].Seccode == instrument &&
                    BaseTable.CurrentParamModelList[i].ClassCode == classCode)
                    return BaseTable.CurrentParamModelList[i];
            }

            for (int i = 0; i < BaseTable.AllCurrentParamModelList.Count; i++)
            {
                if (BaseTable.AllCurrentParamModelList[i].Seccode == instrument &&
                    BaseTable.AllCurrentParamModelList[i].ClassCode == classCode)
                    return BaseTable.AllCurrentParamModelList[i];
            }

            return null;
        }
        #endregion

        #endregion


        #region Методы поставщика данных


        public override void GetSymbolsSupplyData()
        {
        }
        /// <summary>
        /// Получаем информацию о доступных таймфреймах, которые можно загрузить
        /// </summary>
        /// <returns></returns>
        public override List<string> GetTimeFramesSupplyData()
        {
            return new List<string>();
        }

        /// <summary>
        /// Получаем историю по запрашиваемому инструменту
        /// </summary>
        /// <param name="timeFrame">таймфрейм</param>
        /// <param name="dateStart">дата начала загрузки</param>
        /// <param name="dateEnd">дата окончания загрузки</param>
        /// <param name="deep">гулбина</param>
        /// <returns></returns>
        public override Candles GetHistoryData(AvalibleInstrumentsModel symbol, string timeFrame, DateTime dateStart, DateTime dateEnd, int deep, ForeignInstrumentModel instr = null)
        {
            // Необходимо доработать!!!

            return HuobiWrapper.GetHistoryKlinesAsync(symbol, timeFrame, dateStart, dateEnd).GetAwaiter().GetResult();
        }

        #endregion

        #region Стакан
        List<Glass> _glasses = new List<Glass>();

        int count = 0;
        public void UpdateGlass(Glass glass)
        {
            count += 1;
            glass.Deep = 10;
            if (glass.QuotationsBuy.Count > 12)
                glass.Deep = 20;
            if (glass.QuotationsBuy.Count > 42)
                glass.Deep = 50;
            bool add = true;
            for (int i = 0; i < _glasses.Count; i++)
            {
                var item = _glasses[i];
                if (glass.Symbol == item.Symbol)
                {
                    string bids = "";
                    string asks = "";
                    add = false;
                    for (int j = 0; j < item.Deep; j++)
                    {
                        if (j < glass.QuotationsBuy.Count)
                        {

                            (item.QuotationsBuy[j] as GlassQuotation).BuyQty = glass.QuotationsBuy[j].BuyQty;
                            (item.QuotationsBuy[j] as GlassQuotation).Price = glass.QuotationsBuy[j].Price;
                        }
                        else
                        {
                            (item.QuotationsBuy[j] as GlassQuotation).BuyQty = 0;
                            (item.QuotationsBuy[j] as GlassQuotation).Price = 0;
                        }
                        if (j < glass.QuotationsSell.Count)
                        {
                            (item.QuotationsSell[j] as GlassQuotation).SellQty = glass.QuotationsSell[j].SellQty;
                            (item.QuotationsSell[j] as GlassQuotation).Price = glass.QuotationsSell[j].Price;
                        }
                        else
                        {
                            (item.QuotationsSell[j] as GlassQuotation).SellQty = 0;
                            (item.QuotationsSell[j] as GlassQuotation).Price = 0;
                        }
                        bids += item.QuotationsBuy[j].BuyQty + ";" + item.QuotationsBuy[j].Price + "|";
                        asks += item.QuotationsSell[j].SellQty + ";" + item.QuotationsSell[j].Price + "|";
                    }
                    //GlassSaveModel model = new GlassSaveModel();
                    //model.DateTime = TimeServer;
                    //model.Asks = asks;
                    //model.Bids = bids;

                    //item.GlassSaveM.Add(model);
                }
            }

            if (add)
            {
                for (int i = glass.QuotationsBuy.Count; i < glass.Deep; i++)
                {
                    glass.QuotationsBuy.Add(new GlassQuotation());
                }
                for (int i = glass.QuotationsSell.Count; i < glass.Deep; i++)
                {
                    glass.QuotationsSell.Add(new GlassQuotation());
                }

                _glasses.Add(glass);
                dataStorage.AddGlass(MainDispatcher, glass);
            }
        }

        #endregion
    }
}

