using System;
using CommonDataContract;
using SourceEts.Models.CandleVol;

namespace SourceEts.Table
{
    /// <summary>
    /// Котировки стакана
    /// </summary>
    public class GlassQuotation : ViewModelBase, IGlassQuotation
    {
  
        #region pirvate

        private double _qty;
        private double _buyQty;
        private double _sellQty;
        private double _price;
        private IHorizontalVol _detail;
        #endregion

        #region public

        /// <summary>
        /// Количество пользовательских заявок
        /// </summary>
        public double Qty
        {
            get { return _qty; }
            set
            {
                if (Math.Abs(_qty - value) > CfgSourceEts.MyEpsilon)
                {
                    _qty = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }


        /// <summary>
        /// Количество лотов на покупку
        /// </summary>
        public double BuyQty
        {
            get { return _buyQty; }
            set
            {
                if (Math.Abs(_buyQty - value) > CfgSourceEts.MyEpsilon)
                {
                    _buyQty = value;
                    RaisePropertyChanged("BuyQty");
                }
            }
        }

        /// <summary>
        /// Количество лотов на продажу
        /// </summary>
        public double SellQty
        {
            get { return _sellQty; }
            set
            {
                if (Math.Abs(_sellQty - value) > CfgSourceEts.MyEpsilon)
                {
                    _sellQty = value;
                    RaisePropertyChanged("SellQty");
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
                    if (value < 100)
                    {
                    }

                    _price = value;
                    RaisePropertyChanged("Price");
                }
            }
        }


        /// <summary>
        /// цена
        /// </summary>
        public IHorizontalVol Detail
        {
            get { return _detail ?? (_detail = new HorizontalVolModel()); }
            set
            {
                _detail = value;
                RaisePropertyChanged("Detail");

            }
        }

        #endregion
    }
}
