using CommonDataContract;
using System;
using System.Collections.ObjectModel;

namespace SourceEts.Models.TimeFrameTransformModel
{
    /// <summary>
    /// Временная колллекция данных по свечам, при загрузке ее с сервера
    /// </summary>
    public class Candles : ViewModelBase
    {
        /// <summary>
        /// Требуется для IBTWS при запросе свечей раз в минуту
        /// </summary>
        public DateTime LasDateTimeUpdate { get; set; }

        #region Private

        private int _secid;
        private int _period;
        private int _interval;
        private string _typeTimeFram;
        private int _status;
        private bool _isLoad;
        private string _board;
        private string _seccode;
        private string _classCode;
        private int _idRequest;//Требуется для IB TWS
                               //private int _idInstrument;//Требуется для IB TWS

        private ObservableCollection<CandleModel> _candle;

        #endregion

        #region Public

        ///// <summary>
        ///// Требуется для идентификации инструмента в IB TWS
        ///// </summary>
        //public int IdInstrument
        //{
        //    get { return _idInstrument; }
        //    set
        //    {
        //        if (_idInstrument != value)
        //        {
        //            _idInstrument = value;
        //            RaisePropertyChanged("IdInstrument");
        //        }
        //    }
        //}

        /// <summary>
        /// Требуется для идентификации инструмента в IB TWS
        /// </summary>
        public int IdRequest
        {
            get { return _idRequest; }
            set
            {
                if (_idRequest != value)
                {
                    _idRequest = value;
                    RaisePropertyChanged("IdRequest");
                }
            }
        }

        /// <summary>
        /// Загружена история или нет.
        /// </summary>
        public bool IsLoad
        {
            get { return _isLoad; }
            set
            {
                if (_isLoad != value)
                {
                    _isLoad = value;
                    RaisePropertyChanged("IsLoad");
                }
            }
        }

        /// <summary>
        /// Идентификатор бумаги
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
        /// Тип интервала таймфрейма
        /// </summary>
        public string TypeTimeFram
        {
            get { return _typeTimeFram; }
            set
            {
                if (_typeTimeFram != value)
                {
                    _typeTimeFram = value;
                    RaisePropertyChanged("TypeTimeFram");
                }
            }
        }

        /// <summary>
        /// Интервал таймфрейма
        /// </summary>
        public int Interval
        {
            get { return _interval; }
            set
            {
                if (_interval != value)
                {
                    _interval = value;
                    RaisePropertyChanged("Interval");
                }
            }
        }

        /// <summary>
        /// Идентификатор периода"
        /// </summary>
        public int Period
        {
            get { return _period; }
            set
            {
                if (_period != value)
                {
                    _period = value;
                    RaisePropertyChanged("Period");
                }
            }
        }

        /// <summary>
        /// Используется для TransaqConnector
        /// Параметр "status" показывает, осталась ли еще история Возможные значения:
        /// 0 - данных больше нет (дочерпали до дна)
        /// 1 - заказанное количество выдано (если надо еще - делать еще запрос)
        /// 2 - продолжение следует (будет еще порция)
        /// 3 - требуемые данные недоступны (есть смысл попробовать запросить позже).
        /// </summary>
        public int Status
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
        /// идентификатор режима торгов
        /// </summary>
        public string Board
        {
            get { return _board; }
            set
            {
                if (_board != value)
                {
                    _board = value;
                    RaisePropertyChanged("Board");
                }
            }
        }

        /// <summary>
        /// код инструмента
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
        /// код класса инструмента
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
        /// Информация по свечам
        /// </summary>
        public ObservableCollection<CandleModel> Candle
        {
            get { return _candle ?? (_candle = new ObservableCollection<CandleModel>()); }
            set
            {
                if (_candle != value)
                {
                    _candle = value;
                    RaisePropertyChanged("Candle");
                }
            }
        }

        #endregion


    }
}
