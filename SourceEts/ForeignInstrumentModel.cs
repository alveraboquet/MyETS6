using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using CommonDataContract;

namespace SourceEts
{
    [Serializable]
    public class ForeignInstrumentModel : ViewModelBase, ISerializable
    {
        /// <summary>
        /// Прерывание ожидание обновлений
        /// </summary>
        [XmlIgnore]
        public bool UpdateBreak { get; set; }

        /// <summary>
        /// загружены данные по тикам или нет
        /// </summary>
        [XmlIgnore]
        public bool IsTickRequest { get; set; }

        /// <summary>
        /// Номер запроса, чтоб понять при выводе ошибки
        /// </summary>
        [XmlIgnore]
        public int Id { get; set; }
        /// <summary>
        /// проверяем чтоб не было дублей сообщений об ошибке
        /// </summary>
        [XmlIgnore]
        public int IdCheck { get; set; }

        #region private

        private string _symbol;
        private string _symbolIqFeed;
        private string _exchange;
        private string _primaryExchange;
        private string _currency;
        private string _typeInstrument;
        private DateTime _dateSelectExpire;
        private string _dateExpire;
        private double _minStep;
        private bool _check;
        //private double _priceQuotation;
        //private string _priceQuotationString;
        private double _margin;
        private double _strike;
        private string _multiplier;

        #endregion

        #region public


        /// <summary>
        /// Hack: In the class InteractiveBrokersBrokerage.cs line 1727 insert the following code in the method CreateContract:
        /// if (contract.Symbol.Equals("SI")) { contract.Multiplier = "5000"; }
        /// https://github.com/QuantConnect/Lean/issues/2924
        /// </summary>
        public string Multiplier
        {
            get { return _multiplier; }
            set
            {
                if (_multiplier != value)
                {
                    _multiplier = value;
                    RaisePropertyChanged("Multiplier");
                }
            }
        }


        ///// <summary>
        ///// Котировка ввиде строки, пишится в случае необходимости, например по ZB
        ///// </summary>
        //public string PriceQuotationString
        //{
        //    get { return _priceQuotationString; }
        //    set
        //    {
        //        if (_priceQuotationString != value)
        //        {
        //            _priceQuotationString = value;
        //            RaisePropertyChanged("PriceQuotationString");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Котировка, пишится в случае необходимости, например по ZB
        ///// </summary>
        //public double PriceQuotation
        //{
        //    get { return _priceQuotation; }
        //    set
        //    {
        //        if (Math.Abs(_priceQuotation - value) > 0.000001)
        //        {
        //            _priceQuotation = value;
        //            RaisePropertyChanged("PriceQuotation");
        //        }
        //    }
        //}

        /// <summary>
        /// Гарантийное обеспечение
        /// </summary>
        public double Margin
        {
            get { return _margin; }
            set
            {
                if (Math.Abs(_margin - value) > 0.000001)
                {
                    _margin = value;
                    RaisePropertyChanged("Margin");
                }
            }
        }

        /// <summary>
        /// страйк опциона
        /// </summary>
        public double Strike
        {
            get { return _strike; }
            set
            {
                if (Math.Abs(_strike - value) > 0.000001)
                {
                    _strike = value;
                    RaisePropertyChanged("Strike");
                }
            }
        }

        /// <summary>
        /// Минимальный шаг цены для фьючерсов, т.к. в IB TWS нет поля показывающего минимальный шаг цены
        /// </summary>
        public double MinStep
        {
            get { return _minStep; }
            set
            {
                if (Math.Abs(_minStep - value) > 0.000001)
                {
                    _minStep = value;
                    RaisePropertyChanged("MinStep");
                }
            }
        }


        /// <summary>
        /// Дата экспирации
        /// </summary>
        public string DateExpire
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
        /// Дата экспирации в нужном формате для запросов
        /// </summary>
        public DateTime DateSelectExpire
        {
            get
            {
                if (_dateSelectExpire.Year == 1)
                    return DateTime.Now;

                return _dateSelectExpire;
            }
            set
            {
                if (_dateSelectExpire != value)
                {
                    _dateSelectExpire = value;
                    RaisePropertyChanged("DateSelectExpire");
                }
            }
        }


        /// <summary>
        /// . 
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
        /// Символ для IqFeed
        /// </summary>
        public string SymbolIqFeed
        {
            get { return _symbolIqFeed; }
            set
            {
                if (_symbolIqFeed != value)
                {
                    _symbolIqFeed = value;
                    RaisePropertyChanged("SymbolIqFeed");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public string PrimaryExchange
        {
            get { return _primaryExchange; }
            set
            {
                if (_primaryExchange != value)
                {
                    _primaryExchange = value;
                    RaisePropertyChanged("PrimaryExchange");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public string Exchange
        {
            get { return _exchange; }
            set
            {
                if (_exchange != value)
                {
                    _exchange = value;
                    RaisePropertyChanged("Exchange");
                }
            }
        }

        /// <summary>
        /// . 
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
        /// . 
        /// </summary>
        public string TypeInstrument
        {
            get { return _typeInstrument; }
            set
            {
                if (_typeInstrument != value)
                {
                    _typeInstrument = value;
                    RaisePropertyChanged("TypeInstrument");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public bool Check
        {
            get { return _check; }
            set
            {
                if (_check != value)
                {
                    _check = value;
                    RaisePropertyChanged("Check");
                }
            }
        }



        #endregion

        public ForeignInstrumentModel()
        {

        }

        public ForeignInstrumentModel(SerializationInfo info, StreamingContext context)
        {
            Multiplier = info.GetString("Multiplier");
            Symbol = info.GetString("Symbol");
            SymbolIqFeed = info.GetString("SymbolIqFeed");
            Exchange = info.GetString("Exchange");
            PrimaryExchange = info.GetString("PrimaryExchange");
            Currency = info.GetString("Currency");
            DateExpire = info.GetString("DateExpire");
            DateSelectExpire = info.GetDateTime("DateSelectExpire");
            TypeInstrument = info.GetString("TypeInstrument");
            Check = info.GetBoolean("Check");
            MinStep = info.GetDouble("MinStep");
            Margin = info.GetDouble("Margin");
            Strike = info.GetDouble("Strike");
            //PriceQuotation = info.GetDouble("PriceQuotation");
            //PriceQuotationString = info.GetString("PriceQuotationString");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Multiplier", Multiplier);
            info.AddValue("Symbol", Symbol);
            info.AddValue("SymbolIqFeed", SymbolIqFeed);
            info.AddValue("Exchange", Exchange);
            info.AddValue("PrimaryExchange", PrimaryExchange);
            info.AddValue("Currency", Currency);
            info.AddValue("DateExpire", DateExpire);
            info.AddValue("DateSelectExpire", DateSelectExpire);
            info.AddValue("TypeInstrument", TypeInstrument);
            info.AddValue("Check", Check);
            info.AddValue("MinStep", MinStep);
            info.AddValue("Margin", Margin);
            info.AddValue("Strike", Strike);
            //info.AddValue("PriceQuotation", PriceQuotation);
            //info.AddValue("PriceQuotationString", PriceQuotationString);
        }
    }
}
