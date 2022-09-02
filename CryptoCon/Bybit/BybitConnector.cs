using Adapter;
using Adapter.Config;
using Adapter.Model;
using Bybit.Net.Enums;
using CommonDataContract;
using CommonDataContract.AbstractDataTypes;
using CommonDataContract.ReactData;
using CryptoCon.Binance;
using CryptoCon.Binance.Enums;
using CryptoCon.Binance.Models;
using CryptoExchange.Net.Objects;
using SourceEts;
using SourceEts.Models.TimeFrameTransformModel;
using SourceEts.Table;
using SourceEts.Terminals;
using SourceEts.UserConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using OrderType = Bybit.Net.Enums.OrderType;

namespace CryptoCon.Bybit
{
    public class BybitConnector : AbstractTerminal
    {
        private static readonly DataStorage dataStorage = DataStorage.Instance;
        private BybitWrapper BybitWrapper { get; set; }



        private BackgroundWorker woker;
        private DispatcherTimer _dsTimer = new DispatcherTimer();
        DateTime _lastDateUpdate = new DateTime();
        private int _lastCount = 0;
        private string _instr = "";
        private int _counterTransaction = 0;


        public BybitConnector()
        {
            NameUserAdapter = "Bybit";

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

            #endregion
        }

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
                BaseMon.Raise_OnSomething("Ошибка при проверке информации по таблице сделок " + BybitWrapper.NameExchange + ":" + ex.Message);
            }

