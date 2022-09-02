using CommonDataContract.AbstractDataTypes;
using System;
using System.Collections.Generic;

namespace CommonDataContract.ReactData
{
    /// <summary>
    /// Заявки

    /// </summary>
    public class Orders : ViewModelBase, IOrders
    {
        #region Свойства для крипто бирж

        /// <summary>
        /// Время отправки заявки на снятие заявки
        /// </summary>
        public DateTime CancelOrderTime { get; set; }
        /// <summary>
        /// Была ли попытка снятия ордера
        /// </summary>
        public bool TryCancelOrder { get; set; }

        public List<Deal> Deals = new List<Deal>();

        #endregion

        /// <summary>
        /// Тип ордера, рыночный или лимитный
        /// </summary>
        public string TypeOrder { get; set; }
        /// <summary>
        /// Заявка на другом сервере
        /// </summary>
        public bool AnotherServer { get; set; }
        /// <summary>
        /// Время отправки транзакции на снятие
        /// </summary>
        public DateTime DateTimeKill { get; set; }

        #region Private

        private string _number;
        private int _secid;
        private string _classCode;
        private string _symbol;
        private string _account;
        private string _clientCode;
        private string _status;
        private string _operation;
        private DateTime _time;
        private DateTime _accepttime;
        private string _brokerref;
        private double _balance;
        private double _price;
        private double _quantity;
        private double _hidden;
        private DateTime _withdrawtime;
        private string _result;
        private string _comment;
        private string _id;

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
        /// идентификатор транзакции сервера Transaq
        /// </summary>
        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        /// <summary>
        /// биржевой номер заявки
        /// </summary>
        public string Number
        {
            get { return _number; }
            set
            {
                if (_number != value)
                {
                    _number = value;
                    RaisePropertyChanged("Number");
                }
            }
        }

        /// <summary>
        /// идентификатор бумаги
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
        /// код инструмента
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
        /// идентификатор клиента
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
        /// Код клиента
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
        /// статус заявки
        /// Значение status cancelled означает, что заявка находится в процессе снятия.
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }
        /// <summary>
        /// покупка (B) / продажа (S)
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
        /// время регистрации заявки биржей
        /// </summary>
        public DateTime Time
        {
            get { return _time; }
            set
            {
                if (_time != value)
                {
                    _time = value;
                    RaisePropertyChanged("Time");
                }
            }
        }

        /// <summary>
        /// время регистрации заявки сервером Transaq (только для условных заявок)
        /// </summary>
        public DateTime Accepttime
        {
            get { return _accepttime; }
            set
            {
                if (_accepttime != value)
                {
                    _accepttime = value;
                    RaisePropertyChanged("Accepttime");
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
        /// Неудовлетворенный остаток объема заявки в лотах (контрактах)
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
        /// Цена
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
        /// Количество
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
        /// Скрытое количество в лотах
        /// </summary>
        public double Hidden
        {
            get { return _hidden; }
            set
            {
                if (Math.Abs(_hidden - value) > CfgSourceEts.MyEpsilon)
                {
                    _hidden = value;
                    RaisePropertyChanged("Hidden");
                }
            }
        }

        /// <summary>
        /// Время снятия заявки, 0 для активных
        /// Нулевое значение элемента означает, что заявка находится в процессе снятия.
        /// Ненулевое значение withdrawtime при значении status cancelled означает, что заявка снята.
        /// </summary>
        public DateTime Withdrawtime
        {
            get { return _withdrawtime; }
            set
            {
                if (_withdrawtime != value)
                {
                    _withdrawtime = value;
                    RaisePropertyChanged("Withdrawtime");
                }
            }
        }

        /// <summary>
        /// сообщение биржи в случае отказа выставить заявку
        /// </summary>
        public string Result
        {
            get { return _result; }
            set
            {
                if (_result != value)
                {
                    _result = value;
                    RaisePropertyChanged("Result");
                }
            }
        }


        #endregion


    }
}
