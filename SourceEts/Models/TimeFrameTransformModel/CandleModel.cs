using CommonDataContract;
using SourceEts.Table.CandleVol;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SourceEts.Models.TimeFrameTransformModel
{
    /// <summary>
    /// Свечи/тики
    /// </summary>
    [Serializable]
    public class CandleModel : ViewModelBase, ISerializable, ICandle
    {
        #region свечи

        private DateTime _tradeDateTime;
        private double _high;
        private double _low;
        private double _open;
        private double _close;
        private bool _isOperBuy;
        private double _volume;//объем
        private double _volBuy;//объем
        private double _volSell;//объем
        private int _oi;//открытый интерес
        private long _numberTick;//номер сделки из таблицы всех сделок. Нужно только для реального и корректного обновления данных в режиме реального времени
        private int _numberCandle;//номер свечи, используется только при компрессии
        private int _numberCandleDecompres;//номер свечи в большей серии, при декомпрессии
        private ClasterCandleModel _claster;
        
        #endregion

        #region public


        /// <summary>
        /// Используется только для тиков. Если операция buy то значение true, иначе продажа
        /// </summary>
        [XmlIgnore]
        public bool IsOperBuy
        {
            get { return _isOperBuy; }
            set
            {
                if (_isOperBuy != value)
                {
                    _isOperBuy = value;
                    RaisePropertyChanged("IsOperBuy");
                }
            }
        }

        /// <summary>
        /// номер сделки из таблицы всех сделок. Нужно только для реального и корректного обновления данных в режиме реального времени
        /// </summary>
        [XmlIgnore]
        public long NumberTick
        {
            get { return _numberTick; }
            set
            {
                if (_numberTick != value)
                {
                    _numberTick = value;
                    RaisePropertyChanged("NumberTick");
                }
            }
        }

        /// <summary>
        /// номер свечи в большей серии, при декомпрессии
        /// </summary>
        [XmlIgnore]
        public int NumberCandleDecompres
        {
            get { return _numberCandleDecompres; }
            set
            {
                if (_numberCandleDecompres != value)
                {
                    _numberCandleDecompres = value;

                    RaisePropertyChanged("NumberCandleDecompres");
                }
            }
        }

        /// <summary>
        /// номер свечи, используется только при компрессии
        /// </summary>
        public int NumberCandle
        {
            get { return _numberCandle; }
            set
            {
                if (_numberCandle != value)
                {
                    _numberCandle = value;
                    RaisePropertyChanged("NumberCandle");
                }
            }
        }


        /// <summary>
        /// Дата время свечи/тика
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
        /// . 
        /// </summary>
        public double High
        {
            get { return _high; }
            set
            {
                if (Math.Abs(_high - value) > CfgSourceEts.MyEpsilon)
                {
                    _high = value;
                    RaisePropertyChanged("High");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public double Low
        {
            get { return _low; }
            set
            {
                if (Math.Abs(_low - value) > CfgSourceEts.MyEpsilon)
                {
                    _low = value;
                    RaisePropertyChanged("Low");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public double Open
        {
            get { return _open; }
            set
            {
                if (Math.Abs(_open - value) > CfgSourceEts.MyEpsilon)
                {
                    _open = value;
                    RaisePropertyChanged("Open");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public double Close
        {
            get { return _close; }
            set
            {
                if (Math.Abs(_close - value) > CfgSourceEts.MyEpsilon)
                {
                    _close = value;
                    RaisePropertyChanged("Close");
                }
            }
        }

        /// <summary>
        /// объем. 
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
        /// объем Buy
        /// </summary>
        public double VolBuy
        {
            get { return _volBuy; }
            set
            {
                if (Math.Abs(_volBuy - value) > CfgSourceEts.MyEpsilon)
                {
                   _volBuy = value;
                    RaisePropertyChanged("VolBuy");
                }
            }
        }

        /// <summary>
        /// объем Sell
        /// </summary>
        public double VolSell
        {
            get { return _volSell; }
            set
            {
                if (Math.Abs(_volSell - value) > CfgSourceEts.MyEpsilon)
                {
                    _volSell = value;
                    RaisePropertyChanged("VolSell");
                }
            }
        }

        /// <summary>
        /// открытый интерес. 
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
        /// . 
        /// </summary>
        public ClasterCandleModel Claster
        {
            get { return _claster ?? (_claster = new ClasterCandleModel()); }
            set
            {
                if (_claster != value)
                {
                    _claster = value;
                    RaisePropertyChanged("Claster");
                }
            }
        }



        #endregion

        public void Copy(CandleModel model)
        {
            TradeDateTime = model.TradeDateTime;
            High = model.High;
            Low = model.Low;
            Open = model.Open;
            Close = model.Close;
            Volume = model.Volume;
            //VolBuy = model.VolBuy;
            //VolSell = model.VolSell;
            IsOperBuy = model.IsOperBuy;
            Oi = model.Oi;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("TradeDateTime", TradeDateTime);
            info.AddValue("High", High);
            info.AddValue("Low", Low);
            info.AddValue("Open", Open);
            info.AddValue("Close", Close);
            info.AddValue("Volume", Volume);
            info.AddValue("VolBuy", VolBuy);
            info.AddValue("VolSell", VolSell);
            info.AddValue("IsOperBuy", IsOperBuy);
            info.AddValue("Oi", Oi);
            info.AddValue("NumberCandle", NumberCandle);
            info.AddValue("NumberCandleDecompres", NumberCandleDecompres);
            info.AddValue("Claster", Claster);
        }

        #region Constructor
        public CandleModel()
        {

        }

		public CandleModel(CandleModel model)
		{
			TradeDateTime = model.TradeDateTime;
			High = model.High;
			Low = model.Low;
			Open = model.Open;
			Close = model.Close;
			Volume = model.Volume;
			VolBuy = model.VolBuy;
			VolSell = model.VolSell;
			IsOperBuy = model.IsOperBuy;
			Oi = model.Oi;
			NumberCandle = model.NumberCandle;
			NumberCandleDecompres = model.NumberCandleDecompres;
            Claster = model.Claster;

		}

		public CandleModel(SerializationInfo info, StreamingContext context)
        {
            TradeDateTime = info.GetDateTime("TradeDateTime");
            High = info.GetDouble("High");
            Low = info.GetDouble("Low");
            Open = info.GetDouble("Open");
            Close = info.GetDouble("Close");
            Volume = info.GetInt64("Volume");
            VolBuy = info.GetInt64("VolBuy");
            VolSell = info.GetInt64("VolSell");
            IsOperBuy = info.GetBoolean("IsOperBuy");
            Oi = info.GetInt32("Oi");
            NumberCandle = info.GetInt32("NumberCandle");
            NumberCandleDecompres = info.GetInt32("NumberCandleDecompres");
            Claster = (ClasterCandleModel)info.GetValue("Claster", typeof(ClasterCandleModel));
        }
        #endregion

    }
}
