using CommonDataContract.AbstractDataTypes;
using System;

namespace CommonDataContract.ReactData
{
    /// <summary>
    /// Сделки
    /// Передается автоматически после установки соединения (для уже
    /// совершенных сделок), а так же по мере появления новых сделок.
    /// </summary>
    public class Deal : ViewModelBase, IDeals
    {
        #region Private

        private int _secid;
        private string _numberTrade;//_tradeno;
        private string _order;//_orderno; 
        private string _classCode;
        private string _symbol;
        private string _сlientCode;
        private string _account;
        private string _operation;//_buysell;
        private string _comment;//_buysell;
        private DateTime _dateTrade;
        private string _brokerref;
        private double _volume;
        private double _comission;
        private double _price;
        private double _quantity;//_quantity;
        private long _currentpos;

        #endregion

        #region Public

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    RaisePropertyChanged("Comment");
                }
            }
        }


        /// <summary>
        /// Счет
        /// </summary>
        public string Account
        {
            get { return _account; }
            set
            {
                if (_account != value)
                {
                    _account = value;
                    RaisePropertyChanged("Account");
                }
            }
        }
        /// <summary>
        /// Идентификатор бумаги
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
        /// Номер сделки на бирже
        /// </summary>
        public string NumberTrade
        {
            get { return _numberTrade; }
            set
            {
                if (_numberTrade != value)
                {
                    _numberTrade = value;
                    RaisePropertyChanged("NumberTrade");
                }
            }
        }

        /// <summary>
        /// Номер заявки на бирже
        /// </summary>
        public string Order
        {
            get { return _order; }
            set
            {
                if (_order != value)
                {
                    _order = value;
                    RaisePropertyChanged("Order");
                }
            }
        }

        /// <summary>
        /// Идентификатор борда
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
        /// Идентификатор клиента
        /// </summary>
        public string ClientCode
        {
            get { return _сlientCode; }
            set
            {
                if (_сlientCode != value)
                {
                    _сlientCode = value;
                    RaisePropertyChanged("ClientCode");
                }
            }
        }

        /// <summary>
        /// B - покупка, S - продажа
        /// </summary>
        public string Operation
        {
            get { return _operation; }
            set
            {
                if (_operation != value)
                {
                    _operation = value;
                    RaisePropertyChanged("Operation");
                }
            }
        }

        /// <summary>
        /// время сделки
        /// </summary>
        public DateTime DateTrade
        {
            get { return _dateTrade; }
            set
            {
                if (_dateTrade != value)
                {
                    _dateTrade = value;
                    RaisePropertyChanged("DateTrade");
                }
            }
        }

        /// <summary>
        /// примечание
        /// </summary>
        public string Brokerref
        {
            get { return _brokerref; }
            set
            {
                if (_brokerref != value)
                {
                    _brokerref = value;
                    RaisePropertyChanged("Brokerref");
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
        /// комиссия
        /// </summary>
        public double Comission
        {
            get { return _comission; }
            set
            {
                if (Math.Abs(_comission - value) > CfgSourceEts.MyEpsilon)
                {
                    _comission = value;
                    RaisePropertyChanged("Comission");
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
        /// количество лотов
        /// </summary>
        public double Quantity
        {
            get { return _quantity; }
            set
            {
                if (Math.Abs(_quantity - value) > CfgSourceEts.MyEpsilon)
                {
                    _quantity = value;
                    RaisePropertyChanged("Quantity");
                }
            }
        }

        /// <summary>
        /// Текущая позиция по инструменту
        /// </summary>
        public long Currentpos
        {
            get { return _currentpos; }
            set
            {
                if (_currentpos != value)
                {
                    _currentpos = value;
                    RaisePropertyChanged("Currentpos");
                }
            }
        }
        #endregion


    }
}