            return result;
        }

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
                BaseMon.Raise_OnSomething(BybitWrapper.NameExchange + ". Ошибка при проверке информации по денежным средствам: " + ex.Message);
            }

            return result;
        }

        public override bool AddOrder(List<IOrders> orderTable, IOrders orderItem)
        {
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
                BaseMon.Raise_OnSomething(BybitWrapper.NameExchange + ". Ошибка при проверке информации по таблице заявок: " + ex.Message);
            }

            return result;
        }

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
                BaseMon.Raise_OnSomething(BybitWrapper.NameExchange + ". Ошибка при проверке информации по позиции: " + ex.Message);
            }

            return result;
        }

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

        public override void AddTerminalSetting(ITerminalInfo setiing)
        {
            ListTerminalInfo.Add(setiing as TerminalInfo);
        }

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

                            BybitWrapper = new BybitWrapper(pub, sec, typeMarket, this);


                            
                            if (typeRegim != "Публичный")
                            {
                                _isConnectorPrivate = (item as TerminalInfo).UserConSettings.First(a => a.NameParam == "Режим работы коннектора").ValueString == "Публичный и приватный";
                            }

                            item.AccountsPairs.Add(new AccountsPair
                            {
                                Account = pub,
                                ClientCode = sec,
                                AccountClientCode = BybitWrapper.NameExchange + ": " + item.Name + " (" + itemUserCon.ValueString + ")",
                                TypeClass = BybitWrapper.NameExchange
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
            _dsTimer.Tick += _dsTimer_Tick;


            #endregion



            return true;
        }

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



                    #region Placed Orders

                    //List<Task> orders = new List<Task>();

                    //orders.Add(BinanceWrapper.PlaceOrderAsync("BTCUSDT", OrderSide.Buy, OrderType.Limit, quantity: 0.00046m, price: 23000, timeInForce: TimeInForce.GoodTillCanceled, clientId: "AlinaBTC2"));
                    //orders.Add(BinanceWrapper.PlaceOrderAsync("BTCUSDT", OrderSide.Buy, OrderType.Limit, quantity: 0.00046m, price: 23000, timeInForce: TimeInForce.GoodTillCanceled, clientId: "AlinaBTC3"));
                    //orders.Add(BinanceWrapper.PlaceOrderAsync("BTCUSDT", OrderSide.Buy, OrderType.Limit, quantity: 0.00046m, price: 23000, timeInForce: TimeInForce.GoodTillCanceled, clientId: "AlinaBTC4"));
                    //orders.Add(BinanceWrapper.PlaceOrderAsync("BTCUSDT", OrderSide.Buy, OrderType.Limit, quantity: 0.00046m, price: 23000, timeInForce: TimeInForce.GoodTillCanceled, clientId: "AlinaBTC5"));
                    //orders.Add(BinanceWrapper.PlaceOrderAsync("BTCUSDT", OrderSide.Buy, OrderType.Limit, quantity: 0.00046m, price: 23000, timeInForce: TimeInForce.GoodTillCanceled, clientId: "AlinaBTC6"));

                    //Task.WaitAll(orders.ToArray());

                    #endregion

                    List<Task> tasks = new List<Task>();

                    #region Открытие ордера

                    tasks.Add(BybitWrapper.GetOpenOrdersAsync());

                    #endregion

                    #region Получение всего баланса пользователя в usdt
                    if (_isConnectorPrivate)
                        Task.Run(async () =>
                        {
                            for (int i = 0; i < int.MaxValue; i++)
                            {
                                var balance = new TotalBalance();  //BybitWrapper.GetBalanceByDollars().GetAwaiter().GetResult(); TODO: realize
                                if (balance != null)
                                {
                                    BaseMon.Raise_OnSomething(balance.TotalBalanceUSDT.ToString());
                                }

                                await Task.Delay(5000);
                            }
                        });

                    #endregion


                    #region Подписка на стакан

                    tasks.Add(BybitWrapper.SubscribeToGlass(AvalibleInstruments));

                    #endregion

                    //tasks.Add(BinanceWrapper.GetHistoryOrdesAsync(AvalibleInstruments));

                    #region Получение баланс пользователя по всем инструментам

                    tasks.Add(BybitWrapper.GetBalancesAsync());

                    #endregion

                    #region Подписка на инстументы, выбранные пользователем

                    tasks.Add(BybitWrapper.SubcribeToInstrumentsAsync(AvalibleInstruments));

                    #endregion

                    #region Получение изменение баланса по сокету

                    tasks.Add(BybitWrapper.SubscribeToAccountUpdateAsync());

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
                    BaseMon.Raise_OnSomething("Загрузка данных с " + BybitWrapper.NameExchange + " завершена");
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

        private void SetTicker()
        {
            var tickers = BybitWrapper.GetAllTickersAsync().GetAwaiter().GetResult().ToList();
            if (tickers.Any())
            {
                foreach (var ticker in tickers)
                {
                    AddSortInst(this, ticker, ticker.BaseActive);
                }
            }
            else
            {
                BaseMon.Raise_OnSomething(BybitWrapper.NameExchange + " SetTicker: не удалось получить инструменты");
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

        public void _dsTimer_Tick(Object sender, EventArgs args)
        {

            if (!woker.IsBusy)
                woker.RunWorkerAsync();

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
                BaseMon.Raise_OnSomething(BybitWrapper.NameExchange + ". Ошибка: " + ex.Message);
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

        public override void DelTerminalSetting(ITerminalInfo setiing)
        {
            ListTerminalInfo.Remove(setiing as TerminalInfo);
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

        public override void ExecuteUserConnector(string path)
        {
            
        }

        public override Candles GetHistoryData(AvalibleInstrumentsModel symbol, string timeFrame, DateTime dateStart, DateTime dateEnd, int deep, ForeignInstrumentModel instr = null)
        {
            return BybitWrapper.GetHistoryKlinesAsync(symbol, timeFrame, dateStart, dateEnd).GetAwaiter().GetResult();
        }

        public override List<IOrders> GetOrders()
        {
            return BaseTable.OrdersList;
        }

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

            return default;
        }

        public double _difTime = 0;
        public override DateTime GetServerTime()
        {
            return DateTime.UtcNow.AddSeconds(_difTime);
        }

        public override IList<IStop> GetStops()
        {
            IList<IStop> list = new List<IStop>();
            foreach (var item in BaseTable.StopsList)
            {
                list.Add(item);
            }

            return list;
        }

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

        public override void GetSymbolsSupplyData()
        {
            
        }

        public override List<ITick> GetTableAllTradesOrTick()
        {
            return BaseTable.TickList;
        }

        public override List<ISecurity> GetTableCurrentParam()
        {
            return BaseTable.CurrentParamModelList;
        }

        public override List<IDeals> GetTableDeals()
        {
            return BaseTable.DealsList;
        }

        public override List<IMoneyFutures> GetTableLimitMoneyFurures()
        {
            return BaseTable.LimitMoneyFuturesList;
        }

        public override List<IMoneyShares> GetTableLimitMoneyShares()
        {
            return BaseTable.LimitMoneySharesList;
        }

        public override List<ISecurity> GetTableOptions()
        {
            return default;
        }

        public override List<IOrders> GetTableOrders()
        {
            return BaseTable.OrdersList;
        }

        public override List<IPositionFutures> GetTablePositionFutures()
        {
            return BaseTable.PositionFuturesList;
        }

        public override List<IPositionShares> GetTablePositionShares()
        {
            return BaseTable.PositionSharesList;
        }

        public override List<IStop> GetTableStopOrders()
        {
            return BaseTable.StopsList;
        }

        public override List<ITerminalInfo> GetTerminalSetting()
        {
            return ListTerminalInfo;
        }

        public override List<string> GetTimeFramesSupplyData()
        {
            return new List<string>();
        }

        public override void LoadSettings()
        {
            LoadTerminalInfo();
        }

        public override void SaveSettings()
        {
            SaveTerminalInfo();
        }

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

            if (paramOfTransaction.TypeOrder == CfgSourceEts.TypeRequestForTransactionOrder)
            {
                string generatedClientOrderId = $"Binance-{DateTime.UtcNow.Ticks}-{_counterTransaction}";

                // Если тип ордера лимитный
                if (paramOfTransaction.IsMarketOrder != "M")
                {
                    _ = BybitWrapper.PlaceOrderAsync(paramOfTransaction.Symbol, paramOfTransaction.Operation == 'S' ? OrderSide.Sell : OrderSide.Buy,
                        OrderType.Limit, Convert.ToDecimal(paramOfTransaction.Quantity), Convert.ToDecimal(paramOfTransaction.Price), TimeInForce.GoodTillCanceled, generatedClientOrderId);
                }

                // Если тип ордера рыночный
                if (paramOfTransaction.IsMarketOrder == "M")
                {
                    _ = BybitWrapper.PlaceOrderAsync(paramOfTransaction.Symbol, paramOfTransaction.Operation == 'S' ? OrderSide.Sell : OrderSide.Buy,
                         OrderType.Market, Convert.ToDecimal(paramOfTransaction.Quantity), clientOrderId: generatedClientOrderId);
                }

                _counterTransaction++;

                return generatedClientOrderId;
            }

            if (paramOfTransaction.TypeOrder == CfgSourceEts.TypeRequestForTransactionKillOrder)
            {
                _ = BybitWrapper.CancelOrderAsync(paramOfTransaction.Symbol, Convert.ToInt64(paramOfTransaction.OrderNumberForKill));

                //_data.CancelUserOrder(paramOfTransaction);
            }

            return "0";
        }

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
