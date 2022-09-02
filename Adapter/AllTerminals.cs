using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SourceEts.Table;
using SourceEts;
using System.Windows.Threading;
using SourceEts.Terminals;
using SourceEts.Table.TableClass;
using CommonDataContract.AbstractDataTypes;
using CommonDataContract.ReactData;

namespace Adapter
{
    /// <summary>
    /// Создание единого адаптера
    /// </summary>
    public class AllTerminals
    {
        private static readonly DataStorage dataStorage = DataStorage.Instance;

        private List<AbstractTerminal> _cryptoAdapters;


        /// <summary>
        /// Список крипто адаптеров 
        /// </summary>
        public List<AbstractTerminal> CryptoAdapters
        {
            get { return _cryptoAdapters ?? (_cryptoAdapters = new List<AbstractTerminal>()); }
            set
            {
                if (_cryptoAdapters != value)
                {
                    CryptoAdapters = value;
                }
            }
        }

        public Dispatcher MainDispatcher { get; set; }

        #region События


        #region Вывод сообщения

        public delegate void AddMessage(string something, string title = "");

        public event AddMessage OnAddMessage;

        public void Raise_AddMessage(string something, string title = "")
        {
            if (null != OnAddMessage)
                OnAddMessage(something);
        }
        #endregion

        #region Передача информации в самый главный лог
        public delegate void SomethingHappened(string something);

        public event SomethingHappened OnSomething;

        public void Raise_OnSomething(string something)
        {
            if (null != OnSomething)
                OnSomething(something);
        }
        #endregion

        #endregion

        #region Свойства

        /// <summary>
        /// Список инструментов для отобржаения в сканерах
        /// </summary>
        public List<AllInstrForTrwModel> AllInstrTrw = new List<AllInstrForTrwModel>();



        private bool _isPushConnect;

        /// <summary>
        /// Котировки по выводимымм стаканам
        /// </summary>
        public bool IsPushConnect
        {
            get { return _isPushConnect; }
            set
            {
                if (_isPushConnect != value)
                {
                    _isPushConnect = value;
                    RaisePropertyChanged("IsPushConnect");
                }
            }
        }

        #region Таблицы

        #region private

        //private ObservableCollection<ISecurity> _currentList;
        //private ObservableCollection<IPositionFutures> _positionFuturesList;
        //private ObservableCollection<IPositionShares> _positionSharesList;
        //private ObservableCollection<IMoneyShares> _limitMoneySharesList;
        //private ObservableCollection<IMoneyFutures> _limitMoneyFuturesList;

        //private ObservableCollection<IOrders> _ordersList;
        //private ObservableCollection<IStop> _stopOrderList;
        //private ObservableCollection<IDeals> _dealList;

        //private List<IOrders> _orders;
        //private List<IStop> _stopOrders;
        //private List<IDeals> _deals;
        //private List<Glass> _glassess;


        //private int _indexRowAllTrades;

        #endregion


        #region public

        ///// <summary>
        ///// Котировки по выводимымм стаканам
        ///// </summary>
        //public List<Glass> Glassess
        //{
        //    get { return _glassess ?? (_glassess = new List<Glass>()); }
        //    set
        //    {
        //        if (_glassess != value)
        //        {
        //            _glassess = value;
        //            RaisePropertyChanged("Glassess");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Таблица стоп-заявок
        ///// </summary>
        //public ObservableCollection<IStop> StopOrderList
        //{
        //    get { return _stopOrderList ?? (_stopOrderList = new ObservableCollection<IStop>()); }
        //    set
        //    {
        //        if (_stopOrderList != value)
        //        {
        //            _stopOrderList = value;
        //            RaisePropertyChanged("StopOrderList");
        //        }
        //    }
        //}


        ///// <summary>
        ///// Таблица сделок
        ///// </summary>
        //public ObservableCollection<IDeals> DealList
        //{
        //    get { return _dealList ?? (_dealList = new ObservableCollection<IDeals>()); }
        //    set
        //    {
        //        if (_dealList != value)
        //        {
        //            _dealList = value;
        //            RaisePropertyChanged("DealList");
        //        }
        //    }
        //}


        ///// <summary>
        ///// Таблица позиций по фьючерсам и опционам
        ///// </summary>
        //public ObservableCollection<IPositionFutures> PositionFuturesList
        //{
        //    get { return _positionFuturesList ?? (_positionFuturesList = new ObservableCollection<IPositionFutures>()); }
        //    set
        //    {
        //        if (_positionFuturesList != value)
        //        {
        //            _positionFuturesList = value;
        //            RaisePropertyChanged("PositionFuturesList");
        //        }
        //    }
        //}


