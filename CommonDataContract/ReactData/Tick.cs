using CommonDataContract.AbstractDataTypes;
using System;

namespace CommonDataContract.ReactData
{
    /// <summary>
    /// Получение тиков
    /// </summary>
    public class Tick : ViewModelBase, ITick
    {
        private string _seccode;
        private long _tradeNum;
        private DateTime _tradeDateTime;
        private string _tradePeriod;
        private string _classCode;
        private double _price;
        private char _period;
        private double _qty;
        private int _secid;
        private double _volume;
        private char _buySell;
        private int _oi;

        #region public

        /// <summary>
        /// открытый интерес
        /// </summary>
        public int Oi
        {
            get { return _oi; }
            set
            {
                if (_oi != value)
                {
                    _oi = value;
                    RaisePropertyChanged("Oi");
                }
            }
        }

        /// <summary>
        /// Период торгов (O - открытие, N - торги, С -закрытие)
        /// </summary>
        public char Period
        {
            get { return _period; }
            set
            {
                if (_period != value)
                {
                    _period = value;
                    RaisePropertyChanged("Period");
                }
            }
        }


        /// <summary>
        /// идентификатор бумаги, уникален в течении одной сессии
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
        /// Код инструмента
        /// </summary>
        public string Seccode
        {
            get { return _seccode; }
            set
            {
                if (_seccode != value)
                {
                    _seccode = value;
                    RaisePropertyChanged("Seccode");
                }
            }
        }

        /// <summary>
        /// Номер сделки
        /// </summary>
        public long TradeNum
        {
            get { return _tradeNum; }
            set
            {
                if (_tradeNum != value)
                {
                    _tradeNum = value;
                    RaisePropertyChanged("TradeNum");
                }
            }
        }


        /// <summary>
        /// Время сделки
        /// </summary>
        public DateTime TradeDateTime
        {
            get { return _tradeDateTime; }
            set
            {
                if (_tradeDateTime != value)
                {
                    _tradeDateTime = value;
                    RaisePropertyChanged("TradeDateTime");
                }
            }
        }

        /// <summary>
        /// торговый период (O - открытие, N - торги, C - закрытие; передается только для ММВБ)
        /// </summary>
        public string TradePeriod
        {
            get { return _tradePeriod; }
            set
            {
                if (_tradePeriod != value)
                {
                    _tradePeriod = value;
                    RaisePropertyChanged("TradePeriod");
                }
            }
        }

        /// <summary>
        /// Код класса
        /// </summary>
        public string ClassCode
        {
            get { return _classCode; }
            set
            {
                if (_classCode != value)
                {
                    _classCode = value;
                    RaisePropertyChanged("ClassCode");
                }
            }
        }

        /// <summary>
        /// цена
        /// </summary>
        public double Price
        {
            get { return _price; }
            set
            {
                if (Math.Abs(_price - value) > CfgSourceEts.MyEpsilon)
                {
                    _price = value;
                    RaisePropertyChanged("Price");
                }
            }
        }

        /// <summary>
        /// количество
        /// </summary>
        public double Qty
        {
            get { return _qty; }
            set
            {
                if (Math.Abs(_qty - value) > CfgSourceEts.MyEpsilon)
                {
                    _qty = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }

        /// <summary>
        /// объем сделки
        /// </summary>
        public double Volume
        {
            get { return _volume; }
            set
            {
                if (Math.Abs(_volume - value) > CfgSourceEts.MyEpsilon)
                {
                    _volume = value;
                    RaisePropertyChanged("Volume");
                }
            }
        }


        /// <summary>
        /// . 
        /// </summary>
        public char BuySell
        {
            get { return _buySell; }
            set
            {
                if (_buySell != value)
                {
                    _buySell = value;
                    RaisePropertyChanged("BuySell");
                }
            }
        }


        #endregion
    }
}
