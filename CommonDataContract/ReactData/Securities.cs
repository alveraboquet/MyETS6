using CommonDataContract.AbstractDataTypes;
using System;
using System.Collections.Generic;

namespace CommonDataContract.ReactData
{
    public class Securities : ViewModelBase, ISecurity
    {

        #region Private

        private DateTime _timeLastChange;
        private double _bid;//Лучшая котировка на покупку
        private double _offer; //Лучшая котировка на продажу
        private string _shortName;
        private string _seccode; //Код инструмента
        private string _classCode; //Код класса
        private int _accuracy; //Количество десятичных знаков в цене
        private double _minStep; //Шаг цены
        private double _maxPrice; //Максимальная цена (только для FORTS)
        private double _minPrice; //Минимальная цена (только для FORTS)
        private double _lastPrice; //Цена последней сделки
        private double _closePrice; //Цена закрытия
        private double _goSell; //ГО продавца
        private double _goBuy; //ГО покупателя
        private double _pointCost; //Стоимость пункта цены
        private string _tradingStatus;//Состояние торговой сессии по инструменту
        private string _status;//Статус «торговые операции разрешены/запрещены»
        private double _lotSize; //размер лота 
        private Double _lastChangePercent;//изменения в процентах к цене открытия предыдущего дня
        private double _openPrice; //Цена первой сделки
        private bool _isTrade; //Можно ли по инструменту выставлять и снимать заявки
        private double _theorPrice;
        private string _baseActive;
        private string _typeOption;
        private double _strike;
        private double _volatility;
        private DateTime _dateExpire;
        private double _priceQuotation;
        private Dictionary<string, object> _dicValue;
        private double _prevLastPrice;

        private double _minAmount;
        private double _maxAmount;
        private int _lotSizeAccuracy;


        #endregion


        #region Public

        private bool _isCrypto;

        /// <summary>
        /// КриптоИнструмент
        /// </summary>
        public bool IsCrypto
        {
            get { return _isCrypto; }
            set
            {
                if (_isCrypto != value)
                {
                    _isCrypto = value;
                    RaisePropertyChanged("IsCrypto");
                }
            }
        }

        /// <summary>
        /// Минимальный объем
        /// </summary>
        public double MinAmount
        {
            get { return _minAmount; }
            set
            {
                if (Math.Abs(_minAmount - value) > Double.Epsilon)
                {
                    _minAmount = value;
                    RaisePropertyChanged("MinAmount");
                }
            }
        }

        /// <summary>
        /// Максимальный объем
        /// </summary>
        public double MaxAmount
        {
            get { return _maxAmount; }
            set
            {
                if (Math.Abs(_maxAmount - value) > Double.Epsilon)
                {
                    _maxAmount = value;
                    RaisePropertyChanged("MaxAmount");
                }
            }
        }

        /// <summary>
        /// теоретическая цена опциона
        /// </summary>
        public double TheorPrice
        {
            get { return _theorPrice; }
            set
            {
                if (Math.Abs(_theorPrice - value) > Double.Epsilon)
                {
                    _theorPrice = value;
                    RaisePropertyChanged("TheorPrice");
                }
            }
        }

        /// <summary>
        /// Базавый актив
        /// </summary>
        public string BaseActive
        {
            get { return _baseActive; }
            set
            {
                if (_baseActive != value)
                {
                    _baseActive = value;
                    RaisePropertyChanged("BaseActive");
                }
            }
        }

        /// <summary>
        /// тип опциона
        /// </summary>
        public string TypeOption
        {
            get { return _typeOption; }
            set
            {
                if (_typeOption != value)
                {
                    _typeOption = value;
                    RaisePropertyChanged("TypeOption");
                }
            }
        }

        /// <summary>
        /// страйк по опциону
        /// </summary>
        public double Strike
        {
            get { return _strike; }
            set
            {
                if (Math.Abs(_strike - value) > Double.Epsilon)
                {
                    _strike = value;
                    RaisePropertyChanged("Strike");
                }
            }
        }

        /// <summary>
        /// тип опциона
        /// </summary>
        public DateTime DateExpire
        {
            get { return _dateExpire; }
            set
            {
                if (_dateExpire != value)
                {
                    _dateExpire = value;
                    RaisePropertyChanged("DateExpire");
                }
            }
        }



