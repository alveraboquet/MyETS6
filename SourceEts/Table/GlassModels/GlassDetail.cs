using CommonDataContract;
using SourceEts.Table.TableClass;
using System;

namespace SourceEts.Table
{
    public class GlassDetail : ViewModelBase, IGlassDetail
    {

        #region private 

        private bool _isIsberg;
        private double _volBuy;
        private double _volSell;
        private DateTime _lastTimeAnalyze;
        private string _color;

        #endregion

        #region public

        /// <summary>
        /// . 
        /// </summary>
        public bool IsIsberg
        {
            get { return _isIsberg; }
            set
            {
                if (_isIsberg != value)
                {
                    _isIsberg = value;
                    RaisePropertyChanged("IsIsberg");
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
                if (Math.Abs(_volBuy - value) > 0.0000001)
                {
                    _volBuy = value;
                    RaisePropertyChanged("VolBuy");
                }
            }
        }

        /// <summary>
        /// Накопленный объем (необходимо учесть, а что если стакан не с самого начала)
        /// </summary>
        public double VolSell
        {
            get { return _volSell; }
            set
            {
                if (Math.Abs(_volSell - value) > 0.0000001)
                {
                    _volSell = value;
                    RaisePropertyChanged("VolSell");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public DateTime LastTimeAnalyze
        {
            get { return _lastTimeAnalyze; }
            set
            {
                if (_lastTimeAnalyze != value)
                {
                    _lastTimeAnalyze = value;
                    RaisePropertyChanged("LastTimeAnalyze");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public string Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    RaisePropertyChanged("Color");
                }
            }
        }



        #endregion

    }
}
