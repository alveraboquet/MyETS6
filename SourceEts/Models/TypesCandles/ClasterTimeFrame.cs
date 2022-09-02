using CommonDataContract;
using SourceEts.Models.TimeFrameTransformModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SourceEts.Table.CandleVol
{
    [Serializable]
    public class ClasterTimeFrame : ViewModelBase, ISerializable
    {
        #region private 

        private string _typeTimeFrame;
        private int _timeFrame;
        private double _highClaster;
        private bool _isClasterTime;
        private List<ClasterCandleModel> _clasters;

        #endregion

        #region public 

        /// <summary>
        /// . 
        /// </summary>
        public string TypeTimeFrame
        {
            get { return _typeTimeFrame; }
            set
            {
                if (_typeTimeFrame != value)
                {
                    _typeTimeFrame = value;
                    RaisePropertyChanged("TypeTimeFrame");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public int TimeFrame
        {
            get { return _timeFrame; }
            set
            {
                if (_timeFrame != value)
                {
                    _timeFrame = value;
                    RaisePropertyChanged("TimeFrame");
                }
            }
        }

        /// <summary>
        /// Высота кластера, при построения кластера не по времени
        /// </summary>
        public double HighClaster
        {
            get { return _highClaster; }
            set
            {
                if (Math.Abs(_highClaster - value) > 0.0000001)
                {
                    _highClaster = value;
                    RaisePropertyChanged("HighClaster");
                }
            }
        }

        /// <summary>
        /// Кластер 
        /// </summary>
        public bool IsClasterTime
        {
            get { return _isClasterTime; }
            set
            {
                if (_isClasterTime != value)
                {
                    _isClasterTime = value;
                    RaisePropertyChanged("IsClasterTime");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public List<ClasterCandleModel> Clasters
        {
            get { return _clasters ?? (_clasters = new List<ClasterCandleModel>()); }
            set
            {
                if (_clasters != value)
                {
                    _clasters = value;
                    RaisePropertyChanged("Clasters");
                }
            }
        }

        #endregion


        public ClasterTimeFrame()
        {

        }


        public ClasterTimeFrame(SerializationInfo info, StreamingContext context)
        {
            TypeTimeFrame = info.GetString("TypeTimeFrame");
            TimeFrame = info.GetInt32("TimeFrame");
            HighClaster = info.GetDouble("HighClaster");
            IsClasterTime = info.GetBoolean("IsClasterTime");
            Clasters = (List<ClasterCandleModel>)info.GetValue("Clasters", typeof(List<ClasterCandleModel>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TypeTimeFrame", TypeTimeFrame);
            info.AddValue("TimeFrame", TimeFrame);
            info.AddValue("HighClaster", HighClaster);
            info.AddValue("IsClasterTime", IsClasterTime);
            info.AddValue("Clasters", Clasters);
        }
    }


    [Serializable]
    public class ClasterCandleModel : ViewModelBase, ISerializable
    {
        #region private

        //private DateTime _time;
        private List<ClasterDetailModel> _clasterDetails;
        //private double _vol;
        private double _delta;
        private double _buyVol;
        private double _sellVol;
        private int _buyCount;
        private int _sellCount;
        //private double _oi;

        //private double _open;
        //private double _close;
        //private double _high;
        //private double _low;
        #endregion


        #region public

        ///// <summary>
        ///// . 
        ///// </summary>
        //public DateTime Time
        //{
        //    get { return _time; }
        //    set
        //    {
        //        if (_time != value)
        //        {
        //            _time = value;
        //            RaisePropertyChanged("Time");
        //        }
        //    }
        //}

        /// <summary>
        /// . 
        /// </summary>
        public List<ClasterDetailModel> ClasterDetails
        {
            get { return _clasterDetails ?? (_clasterDetails = new List<ClasterDetailModel>()); }
            set
            {
                if (_clasterDetails != value)
                {
                    _clasterDetails = value;
                    RaisePropertyChanged("ClasterDetails");
                }
            }
        }

        ///// <summary>
        ///// . 
        ///// </summary>
        //public double Vol
        //{
        //    get { return _vol; }
        //    set
        //    {
        //        if (Math.Abs(_vol - value) > 0.0000001)
        //        {
        //            _vol = value;
        //            RaisePropertyChanged("Vol");
        //        }
        //    }
        //}

        /// <summary>
        /// . 
        /// </summary>
        public double Delta
        {
            get { return _delta; }
            set
            {
                if (Math.Abs(_delta - value) > 0.0000001)
                {
                    _delta = value;
                    RaisePropertyChanged("Delta");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public double BuyVol
        {
            get { return _buyVol; }
            set
            {
                if (Math.Abs(_buyVol - value) > 0.0000001)
                {
                    _buyVol = value;
                    RaisePropertyChanged("BuyVol");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public double SellVol
        {
            get { return _sellVol; }
            set
            {
                if (Math.Abs(_sellVol - value) > 0.0000001)
                {
                    _sellVol = value;
                    RaisePropertyChanged("SellVol");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public int BuyCount
        {
            get { return _buyCount; }
            set
            {
                if (_buyCount != value)
                {
                    _buyCount = value;
                    RaisePropertyChanged("BuyCount");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public int SellCount
        {
            get { return _sellCount; }
            set
            {
                if (_sellCount != value)
                {
                    _sellCount = value;
                    RaisePropertyChanged("SellCount");
                }
            }
        }

        [XmlIgnore]
        public double BuyVolBase { get; set; }
        [XmlIgnore]
        public double SellVolBase { get; set; }

        ///// <summary>
        ///// . 
        ///// </summary>
        //public double Oi
        //{
        //    get { return _oi; }
        //    set
        //    {
        //        if (Math.Abs(_oi - value) > 0.0000001)
        //        {
        //            _oi = value;
        //            RaisePropertyChanged("Oi");
        //        }
        //    }
        //}

        ///// <summary>
        ///// . 
        ///// </summary>
        //public double Open
        //{
        //    get { return _open; }
        //    set
        //    {
        //        if (Math.Abs(_open - value) > 0.0000001)
        //        {
        //            _open = value;
        //            RaisePropertyChanged("Open");
        //        }
        //    }
        //}

        ///// <summary>
        ///// . 
        ///// </summary>
        //public double Close
        //{
        //    get { return _close; }
        //    set
        //    {
        //        if (Math.Abs(_close - value) > 0.0000001)
        //        {
        //            _close = value;
        //            RaisePropertyChanged("Close");
        //        }
        //    }
        //}

        ///// <summary>
        ///// . 
        ///// </summary>
        //public double High
        //{
        //    get { return _high; }
        //    set
        //    {
        //        if (Math.Abs(_high - value) > 0.0000001)
        //        {
        //            _high = value;
        //            RaisePropertyChanged("High");
        //        }
        //    }
        //}

        ///// <summary>
        ///// . 
        ///// </summary>
        //public double Low
        //{
        //    get { return _low; }
        //    set
        //    {
        //        if (Math.Abs(_low - value) > 0.0000001)
        //        {
        //            _low = value;
        //            RaisePropertyChanged("Low");
        //        }
        //    }
        //}



        #endregion


        public ClasterCandleModel()
        {

        }


        public ClasterCandleModel(SerializationInfo info, StreamingContext context)
        {
            //Time = info.GetDateTime("Time");
            ClasterDetails = (List<ClasterDetailModel>)info.GetValue("ClasterDetails", typeof(List<ClasterDetailModel>));
            //Vol = info.GetDouble("Vol");
            Delta = info.GetDouble("Delta");
            BuyVol = info.GetDouble("BuyVol");
            SellVol = info.GetDouble("SellVol");
            SellCount = info.GetInt32("SellCount");
            BuyCount = info.GetInt32("BuyCount");
            //Oi = info.GetDouble("Oi");
            //Open = info.GetDouble("Open");
            //Close = info.GetDouble("Close");
            //High = info.GetDouble("High");
            //Low = info.GetDouble("Low");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue("Time", Time);
            info.AddValue("ClasterDetails", ClasterDetails);
            //info.AddValue("Vol", Vol);
            info.AddValue("Delta", Delta);
            info.AddValue("BuyVol", BuyVol);
            info.AddValue("SellVol", SellVol);
            info.AddValue("BuyCount", BuyCount);
            info.AddValue("SellCount", SellCount);
            //info.AddValue("Oi", Oi);
            //info.AddValue("Open", Open);
            //info.AddValue("Close", Close);
            //info.AddValue("High", High);
            //info.AddValue("Low", Low);

        }

        public void AddClasterTime(CandleModel candle, double minstep, int accuracy)
        {

            BuyVol += candle.VolBuy;
            SellVol += candle.VolSell;
            if (candle.VolSell > 0)
                SellCount += 1;
            if (candle.VolBuy > 0)
                BuyCount += 1;
            Delta = BuyVol - SellVol;

            if (ClasterDetails.Count == 0)
            {
                var clasterDetail = new ClasterDetailModel { Price = candle.Close };

                clasterDetail.BuyVol += candle.VolBuy;
                clasterDetail.SellVol += candle.VolSell;

                ClasterDetails.Add(clasterDetail);
            }
            else
            {
                //try
                //{
                var clasters = ClasterDetails;
                while ((candle.Close - clasters[clasters.Count - 1].Price) > 0.000000001)
                {
                    clasters.Add(new ClasterDetailModel { Price = Math.Round(clasters[clasters.Count - 1].Price + minstep, accuracy) });
                }

                while (clasters[0].Price - candle.Close > 0.000000001)
                {
                    clasters.Insert(0, new ClasterDetailModel { Price = Math.Round(clasters[0].Price - minstep, accuracy) });
                }

                int index = (int)((candle.Close - clasters[0].Price) / minstep);
                var clasterDetail = clasters[index];
                clasterDetail.BuyVol += candle.VolBuy;
                clasterDetail.SellVol += candle.VolSell;
            }

        }

    }


    /// <summary>
    /// из чего состоит кластер
    /// </summary>
    [Serializable]
    public class ClasterDetailModel : ViewModelBase, ISerializable
    {
        [XmlIgnore]
        public double BuyVolBase { get; set; }
        [XmlIgnore]
        public double SellVolBase { get; set; }

        #region private

        private double _price;
        private double _buyVol;
        private double _sellVol;

        #endregion

        #region public

        /// <summary>
        /// . 
        /// </summary>
        public double Price
        {
            get { return _price; }
            set
            {
                if (Math.Abs(_price - value) > 0.0000001)
                {
                    _price = value;
                    RaisePropertyChanged("Price");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public double BuyVol
        {
            get { return _buyVol; }
            set
            {
                if (Math.Abs(_buyVol - value) > 0.0000001)
                {
                    _buyVol = value;
                    RaisePropertyChanged("BuyVol");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public double SellVol
        {
            get { return _sellVol; }
            set
            {
                if (Math.Abs(_sellVol - value) > 0.0000001)
                {
                    _sellVol = value;
                    RaisePropertyChanged("SellVol");
                }
            }
        }





        #endregion

        public ClasterDetailModel()
        {

        }


        public ClasterDetailModel(SerializationInfo info, StreamingContext context)
        {
            Price = info.GetDouble("Price");
            BuyVol = info.GetDouble("BuyVol");
            SellVol = info.GetDouble("SellVol");

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Price", Price);
            info.AddValue("BuyVol", BuyVol);
            info.AddValue("SellVol", SellVol);
        }
    }
}
