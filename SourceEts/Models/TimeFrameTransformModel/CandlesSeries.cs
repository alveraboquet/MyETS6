using CommonDataContract;
using SourceEts.BaseClass;
using SourceEts.Table.CandleVol;
using System;
using System.Collections.Generic;

namespace SourceEts.Models.TimeFrameTransformModel
{
    /// <summary>
    /// Данные по кллекциям свечей и последнему индексу
    /// </summary>
    public class CandlesSeries : ViewModelBase, ICandlesSeries
    {

        public void AddClasterTime(ClasterCandleModel claster, CandleModel candle, double minstep, int accuracy)
        {
            if (candle.VolBuy == 0 && candle.VolSell == 0)
                return;

            if (candle.TradeDateTime.Hour == 13 && candle.TradeDateTime.Minute == 37 &&
                candle.Close == 334.9)
            {

            }

            claster.BuyVol += candle.VolBuy;
            claster.SellVol += candle.VolSell;
            claster.Delta = claster.BuyVol - claster.SellVol;
            if (claster.ClasterDetails.Count == 0)
            {
                var clasterDetail = new ClasterDetailModel { Price = candle.Close };

                clasterDetail.BuyVol += candle.VolBuy;
                clasterDetail.SellVol += candle.VolSell;

                claster.ClasterDetails.Add(clasterDetail);
            }
            else
            {
                //try
                //{
                var clasters = claster.ClasterDetails;
                bool add = false;
                for (int i = 0; i < clasters.Count; i++)
                {
                    if (clasters[i].Price > candle.Close)
                    {
                        clasters.Insert(i, new ClasterDetailModel { Price = candle.Close, BuyVol = candle.VolBuy, SellVol = candle.VolSell });
                        add = true;
                        break;
                    }

                    if (Math.Abs(clasters[i].Price - candle.Close) < 0.000000000001)
                    {
                        clasters[i].BuyVol += candle.VolBuy;
                        clasters[i].SellVol += candle.VolSell;
                        add = true;
                        break;
                    }
                }
                if (!add)
                    clasters.Add(new ClasterDetailModel { Price = candle.Close, BuyVol = candle.VolBuy, SellVol = candle.VolSell });



                //while ((candle.Close - clasters[clasters.Count - 1].Price) > 0.000000001)
                //    {
                //        clasters.Add(new ClasterDetailModel { Price = Math.Round(clasters[clasters.Count - 1].Price + minstep, accuracy) });
                //    }

                //    while (clasters[0].Price - candle.Close > 0.000000001)
                //    {
                //        clasters.Insert(0, new ClasterDetailModel { Price = Math.Round(clasters[0].Price - minstep, accuracy) });
                //    }

                //int index = (int)((candle.Close - clasters[0].Price) / minstep);
                //var clasterDetail = clasters[index];
                //clasterDetail.BuyVol += candle.VolBuy;
                //clasterDetail.SellVol += candle.VolSell;
                //}
                //catch (Exception ex)
                //{ }
            }
        }



        private int _candleCount;
        private int _lastIndexCandle;

        private List<int> _numberCandle;
        //private List<int> _numberCandleAfterCompress;