        ///// <summary>
        ///// Таблица позиций по акциям
        ///// </summary>
        //public ObservableCollection<IPositionShares> PositionSharesList
        //{
        //    get { return _positionSharesList ?? (_positionSharesList = new ObservableCollection<IPositionShares>()); }
        //    set
        //    {
        //        if (_positionSharesList != value)
        //        {
        //            _positionSharesList = value;
        //            RaisePropertyChanged("PositionSharesList");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Таблица лимитов денежных средств по акциям. 
        ///// </summary>
        //public ObservableCollection<IMoneyShares> LimitMoneySharesList
        //{
        //    get
        //    {
        //        if (_limitMoneySharesList == null)
        //            _limitMoneySharesList = new ObservableCollection<IMoneyShares>();
        //        return _limitMoneySharesList;
        //    }
        //    set
        //    {
        //        if (_limitMoneySharesList != value)
        //        {
        //            _limitMoneySharesList = value;
        //            RaisePropertyChanged("LimitMoneySharesList");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Таблица лимитов по фьючерсам
        ///// </summary>
        //public ObservableCollection<IMoneyFutures> LimitMoneyFuturesList
        //{
        //    get
        //    {
        //        if (_limitMoneyFuturesList == null)
        //            _limitMoneyFuturesList = new ObservableCollection<IMoneyFutures>();
        //        return _limitMoneyFuturesList;
        //    }
        //    set
        //    {
        //        if (_limitMoneyFuturesList != value)
        //        {
        //            _limitMoneyFuturesList = value;
        //            RaisePropertyChanged("LimitMoneyFuturesList");
        //        }
        //    }
        //}



        ///// <summary>
        ///// Таблица заявок
        ///// </summary>
        //public List<IOrders> Orders
        //{
        //    get { return _orders ?? (_orders = new List<IOrders>()); }
        //    set
        //    {
        //        _orders = value;
        //    }
        //}


        ///// <summary>
        ///// Таблица заявок
        ///// </summary>
        //public List<IDeals> Deals
        //{
        //    get { return _deals ?? (_deals = new List<IDeals>()); }
        //    set
        //    {
        //        _deals = value;
        //    }
        //}


        ///// <summary>
        ///// Таблица заявок
        ///// </summary>
        //public List<IStop> StopOrders
        //{
        //    get { return _stopOrders ?? (_stopOrders = new List<IStop>()); }
        //    set
        //    {
        //        _stopOrders = value;
        //    }
        //}


        ///// <summary>
        ///// Таблица заявок
        ///// </summary>
        //public ObservableCollection<IOrders> OrdersList
        //{
        //    get { return _ordersList ?? (_ordersList = new ObservableCollection<IOrders>()); }
        //    set
        //    {
        //        if (_ordersList != value)
        //        {
        //            _ordersList = value;
        //            RaisePropertyChanged("OrdersList");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Таблица текущих параметров
        ///// </summary>
        //public ObservableCollection<ISecurity> CurrentList
        //{
        //    get { return _currentList ?? (_currentList = new ObservableCollection<ISecurity>()); }
        //    set
        //    {
        //        if (_currentList != value)
        //        {
        //            _currentList = value;
        //            RaisePropertyChanged("CurrentList");
        //        }
        //    }
        //}
        #endregion


        public List<IMoneyShares> Portfel = new List<IMoneyShares>();

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion


      
        public List<ITerminalInfo> AllTerminalInfos = new List<ITerminalInfo>();
        /// <summary>
        /// Получаем все используемые терминалы
        /// </summary>
        /// <returns></returns>
        public List<ITerminalInfo> GetAllTerminals()
        {
            AllTerminalInfos.Clear();

            foreach (var abstractTerminal in CryptoAdapters)
            {
                foreach (var item in abstractTerminal.GetTerminalSetting())
                {
                    if (item.IsUse)
                        AllTerminalInfos.Add(item);
                }
            }

            return AllTerminalInfos;
        }

        /// <summary>
        /// Соединение с терминалом
        /// </summary>
        public void Connect()
        {

            foreach (var abstractTerminal in CryptoAdapters)
            {
                bool connect = false;
                for (int i = 0; i < abstractTerminal.ListTerminalInfo.Count; i++)
                {
                    if (abstractTerminal.ListTerminalInfo[i].IsUse)
                        connect = true;
                }
                if (connect)
                    abstractTerminal.Connect();
            }
        }


        public void Disconnect()
        {
  
            foreach (var abstractTerminal in CryptoAdapters)
            {
                abstractTerminal.Disconnect();
            }
        }


        public void SaveSetting()
        {

            foreach (var abstractTerminal in CryptoAdapters)
            {
                abstractTerminal.SaveSettings();
            }
        }

        public DateTime TimeServer { get; set; }