        /// <summary>
        /// Применяется при нестандартных шкалах цены
        /// </summary>
        public double PriceQuotation
        {
            get { return _priceQuotation; }
            set
            {
                if (Math.Abs(_priceQuotation - value) > Double.Epsilon)
                {
                    _priceQuotation = value;
                    RaisePropertyChanged("PriceQuotation");
                }
            }
        }
        /// <summary>
        /// Можно ли по инструменту выставлять и снимать заявки
        /// </summary>
        public bool IsTrade
        {
            get { return _isTrade; }
            set
            {
                if (_isTrade != value)
                {
                    _isTrade = value;
                    RaisePropertyChanged("IsTrade");
                }
            }
        }

        /// <summary>
        /// изменение в процентах от цены закрытия предыдущей сессии
        /// </summary>
        public Double LastChangePercent
        {
            get { return _lastChangePercent; }
            set
            {
                if (Math.Abs(_lastChangePercent - value) > CfgSourceEts.MyEpsilon)
                {
                    _lastChangePercent = value;
                    RaisePropertyChanged("LastChangePercent");
                }
            }
        }

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


        /// <summary>
        /// Предпоследняя цена сделки
        /// </summary>
        public Double PrevLastPrice
        {
            get { return _prevLastPrice; }
            set
            {
                if (Math.Abs(_prevLastPrice - value) > CfgSourceEts.MyEpsilon)
                {
                    _prevLastPrice = value;
                    RaisePropertyChanged("PrevLastPrice");
                }
            }
        }



        /// <summary>
        /// Наименование бумаги. 
        /// </summary>
        public string ShortName
        {
            get { return _shortName; }
            set
            {
                if (_shortName != value)
                {
                    _shortName = value;
                    RaisePropertyChanged("ShortName");
                }
            }
        }

        /// <summary>
        /// Цена первой сделки. 
        /// </summary>
        public double OpenPrice
        {
            get { return _openPrice; }
            set
            {
                if (Math.Abs(_openPrice - value) > CfgSourceEts.MyEpsilon)
                {
                    _openPrice = value;
                    RaisePropertyChanged("OpenPrice");
                }
            }
        }

        /// <summary>
        /// Время последнего обновления
        /// </summary>
        public DateTime TimeLastChange
        {
            get { return _timeLastChange; }
            set
            {
                if (_timeLastChange != value)
                {
                    _timeLastChange = value;
                    RaisePropertyChanged("TimeLastChange");
                }
            }
        }

        /// <summary>
        /// Лучшая котировка на покупку. 
        /// </summary>
        public double Bid
        {
            get { return _bid; }
            set
            {
                if (Math.Abs(_bid - value) > CfgSourceEts.MyEpsilon)
                {
                    _bid = value;
                    RaisePropertyChanged("Bid");
                }
            }
        }

        /// <summary>
        /// Лучшая котировка на продажу. 
        /// </summary>
        public double Offer
        {
            get { return _offer; }
            set
            {
                if (Math.Abs(_offer - value) > CfgSourceEts.MyEpsilon)
                {
                    _offer = value;
                    RaisePropertyChanged("Offer");
                }
            }
        }



        /// <summary>
        /// Код инструмента. 
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
        /// Код класса. 
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
        /// Количество десятичных знаков в цене. 
        /// </summary>
        public int Accuracy
        {
            get { return _accuracy; }
            set
            {
                if (_accuracy != value)
                {
                    _accuracy = value;
                    RaisePropertyChanged("Accuracy");
                }
            }
        }

        /// <summary>
        /// Шаг цены. 
        /// </summary>
        public double MinStep
        {
            get { return _minStep; }
            set
            {
                if (Math.Abs(_minStep - value) > CfgSourceEts.MyEpsilon)
                {
                    _minStep = value;
                    RaisePropertyChanged("MinStep");
                }
            }
        }

        /// <summary>
        /// Максимальная цена (только для FORTS). 
        /// </summary>
        public double MaxPrice
        {
            get { return _maxPrice; }
            set
            {
                if (Math.Abs(_maxPrice - value) > CfgSourceEts.MyEpsilon)
                {
                    _maxPrice = value;
                    RaisePropertyChanged("MaxPrice");
                }
            }
        }

        /// <summary>
        /// Минимальная цена (только для FORTS). 
        /// </summary>
        public double MinPrice
        {
            get { return _minPrice; }
            set
            {
                if (Math.Abs(_minPrice - value) > CfgSourceEts.MyEpsilon)
                {
                    _minPrice = value;
                    RaisePropertyChanged("MinPrice");
                }
            }
        }

