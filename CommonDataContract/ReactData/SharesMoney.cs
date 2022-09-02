using CommonDataContract.AbstractDataTypes;
using System;
using System.Collections.Generic;

namespace CommonDataContract.ReactData
{
    /// <summary>
    /// Денежные средства по акциям
    /// </summary>
    public class MoneyPosition : ViewModelBase, IMoneyShares
    {
        #region Private

        private string _clientCode;
        private string _account;
        private List<int> _marketList;
        private string _market;
        private string _group;
        private string _asset;
        private string _limitKind;
        private double _openBalance;
        private double _balance;
        private double _totalBalance;

        private double _bought;
        private double _sold;
        private double _ordbuy;
        private double _ordbuycond;
        private double _comission;
        private string _currency;
        #endregion

        private Dictionary<string, object> _dicValue;

        /// <summary>
        /// Параметры передаваемые дополнительно в теминал по какому-то конкретному терминалу
        /// </summary>
        public Dictionary<string, object> DicValue
        {
            get { return _dicValue ?? (_dicValue = new Dictionary<string, object>()); }
            set
            {
                if (_dicValue != null && _dicValue != value)
                {
                    _dicValue = value;
                }
            }
        }

        #region Public

        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public string Currency
        {
            get { return _currency; }
            set
            {
                if (_currency != value)
                {
                    _currency = value;
                    RaisePropertyChanged("Currency");
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
        /// Счет депо, требуется в основном для квика, т.к. клиентские коды могут быть одинаковыми
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
        /// Внутренний код рынка
        /// </summary>
        public List<int> MarketList
        {
            get { return _marketList; }
            set
            {

                if (_marketList != value)
                {
                    _marketList = value;
                    RaisePropertyChanged("MarketList");
                }
            }
        }

        /// <summary>
        /// Регистр учета (Идентификатор торговой сессии, в которой ведется лимит, например EQTV – Фондовая Московская биржа)
        /// </summary>
        public string Group
        {
            get { return _group; }
            set
            {
                if (_group != value)
                {
                    _group = value;
                    RaisePropertyChanged("Group");
                }
            }
        }

        /// <summary>
        /// Код вида средств
        /// </summary>
        public string Asset
        {
            get { return _asset; }
            set
            {
                if (_asset != value)
                {
                    _asset = value;
                    RaisePropertyChanged("Asset");
                }
            }
        }

        /// <summary>
        /// Наименование вида средств
        /// </summary>
        public string LimitKind
        {
            get { return _limitKind; }
            set
            {
                if (_limitKind != value)
                {
                    _limitKind = value;
                    RaisePropertyChanged("LimitKind");
                }
            }
        }

        /// <summary>
        /// Входящий остаток
        /// </summary>
        public double OpenBalance
        {
            get { return _openBalance; }
            set
            {
                if (Math.Abs(_openBalance - value) > 0.00001)
                {
                    _openBalance = value;
                    RaisePropertyChanged("OpenBalance");
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
                if (_bought != value)
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
                if (_sold != value)
                {
                    _sold = value;
                    RaisePropertyChanged("Sold");
                }
            }
        }


        /// <summary>
        /// Текущее сальдо
        /// </summary>
        public double TotalBalance
        {
            get { return _totalBalance; }
            set
            {
                if (Math.Abs(_totalBalance - value) > 0.000001)
                {
                    _totalBalance = value;
                    RaisePropertyChanged("TotalBalance");
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
                if (Math.Abs(_balance - value) > 0.000001)
                {
                    _balance = value;
                    RaisePropertyChanged("Balance");
                }
            }
        }

        /// <summary>
        /// В заявках на покупку + комиссия
        /// </summary>
        public double Ordbuy
        {
            get { return _ordbuy; }
            set
            {
                if (_ordbuy != value)
                {
                    _ordbuy = value;
                    RaisePropertyChanged("Ordbuy");
                }
            }
        }

        /// <summary>
        /// В условных заявках на покупку
        /// </summary>
        public double Ordbuycond
        {
            get { return _ordbuycond; }
            set
            {
                if (_ordbuycond != value)
                {
                    _ordbuycond = value;
                    RaisePropertyChanged("Ordbuycond");
                }
            }
        }

        /// <summary>
        /// Сумма списанной комиссии
        /// </summary>
        public double Comission
        {
            get { return _comission; }
            set
            {
                if (_comission != value)
                {
                    _comission = value;
                    RaisePropertyChanged("Comission");
                }
            }
        }
        #endregion


    }
}