        private List<double> _highSeries;
        private List<DateTime> _dateTimeCandle;
        private List<double> _lowSeries;
        private List<double> _openSeries;
        private List<double> _closeSeries;
        private List<double> _medianSeries; //(h+l)/2
        private List<double> _typicalSeries;//(h+l+c)/3
        private List<double> _volume;//объем
        private List<double> _volBuy;//объем
        private List<double> _volSell;//объем
        private List<int> _oi;//открытый интерес
        private List<ClasterCandleModel> _clasterTimeSeries;
        ///// <summary>
        ///// Вызывается для освобождения памяти
        ///// Удаляются данные из всех серий и изменятеся IndexBar
        ///// Необходимо учесть в роботе данные изменения как для тестов, так и для реально торговли, 
        ///// если включен режим оптимизации работы с большим объемом данных
        ///// </summary>
        //public void ClearDataSeriesSeries(int max, int countDel)
        //{
        //    if (CandleCount > max)
        //    {
        //        if (NumberCandle.Count > max)
        //            NumberCandle.RemoveRange(0, countDel);
        //        if (NumberCandleAfterCompress.Count > max)
        //            NumberCandleAfterCompress.RemoveRange(0, countDel);
        //        HighSeries.RemoveRange(0, countDel);
        //        DateTimeCandle.RemoveRange(0, countDel);
        //        LowSeries.RemoveRange(0, countDel);
        //        OpenSeries.RemoveRange(0, countDel);
        //        CloseSeries.RemoveRange(0, countDel);
        //        if (MedianSeries.Count > max)
        //            MedianSeries.RemoveRange(0, countDel);
        //        if (TypicalSeries.Count > max)
        //            TypicalSeries.RemoveRange(0, countDel);
        //        Volume.RemoveRange(0, countDel);
        //        VolBuy.RemoveRange(0, countDel);
        //        VolSell.RemoveRange(0, countDel);
        //        Oi.RemoveRange(0, countDel);

        //        CandleCount = _closeSeries.Count;
        //        _lastIndexCandle = _lastIndexCandle - countDel;
        //        if (_lastIndexCandle < 0)
        //            _lastIndexCandle = 0;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public void ClearDataSeriesSeries()
        {

            NumberCandle.Clear();
            HighSeries.Clear();
            DateTimeCandle.Clear();
            LowSeries.Clear();
            OpenSeries.Clear();
            CloseSeries.Clear();
            CloseSeries.Clear();

            MedianSeries.Clear();
            ClasterTimeSeries.Clear();

            TypicalSeries.Clear();
            Volume.Clear();
            VolBuy.Clear();
            VolSell.Clear();
            Oi.Clear();

            CandleCount = _closeSeries.Count;
            _lastIndexCandle = 0;
            _lastIndexCandle = 0;
        }

        ///// <summary>
        ///// Коллекция номеров свечей
        ///// </summary>
        //public List<int> NumberCandleAfterCompress
        //{
        //    get
        //    {
        //        if (_numberCandleAfterCompress == null)
        //            _numberCandleAfterCompress = new List<int>();
        //        return _numberCandleAfterCompress;
        //    }
        //    set
        //    {
        //        if (_numberCandleAfterCompress != value)
        //        {
        //            _numberCandleAfterCompress = value;
        //            RaisePropertyChanged("NumberCandleAfterCompress");
        //        }
        //    }
        //}

