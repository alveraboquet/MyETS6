using CommonDataContract.AbstractDataTypes;
using System;

namespace CommonDataContract.ReactData
{
    /// <summary>
    /// Стоп ордеры
    /// </summary>
    public class StopOrdes : ViewModelBase, IStop
    {

        /// <summary>
        /// Признак, что стоп-заявка выставлена на другом сервере
        /// </summary>
        public bool AnotherServer { get; set; }

        /// <summary>
        /// Время отправки транзакции на снятие стоп-заявки
        /// </summary>
        public DateTime DateTimeKill { get; set; }

        public int KillCount { get; set; }

        #region Private

        #region Общие

        private int _transactionid;
        private string _numberOrder;
        private int _secid;
        private string _classCode;
        private string _symbol;
        private string _client;
        private string _operation;
        private string _canceller;

        private string _number;
        private DateTime _validbefore;
        private string _author;
        private DateTime _accepttime;
        private DateTime _withdrawtime;
        private long _linkedorderno;
        private DateTime _expdate;
        private string _status;

        public DateTime Time { get; set; }
        public string TypeStop { get; set; }
        public string Account { get; set; }


        public double Quantity { get; set; }
        public double Balance { get; set; }
        public double FilledQuantity { get; set; }
        public string Comment { get; set; }
        public string Result { get; set; }
        public string Id { get; set; }
        public string ClientCode { get; set; }


        #endregion

        #region StopLoss

        public double StopPrice { get; set; }
        public double Price { get; set; }

        private string _slUseCredit;
        private double _slActivationPrice;
        private DateTime _slGuardTime;
        private string _slBrokerref;
        private double _slQuantity;
        private string _slbymarket;
        private double _slOrderPrice;

        #endregion

        #region TrallingProfit

        public double OtstupMaxMin { get; set; }
        public bool IsPercentOtstupMaxMin { get; set; }
        public double Spread { get; set; }
        public bool IsPercentSpread { get; set; }

        private double _tpActivationPrice;
        private DateTime _tpGuardTime;
        private string _tpBrokerref;
        private double _tpQuantity;
        private double _tpExtremum;
        private double _tpLevel;
        private double _tpCorrection;
        private double _tpGuardSpread;

        #endregion

        #endregion

        #region Public

        #region Общие
        /// <summary>
        /// идентификатор транзакции сервера Transaq
        /// </summary>
        public int Transactionid
        {
            get { return _transactionid; }
            set
            {
                if (_transactionid != value)
                {
                    _transactionid = value;
                    RaisePropertyChanged("Transactionid");
                }
            }
        }

