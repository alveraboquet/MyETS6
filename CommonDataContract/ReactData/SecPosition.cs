using System;
using CommonDataContract.AbstractDataTypes;

namespace CommonDataContract.ReactData
{
    public class SecPosition : ViewModelBase, IPositionShares
    {
        /// <summary>
        /// Время последнего обновления данных, используется только для криптовалют
        /// </summary>
        public DateTime LastTimeUpdate { get; set; }
        /// <summary>
        /// Время последнего пополнения
        /// </summary>
        public DateTime LastTimeDeposit { get; set; }
        /// <summary>
        /// Сумма последнего пополнения
        /// </summary>
        public double Deposit { get; set; }

        #region private
        private int _secid;
        private int _market;
        private string _symbol;
        private string _register;
        private string _clientCode;
        private string _account;
        private string _nameSymbol;
        private double _enterOst;
        private double _saldomin;
        private double _bought;
        private double _sold;
        private double _balance;
        private double _ordbuy;
        private double _ordsell;
        private string _addInfo;
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
        public int Market
        {
            get { return _market; }
            set
            {
                if (_market != value)
                {
                    _market = value;
                    RaisePropertyChanged("MarketList");
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
        /// Регистр учета
        /// </summary>
        public string Register
        {
            get { return _register; }
            set
            {
                if (_register != value)
                {
                    _register = value;
                    RaisePropertyChanged("Register");
                }
            }
        }

        /// <summary>
        /// Для транзак коннектор не представляет надобности
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
        /// Наименование бумаги
        /// </summary>
        public string NameSymbol
        {
            get { return _nameSymbol; }
            set
            {
                if (_nameSymbol != value)
                {
                    _nameSymbol = value;
                    RaisePropertyChanged("NameSymbol");
                }
            }
        }
        /// <summary>
        /// Справочная информация
        /// </summary>
        public string AddInfo
        {
            get { return _addInfo; }
            set
            {
                if (_addInfo != value)
                {
                    _addInfo = value;
                    RaisePropertyChanged("AddInfo");
                }
            }
        }

        /// <summary>
        /// Входящий остаток
        /// </summary>
        public double EnterOst
        {
            get { return _enterOst; }
            set
            {
                if (Math.Abs(_enterOst - value) > CfgSourceEts.MyEpsilon)
                {
                    _enterOst = value;
                    RaisePropertyChanged("EnterOst");
                }
            }
        }

        /// <summary>
        /// Неснижаемый остаток
        /// </summary>
        public double Saldomin
        {
            get { return _saldomin; }
            set
            {
                if (Math.Abs(_saldomin - value) > CfgSourceEts.MyEpsilon)
                {
                    _saldomin = value;
                    RaisePropertyChanged("Saldomin");
                }
            }
        }

        /// <summary>
        /// Куплено
        /// </summary>
        public double Bought
        {
            get { return _bought; }
            set
            {
                if (Math.Abs(_bought - value) > CfgSourceEts.MyEpsilon)
                {
                    _bought = value;
                    RaisePropertyChanged("Bought");
                }
            }
        }

        /// <summary>
        /// Продано
        /// </summary>
        public double Sold
        {
            get { return _sold; }
            set
            {
                if (Math.Abs(_sold - value) > CfgSourceEts.MyEpsilon)
                {
                    _sold = value;
                    RaisePropertyChanged("Sold");
                }
            }
        }

        /// <summary>
        /// Текущее сальдо
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
        /// В заявках на покупку
        /// </summary>
        public double Ordbuy
        {
            get { return _ordbuy; }
            set
            {
                if (Math.Abs(_ordbuy - value) > CfgSourceEts.MyEpsilon)
                {
                    _ordbuy = value;
                    RaisePropertyChanged("Ordbuy");
                }
            }
        }

        /// <summary>
        /// В заявках на продажу
        /// </summary>
        public double Ordsell
        {
            get { return _ordsell; }
            set
            {
                if (Math.Abs(_ordsell - value) > CfgSourceEts.MyEpsilon)
                {
                    _ordsell = value;
                    RaisePropertyChanged("Ordsell");
                }
            }
        }

        #endregion

       
    }
}