        /// <summary>
        /// Коллекция номера свечи большего таймфрейма с учетом компрессии
        /// </summary>
        public List<int> NumberCandle
        {
            get
            {
                if (_numberCandle == null)
                    _numberCandle = new List<int>();
                return _numberCandle;
            }
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
        /// Время свечи
        /// </summary>
        public List<DateTime> DateTimeCandle
        {
            get
            {
                if (_dateTimeCandle == null)
                    _dateTimeCandle = new List<DateTime>();
                return _dateTimeCandle;
            }
            set
            {
                if (_dateTimeCandle != value)
                {
                    _dateTimeCandle = value;
                    RaisePropertyChanged("DateTimeCandle");
                }
            }
        }

        /// <summary>
        /// (h+l+c)/3
        /// </summary>
        public List<double> TypicalSeries
        {
            get { return _typicalSeries ?? (_typicalSeries = new List<double>()); }
            set
            {
                if (_typicalSeries != value)
                {
                    _typicalSeries = value;
                    RaisePropertyChanged("TypicalSeries");
                }
            }
        }


        /// <summary>
        /// (h+l)/2
        /// </summary>
        public List<double> MedianSeries
        {
            get { return _medianSeries ?? (_medianSeries = new List<double>()); }
            set
            {
                if (_medianSeries != value)
                {
                    _medianSeries = value;
                    RaisePropertyChanged("MedianSeries");
                }
            }
        }

        /// <summary>
        /// Индекс последней обработанной в серии свечи, сделки
        /// </summary>
        public int LastIndexCandle
        {
            get { return _lastIndexCandle; }
            set
            {
                if (_lastIndexCandle != value)
                {
                    _lastIndexCandle = value;
                    RaisePropertyChanged("LastIndexCandle");
                }
            }
        }

        /// <summary>
        /// Количество свечей в серии
        /// </summary>
        public int CandleCount
        {
            get { return _candleCount; }
            set
            {
                if (_candleCount != value)
                {
                    _candleCount = value;
                    RaisePropertyChanged("CandleCount");
                }
            }
        }


        /// <summary>
        /// . 
        /// </summary>
        public List<double> HighSeries
        {
            get
            {
                if (_highSeries == null)
                    _highSeries = new List<double>();
                return _highSeries;
            }
            set
            {
                if (_highSeries != value)
                {
                    _highSeries = value;
                    RaisePropertyChanged("HighSeries");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public List<double> LowSeries
        {
            get
            {
                if (_lowSeries == null)
                    _lowSeries = new List<double>();
                return _lowSeries;
            }
            set
            {
                if (_lowSeries != value)
                {
                    _lowSeries = value;
                    RaisePropertyChanged("LowSeries");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public List<double> OpenSeries
        {
            get
            {
                if (_openSeries == null)
                    _openSeries = new List<double>();
                return _openSeries;
            }
            set
            {
                if (_openSeries != value)
                {
                    _openSeries = value;
                    RaisePropertyChanged("OpenSeries");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public List<double> CloseSeries
        {
            get
            {
                if (_closeSeries == null)
                    _closeSeries = new List<double>();
                return _closeSeries;
            }
            set
            {
                if (_closeSeries != value)
                {
                    _closeSeries = value;
                    RaisePropertyChanged("CloseSeries");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public List<ClasterCandleModel> ClasterTimeSeries
        {
            get
            {
                if (_clasterTimeSeries == null)
                    _clasterTimeSeries = new List<ClasterCandleModel>();
                return _clasterTimeSeries;
            }
            set
            {
                if (_clasterTimeSeries != value)
                {
                    _clasterTimeSeries = value;
                    RaisePropertyChanged("ClasterTimeSeries");
                }
            }
        }

        public int LastUpdateIndex = 0;
        public double VolBuyBase = 0;
        public double VolSellBase = 0;
        public double VolBase = 0;

        /// <summary>
        /// объем. 
        /// </summary>
        public List<double> Volume
        {
            get { return _volume ?? (_volume = new List<double>()); }
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    RaisePropertyChanged("Volume");
                }
            }
        }


        /// <summary>
        /// объем. 
        /// </summary>
        public List<double> VolBuy
        {
            get { return _volBuy ?? (_volBuy = new List<double>()); }
            set
            {
                if (_volBuy != value)
                {
                    _volBuy = value;
                    RaisePropertyChanged("VolBuy");
                }
            }
        }



        /// <summary>
        /// объем. 
        /// </summary>
        public List<double> VolSell
        {
            get { return _volSell ?? (_volSell = new List<double>()); }
            set
            {
                if (_volSell != value)
                {
                    _volSell = value;
                    RaisePropertyChanged("VolSell");
                }
            }
        }

        /// <summary>
        /// открытый интерес. 
        /// </summary>
        public List<int> Oi
        {
            get
            {
                if (_oi == null)
                    _oi = new List<int>();
                return _oi;
            }
            set
            {
                if (_oi != value)
                {
                    _oi = value;
                    RaisePropertyChanged("Oi");
                }
            }
        }




        ///// <summary>
        ///// Серия свечей
        ///// </summary>
        //public ObservableCollection<Candle> CandleSeries
        //{
        //    get { return _candleSeries ?? (_candleSeries = new ObservableCollection<Candle>()); }
        //    set
        //    {
        //        if (_candleSeries != value)
        //        {
        //            _candleSeries = value;
        //            RaisePropertyChanged("CandleSeries");
        //        }
        //    }
        //}



    }
}
