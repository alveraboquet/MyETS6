using System;
using System.Collections.Generic;
using CommonDataContract.AbstractDataTypes;

namespace CommonDataContract.ReactData
{
    public class FortsPosition : ViewModelBase, IPositionFutures
    {
        #region Private

        private int _secid;
        private List<int> _marketsList;
        private string _market;
        private string _symbol;
        private string _clientCode;

        private double _enterEmptyPos;
        private double _activeBuy;
        private double _activeSell;

        private double _balance;
        private double _currentLongPos;
        private double _currentShortPos;
        private double _variableMarga;

        private double _optmargin;
        private double _expirationpos;
        private double _usedsellspotlimit;
        private double _sellspotlimit;
        private double _netto;
        private double _kgo;

        #endregion
        #region Public

        /// <summary>
        /// Код инструмента
        /// </summary>
        public int Secid
        {
            get { return _secid; }
            set
            {
                if (_secid != value)
                {
                    _secid = value;
                    RaisePropertyChanged("Secid");
                }
            }
        }

        /// <summary>
        /// Внутренний код рынка
        /// </summary>
        public List<int> MarketsList
        {
            get { return _marketsList; }
            set
            {
                if (_marketsList != value)
                {
                    _marketsList = value;
                    RaisePropertyChanged("MarketsList");
                }
            }
        }
        /// <summary>
        /// Рынок
        /// </summary>
        public string Market
        {
            get { return _market; }
            set
            {
                if (_market != value)
                {
                    _market = value;
                    RaisePropertyChanged("Market");
                }
            }
        }
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public string ClientCode
        {
            get { return _clientCode; }
            set
            {
                if (_clientCode != value)
                {
                    _clientCode = value;
                    RaisePropertyChanged("ClientCode");
                }
            }
        }

        /// <summary>
        /// Код инструмента
        /// </summary>
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                if (_symbol != value)
                {
                    _symbol = value;
                    RaisePropertyChanged("Symbol");
                }
            }
        }

        /// <summary>
        /// Входящая позиция по инструменту
        /// </summary>
        public double EnterEmptyPos
        {
            get { return _enterEmptyPos; }
            set
            {
                if (Math.Abs(_enterEmptyPos - value) > CfgSourceEts.MyEpsilon)
                {
                    _enterEmptyPos = value;
                    RaisePropertyChanged("EnterEmptyPos");
                }
            }
        }

        /// <summary>
        /// В заявках на покупку
        /// </summary>
        public double ActiveBuy
        {
            get { return _activeBuy; }
            set
            {
                if (Math.Abs(_activeBuy - value) > CfgSourceEts.MyEpsilon)
                {
                    _activeBuy = value;
                    RaisePropertyChanged("ActiveBuy");
                }
            }
        }

        /// <summary>
        /// В заявках на продажу
        /// </summary>
        public double ActiveSell
        {
            get { return _activeSell; }
            set
            {
                if (Math.Abs(_activeSell - value) > CfgSourceEts.MyEpsilon)
                {
                    _activeSell = value;
                    RaisePropertyChanged("ActiveSell");
                }
            }
        }

        /// <summary>
        /// Текущая позиция по инструменту
        /// </summary>
        public double Balance
        {
            get { return _balance; }
            set
            {
                if (Math.Abs(_balance - value) > CfgSourceEts.MyEpsilon)
                {
                    _balance = value;
                    RaisePropertyChanged("Balance");
                }
            }
        }

        /// <summary>
        /// Куплено
        /// </summary>
        public double CurrentLongPos
        {
            get { return _currentLongPos; }
            set
            {
                if (Math.Abs(_currentLongPos - value) > CfgSourceEts.MyEpsilon)
                {
                    _currentLongPos = value;
                    RaisePropertyChanged("CurrentLongPos");
                }
            }
        }

        /// <summary>
        /// Продано
        /// </summary>
        public double CurrentShortPos
        {
            get { return _currentShortPos; }
            set
            {
                if (Math.Abs(_currentShortPos - value) > CfgSourceEts.MyEpsilon)
                {
                    _currentShortPos = value;
                    RaisePropertyChanged("CurrentShortPos");
                }
            }
        }

        /// <summary>
        /// Маржа для маржируемых опционов
        /// </summary>
        public double Optmargin
        {
            get { return _optmargin; }
            set
            {
                if (Math.Abs(_optmargin - value) > CfgSourceEts.MyEpsilon)
                {
                    _optmargin = value;
                    RaisePropertyChanged("Optmargin");
                }
            }
        }

        /// <summary>
        /// Вариационная маржа
        /// </summary>
        public double VariableMarga
        {
            get { return _variableMarga; }
            set
            {
                if (Math.Abs(_variableMarga - value) > CfgSourceEts.MyEpsilon)
                {
                    _variableMarga = value;
                    RaisePropertyChanged("VariableMarga");
                }
            }
        }

        /// <summary>
        /// Опционов в заявках на исполнение
        /// </summary>
        public double Expirationpos
        {
            get { return _expirationpos; }
            set
            {
                if (Math.Abs(_expirationpos - value) > CfgSourceEts.MyEpsilon)
                {
                    _expirationpos = value;
                    RaisePropertyChanged("Expirationpos");
                }
            }
        }

        /// <summary>
        /// Объем использованого спот-лимита на продажу
        /// </summary>
        public double Usedsellspotlimit
        {
            get { return _usedsellspotlimit; }
            set
            {
                if (Math.Abs(_usedsellspotlimit - value) > CfgSourceEts.MyEpsilon)
                {
                    _usedsellspotlimit = value;
                    RaisePropertyChanged("Usedsellspotlimit");
                }
            }
        }

        /// <summary>
        /// текущий спот-лимит на продажу, установленный Брокером
        /// </summary>
        public double Sellspotlimit
        {
            get { return _sellspotlimit; }
            set
            {
                if (Math.Abs(_sellspotlimit - value) > CfgSourceEts.MyEpsilon)
                {
                    _sellspotlimit = value;
                    RaisePropertyChanged("Sellspotlimit");
                }
            }
        }

        /// <summary>
        /// нетто-позиция по всем инструментам данного спота
        /// </summary>
        public double Netto
        {
            get { return _netto; }
            set
            {
                if (Math.Abs(_netto - value) > CfgSourceEts.MyEpsilon)
                {
                    _netto = value;
                    RaisePropertyChanged("Netto");
                }
            }
        }

        /// <summary>
        /// коэффициент ГО для спота
        /// </summary>
        public double Kgo
        {
            get { return _kgo; }
            set
            {
                if (Math.Abs(_kgo - value) > CfgSourceEts.MyEpsilon)
                {
                    _kgo = value;
                    RaisePropertyChanged("Kgo");
                }
            }
        }


        #endregion
    }
}