        /// <summary>
        /// Цена последней сделки. 
        /// </summary>
        public double LastPrice
        {
            get { return _lastPrice; }
            set
            {
                if (Math.Abs(_lastPrice - value) > CfgSourceEts.MyEpsilon)
                {
                    _lastPrice = value;
                    RaisePropertyChanged("LastPrice");
                }
            }
        }

        /// <summary>
        /// Цена закрытия. 
        /// </summary>
        public double ClosePrice
        {
            get { return _closePrice; }
            set
            {
                if (Math.Abs(_closePrice - value) > CfgSourceEts.MyEpsilon)
                {
                    _closePrice = value;
                    RaisePropertyChanged("ClosePrice");
                }
            }
        }

        /// <summary>
        /// ГО продавца. 
        /// </summary>
        public double GoSell
        {
            get { return _goSell; }
            set
            {
                if (Math.Abs(_goSell - value) > CfgSourceEts.MyEpsilon)
                {
                    _goSell = value;
                    RaisePropertyChanged("GoSell");
                }
            }
        }

        /// <summary>
        /// ГО покупателя. 
        /// </summary>
        public double GoBuy
        {
            get { return _goBuy; }
            set
            {
                if (Math.Abs(_goBuy - value) > CfgSourceEts.MyEpsilon)
                {
                    _goBuy = value;
                    RaisePropertyChanged("GoBuy");
                }
            }
        }

        /// <summary>
        /// Стоимость пункта цены. 
        /// </summary>
        public double PointCost
        {
            get { return _pointCost; }
            set
            {
                if (Math.Abs(_pointCost - value) > CfgSourceEts.MyEpsilon)
                {
                    _pointCost = value;
                    RaisePropertyChanged("PointCost");
                }
            }
        }

        /// <summary>
        /// Состояние торговой сессии по инструменту. 
        /// </summary>
        public string TradingStatus
        {
            get { return _tradingStatus; }
            set
            {
                if (_tradingStatus != value)
                {
                    _tradingStatus = value;
                    RaisePropertyChanged("TradingStatus");
                }
            }
        }

        /// <summary>
        /// Статус «торговые операции разрешены/запрещены». 
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
        /// размер лота. 
        /// </summary>
        public double LotSize
        {
            get { return _lotSize; }
            set
            {
                if (Math.Abs(_lotSize - value) > CfgSourceEts.MyEpsilon)
                {
                    _lotSize = value;
                    RaisePropertyChanged("LotSize");
                }
            }
        }

        /// <summary>
        /// точность расчет объема, количество знаков после запятой
        /// </summary>
        public int LotSizeAccuracy
        {
            get { return _lotSizeAccuracy; }
            set
            {
                if (_lotSizeAccuracy != value)
                {
                    _lotSizeAccuracy = value;
                    RaisePropertyChanged("LotSizeAccuracy");
                }
            }
        }

        /// <summary>
        /// Волатильность. 
        /// </summary>
        public double Volatility
        {
            get { return _volatility; }
            set
            {
                if (Math.Abs(_volatility - value) > CfgSourceEts.MyEpsilon)
                {
                    _volatility = value;
                    RaisePropertyChanged("Volatility");
                }
            }
        }

        public string Isin { get; set; }
        public string LastNumberTrade { get; set; }


        #endregion

        #region Добавления от 25.09.2018

        #region private

        private int _numBids;
        private int _numOffers;
        private int _numTrades;
        private double _volatToday;

        #region Облигации

        private DateTime _nextCoupon;
        private DateTime _matDate;
        private int _couponPeriod;
        private double _yield;
        private double _duration;
        private double _accuedint;
        private double _secFaceValue;
        private double _couponValue;
        private string _secType;
        private string _quotesBasis;
        private string _typePriceBond;
        private int _daysToMatDateBond;

        #endregion 

        #endregion

        #region public

        /// <summary>
        /// Заявок на покупку
        /// </summary>
        public int NumBids
        {
            get { return _numBids; }
            set
            {
                if (_numBids != value)
                {
                    _numBids = value;
                    RaisePropertyChanged("NumBids");
                }
            }
        }

        /// <summary>
        /// Заявок на продажу
        /// </summary>
        public int NumOffers
        {
            get { return _numOffers; }
            set
            {
                if (_numOffers != value)
                {
                    _numOffers = value;
                    RaisePropertyChanged("NumOffers");
                }
            }
        }

        /// <summary>
        /// Кол-во сделок
        /// </summary>
        public int NumTrades
        {
            get { return _numTrades; }
            set
            {
                if (_numTrades != value)
                {
                    _numTrades = value;
                    RaisePropertyChanged("NumTrades");
                }
            }
        }