        public DateTime GetServerTime()
        {
    

            foreach (var abstractTerminal in CryptoAdapters)
            {
                if (abstractTerminal.IsConnect)
                    TimeServer = abstractTerminal.GetServerTime();
            }

            return TimeServer;
        }

        /// <summary>
        /// Get instance Security 
        /// </summary>
        /// <returns></returns>
        public ISecurity GetSecurity(string instrument)
        {
            for (int i = 0; i < dataStorage.CurrentParamModelList.Count; i++)
            {
                if (dataStorage.CurrentParamModelList[i].Seccode == instrument)
                    return dataStorage.CurrentParamModelList[i];

            }

            return new Securities();
        }


        /// <summary>
        /// Get instance Security 
        /// </summary>
        /// <returns></returns>
        public ISecurity GetSecurity(string instrument, string classCode)
        {
            for (int i = 0; i < dataStorage.CurrentParamModelList.Count; i++)
            {
                if (dataStorage.CurrentParamModelList[i].Seccode == instrument && dataStorage.CurrentParamModelList[i].ClassCode == classCode)
                    return dataStorage.CurrentParamModelList[i];
            }

            return new Securities();
        }


        /// <summary>
        /// GetAllInstruments
        /// </summary>
        /// <returns></returns>
        public IList<string> GetSeccodeList()
        {
            IList<string> list = new List<string>();
            foreach (var item in dataStorage.CurrentParamModelList)
            {
                list.Add(item.Seccode);
            }

            return list;
        }

        /// <summary>
        /// GetAllInstruments on ClassCode
        /// </summary>
        /// <returns></returns>
        public IList<string> GetSeccodeList(string classCode)
        {
            IList<string> list = new List<string>();
            foreach (var item in dataStorage.CurrentParamModelList)
            {
                if (item.ClassCode == classCode)
                    list.Add(item.Seccode);
            }

            return list;
        }

        /// <summary>
        /// Get all markets
        /// </summary>
        /// <returns></returns>
        public IList<string> GetClasscodeList()
        {
            IList<string> list = new List<string>();
            foreach (var item in dataStorage.CurrentParamModelList)
            {
                if (!list.Contains(item.ClassCode))
                    list.Add(item.ClassCode);
            }

            return list;
        }

        /// <summary>
        /// Get market on symbol
        /// </summary>
        /// <returns></returns>
        public string GetClasscode(string seccode)
        {
            foreach (var item in dataStorage.CurrentParamModelList)
            {
                if (item.Seccode == seccode)
                    return item.ClassCode;
            }


            return "";
        }

        public List<AccountsPair> GetPairClienCodesAccount()
        {
            List<AccountsPair> list = new List<AccountsPair>();

           

            foreach (var userAdapter in CryptoAdapters)
            {
                if (IsPushConnect)
                    for (int i = userAdapter.ListTerminalInfo.Count - 1; i >= 0; i--)
                    {
                        if (userAdapter.ListTerminalInfo[i].IsUse)
                        {
                            for (int j = userAdapter.ListTerminalInfo[i].AccountsPairs.Count - 1; j >= 0; j--)
                            {
                                list.Add(userAdapter.ListTerminalInfo[i].AccountsPairs[j]);
                            }
                        }
                    }
            }


            return list;

        }
      
        public IPositionFutures GetPositionFuturesItem(string clientCode, string symbol)
        {
            for (int i = dataStorage.PositionFuturesList.Count - 1; i >= 0; i--)
            {
                var item = dataStorage.PositionFuturesList[i];
                if (item.ClientCode == clientCode && item.Symbol == symbol)
                    return item;
            }

            return null;
        }

        public IPositionShares GetPositionSharesItem(string clientCode, string account, string symbol)
        {
            for (int i = dataStorage.PositionSharesList.Count - 1; i >= 0; i--)
            {
                var item = dataStorage.PositionSharesList[i];
                if (item.Account == account && item.ClientCode == clientCode && item.Symbol == symbol)
                    return item;
            }

            return null;

        }

        public AbstractTerminal GetTerminal(string account, string clientCode)
        {
            string accClient = account + clientCode;

            foreach (var abstractTerminal in CryptoAdapters)
            {
                for (int i = 0; i < abstractTerminal.ListTerminalInfo.Count; i++)
                {
                    for (int j = 0; j < abstractTerminal.ListTerminalInfo[i].AccountsPairs.Count; j++)
                    {
                        if (abstractTerminal.ListTerminalInfo[i].AccountsPairs[j].Account +
                            abstractTerminal.ListTerminalInfo[i].AccountsPairs[j].ClientCode == accClient)
                        {
                            return abstractTerminal;
                        }
                    }
                }
            }

            return null;
        }

    }
}