        /// <summary>
        /// номер заявки Биржевой регистрационный номер заявки, выставленной на рынок в результате исполнения cтопа.
        /// </summary>
        public string NumberOrder
        {
            get { return _numberOrder; }
            set
            {
                if (_numberOrder != value)
                {
                    _numberOrder = value;
                    RaisePropertyChanged("NumberOrder");
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
        /// идентификатор борда
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
        public string Client
        {
            get { return _client; }
            set
            {
                if (_client != value)
                {
                    _client = value;
                    RaisePropertyChanged("Client");
                }
            }
        }
        /// <summary>
        /// идентификатор трейдера, который отменил стоп.
        /// Значение status cancelled означает, что заявка находится в процессе снятия.
        /// </summary>
        public string Canceller
        {
            get { return _canceller; }
            set
            {
                if (_canceller != value)
                {
                    _canceller = value;
                    RaisePropertyChanged("Canceller");
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
        /// биржевой регистрационный номер сделки, явившейся основанием для перехода стопа в текущее состояние.
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
        /// до какого момента действительно (см. newcondorder)
        /// </summary>
        public DateTime Validbefore
        {
            get { return _validbefore; }
            set
            {
                if (_validbefore != value)
                {
                    _validbefore = value;
                    RaisePropertyChanged("Validbefore");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Author
        {
            get { return _author; }
            set
            {
                if (_author != value)
                {
                    _author = value;
                    RaisePropertyChanged("Author");
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
        /// Время снятия стоп-заявки
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
        /// Номер связанной заявки
        /// </summary>
        public long Linkedorderno
        {
            get { return _linkedorderno; }
            set
            {
                if (_linkedorderno != value)
                {
                    _linkedorderno = value;
                    RaisePropertyChanged("Linkedorderno");
                }
            }
        }

        /// <summary>
        /// дата экспирации (только для ФОРТС)
        /// </summary>
        public DateTime Expdate
        {
            get { return _expdate; }
            set
            {
                if (_expdate != value)
                {
                    _expdate = value;
                    RaisePropertyChanged("Expdate");
                }
            }
        }


        /// <summary>
        /// статус заявки стоп заявки
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




        #endregion

        #region СтопЛосс
        /// <summary>
        /// 
        /// </summary>
        public string SlUseCredit
        {
            get { return _slUseCredit; }
            set
            {
                if (_slUseCredit != value)
                {
                    _slUseCredit = value;
                    RaisePropertyChanged("SlUseCredit");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public double SlActivationPrice
        {
            get { return _slActivationPrice; }
            set
            {
                if (Math.Abs(_slActivationPrice - value) > CfgSourceEts.MyEpsilon)
                {
                    _slActivationPrice = value;
                    RaisePropertyChanged("SlActivationPrice");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime SlGuardTime
        {
            get { return _slGuardTime; }
            set
            {
                if (_slGuardTime != value)
                {
                    _slGuardTime = value;
                    RaisePropertyChanged("SlGuardTime");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SlBrokerref
        {
            get { return _slBrokerref; }
            set
            {
                if (_slBrokerref != value)
                {
                    _slBrokerref = value;
                    RaisePropertyChanged("SlBrokerref");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public double SlQuantity
        {
            get { return _slQuantity; }
            set
            {
                if (Math.Abs(_slQuantity - value) > CfgSourceEts.MyEpsilon)
                {
                    _slQuantity = value;
                    RaisePropertyChanged("SlQuantity");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Slbymarket
        {
            get { return _slbymarket; }
            set
            {
                if (_slbymarket != value)
                {
                    _slbymarket = value;
                    RaisePropertyChanged("Slbymarket");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public double SlOrderPrice
        {
            get { return _slOrderPrice; }
            set
            {
                if (Math.Abs(_slOrderPrice - value) > CfgSourceEts.MyEpsilon)
                {
                    _slOrderPrice = value;
                    RaisePropertyChanged("SlOrderPrice");
                }
            }
        }

        #endregion

        #region TrallingProfit

        /// <summary>
        /// 
        /// </summary>
        public double TpActivationPrice
        {
            get { return _tpActivationPrice; }
            set
            {
                if (Math.Abs(_tpActivationPrice - value) > CfgSourceEts.MyEpsilon)
                {
                    _tpActivationPrice = value;
                    RaisePropertyChanged("TpActivationPrice");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime TpGuardTime
        {
            get { return _tpGuardTime; }
            set
            {
                if (_tpGuardTime != value)
                {
                    _tpGuardTime = value;
                    RaisePropertyChanged("TpGuardTime");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TpBrokerref
        {
            get { return _tpBrokerref; }
            set
            {
                if (_tpBrokerref != value)
                {
                    _tpBrokerref = value;
                    RaisePropertyChanged("TpBrokerref");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public double TpQuantity
        {
            get { return _tpQuantity; }
            set
            {
                if (Math.Abs(_tpQuantity - value) > CfgSourceEts.MyEpsilon)
                {
                    _tpQuantity = value;
                    RaisePropertyChanged("TpQuantity");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public double TpExtremum
        {
            get { return _tpExtremum; }
            set
            {
                if (Math.Abs(_tpExtremum - value) > CfgSourceEts.MyEpsilon)
                {
                    _tpExtremum = value;
                    RaisePropertyChanged("TpExtremum");
                }
            }
        }
        public double TpLevel
        {
            get { return _tpLevel; }
            set
            {
                if (Math.Abs(_tpLevel - value) > CfgSourceEts.MyEpsilon)
                {
                    _tpLevel = value;
                    RaisePropertyChanged("TpLevel");
                }
            }
        }
        public double TpCorrection
        {
            get { return _tpCorrection; }
            set
            {
                if (Math.Abs(_tpCorrection - value) > CfgSourceEts.MyEpsilon)
                {
                    _tpCorrection = value;
                    RaisePropertyChanged("TpCorrection");
                }
            }
        }
        public double TpGuardSpread
        {
            get { return _tpGuardSpread; }
            set
            {
                if (Math.Abs(_tpGuardSpread - value) > CfgSourceEts.MyEpsilon)
                {
                    _tpGuardSpread = value;
                    RaisePropertyChanged("TpGuardSpread");
                }
            }
        }
        #endregion

        #endregion


    }
}