        /// <summary>
        /// Оборот в деньгах
        /// </summary>
        public double VolatToday
        {
            get { return _volatToday; }
            set
            {
                if (Math.Abs(_volatToday - value) > CfgSourceEts.MyEpsilon)
                {
                    _volatToday = value;
                    RaisePropertyChanged("VolatToday");
                }
            }
        }

        /// <summary>
        /// Дата выплаты следующего купона
        /// </summary>
        public DateTime NextCoupon
        {
            get { return _nextCoupon; }
            set
            {
                if (_nextCoupon != value)
                {
                    _nextCoupon = value;
                    RaisePropertyChanged("NextCoupon");
                }
            }
        }

        /// <summary>
        /// Дата погашения
        /// </summary>
        public DateTime MatDate
        {
            get { return _matDate; }
            set
            {
                if (_matDate != value)
                {
                    _matDate = value;
                    RaisePropertyChanged("MatDate");
                }
            }
        }

        /// <summary>
        /// Длительность купона
        /// </summary>
        public int CouponPeriod
        {
            get { return _couponPeriod; }
            set
            {
                if (_couponPeriod != value)
                {
                    _couponPeriod = value;
                    RaisePropertyChanged("CouponPeriod");
                }
            }
        }

        /// <summary>
        /// Доходность
        /// </summary>
        public double Yield
        {
            get { return _yield; }
            set
            {
                if (Math.Abs(_yield - value) > CfgSourceEts.MyEpsilon)
                {
                    _yield = value;
                    RaisePropertyChanged("Yield");
                }
            }
        }

        /// <summary>
        /// Дюрация
        /// </summary>
        public double Duration
        {
            get { return _duration; }
            set
            {
                if (Math.Abs(_duration - value) > CfgSourceEts.MyEpsilon)
                {
                    _duration = value;
                    RaisePropertyChanged("Duration");
                }
            }
        }

        /// <summary>
        /// НКД 
        /// </summary>
        public double Accuedint
        {
            get { return _accuedint; }
            set
            {
                if (Math.Abs(_accuedint - value) > CfgSourceEts.MyEpsilon)
                {
                    _accuedint = value;
                    RaisePropertyChanged("Accuedint");
                }
            }
        }

        /// <summary>
        /// Номанал облигации
        /// </summary>
        public double SecFaceValue
        {
            get { return _secFaceValue; }
            set
            {
                if (Math.Abs(_secFaceValue - value) > CfgSourceEts.MyEpsilon)
                {
                    _secFaceValue = value;
                    RaisePropertyChanged("SecFaceValue");
                }
            }
        }

        /// <summary>
        /// Размер купона
        /// </summary>
        public double CouponValue
        {
            get { return _couponValue; }
            set
            {
                if (Math.Abs(_couponValue - value) > CfgSourceEts.MyEpsilon)
                {
                    _couponValue = value;
                    RaisePropertyChanged("CouponValue");
                }
            }
        }

        /// <summary>
        /// Тип инструмента
        /// </summary>
        public string SecType
        {
            get { return _secType; }
            set
            {
                if (_secType != value)
                {
                    _secType = value;
                    RaisePropertyChanged("SecType");
                }
            }
        }

        /// <summary>
        /// Тип цены 
        /// </summary>
        public string QuotesBasis
        {
            get { return _quotesBasis; }
            set
            {
                if (_quotesBasis != value)
                {
                    _quotesBasis = value;
                    RaisePropertyChanged("QuotesBasis");
                }
            }
        }

        /// <summary>
        /// Тип цены облиагации
        /// </summary>
        public string TypePriceBond
        {
            get { return _typePriceBond; }
            set
            {
                if (_typePriceBond != value)
                {
                    _typePriceBond = value;
                    RaisePropertyChanged("TypePriceBond");
                }
            }
        }

        /// <summary>
        /// Дней до погашения облигации 
        /// </summary>
        public int DaysToMatDateBond
        {
            get { return _daysToMatDateBond; }
            set
            {
                if (_daysToMatDateBond != value)
                {
                    _daysToMatDateBond = value;
                    RaisePropertyChanged("DaysToMatDateBond");
                }
            }
        }


        #endregion

        #endregion

        private double _minNational;

        /// <summary>
        /// Минимальный сумма сделки
        /// </summary>
        public double MinNational
        {
            get { return _minNational; }
            set
            {
                if (Math.Abs(_minNational - value) > Double.Epsilon)
                {
                    _minNational = value;
                    RaisePropertyChanged("MinNational");
                }
            }
        }
    }
}
