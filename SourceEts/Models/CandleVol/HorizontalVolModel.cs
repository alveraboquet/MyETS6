using CommonDataContract;
using SourceEts.Models.TimeFrameTransformModel;
using System;
using System.Collections.Generic;

namespace SourceEts.Models.CandleVol
{
    public class HorizontalVolModel : ViewModelBase, IHorizontalVol
    {
        public List<CandleModel> AllTrades = new List<CandleModel>();
        public double VolBuyIceberg = 0;
        public double VolSellIceberg = 0;
        public double OrdersBuyIceBerg = 0;
        public double OrdersSellIceBerg = 0;
        public bool IsUpdate { get; set; }

        private bool _isIceberg;

        /// <summary>
        /// Используется только для теста, т.к. снимкси стаканов могут иметь разрыв более 1,5 секунд
        /// </summary>
        public int StepNumber { get; set; }
        /// <summary>
        /// Время появления предполагаемого айсберга, требует подтверждения через 1,5 секунды
        /// </summary>
        public DateTime IcebergBuyAppear { get; set; }
        public bool IsIcebergBuyAppear { get; set; }
        public DateTime IcebergSellAppear { get; set; }
        public bool IsIcebergSellAppear { get; set; }

        /// <summary>
        /// . 
        /// </summary>
        public bool IsIceberg
        {
            get { return _isIceberg; }
            set
            {
                if (Math.Abs(Price - 237.95) < 0.000000001)
                {
                }
                if (value == true && _isIceberg == false)
                {
                }
                if (_isIceberg != value)
                {
                    _isIceberg = value;
                    RaisePropertyChanged("IsIceberg");
                }
            }
        }

        /// <summary>
        /// Последняя 
        /// </summary>
        public DateTime LastTimeVisibleIsbergBuy { get; set; }
        public DateTime LastTimeVisibleIsbergSell { get; set; }
        //public bool IsBuy { get; set; }
        public double LastVolGlassBuy { get; set; }
        public double LastVolGlassSell { get; set; }

        #region private 

        private double _price;
        private double _volBuy;
        private double _volSell;

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
                if (Math.Abs(_price - value) > CfgSourceEts.MyEpsilon)
                {
                    _price = value;
                    RaisePropertyChanged("Price");
                }
            }
        }

        /// <summary>
        /// . 
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
        /// . 
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



        #endregion
    }
}
