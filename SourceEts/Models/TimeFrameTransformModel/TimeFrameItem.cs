using CommonDataContract;
using SourceEts.BaseClass;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SourceEts.Models.TimeFrameTransformModel
{

    /// <summary>
    /// Данные по настроенным таймфреймам
    /// </summary>
    [Serializable]
    public class TimeFrameItem : ViewModelBase, ISerializable//, IEditableObject
    {
        public int LastIndexCandle { get; set; }
        public double VolBuyBase { get; set; }
        public double VolSellBase { get; set; }
        public double VolBase { get; set; }

        /// <summary>
        /// Последний добавленный объем. Нужен только при добавлении свечей, вместо тиков
        /// </summary>
        [XmlIgnore]
        public double LastQtyAdd { get; set; }

        #region private

        private int _timeFrame;
        private string _typeTimeFrame;
        private int _tmpTimeFrameCompressFrom; //только для временных коллекций, из какого таймфрейма сделана
        private string _tmpTypeTimeFrameCompressFrom; //только для временных коллекций, из какого таймфрейма сделана
        private string _comment;
        private bool _check;
        private bool _initCompressComplete;//Первоначальная компрессия закончена
        private long _lastNumberDeal;
        private int _lastTickIndex; //Индекс последнего добавленного тика из источника по тикам, используется только для нестандартных фреймов
        private int _lastCompressIndexAdd; //Последняя добавленная свеча в больший таймфрейм. Чтоб не добавлялась лишняя свеча и не сбивался расчет на одну свечу
        //private Guid _guid;
        private List<CandleModel> _candleItemsSource; //источник, из которого собирается новый фрейм. Используется только во временной коллекции
        private List<CandleModel> _candleItems; //Компрессионные свечи
        private List<CandleModel> _candleItemsDecompress; //Декомпрессованые свечи источника, используется только временной коллекцией
        private List<CandleModel> _candleItemsStepTest; //Коллекция свечей только для пошагового тестирования. Напирмер при сжатии из минуток в 10 минутки, какждая последущая свеча серии будет учитывать предыдущие значения свечей для формирования новой свечи и учитывать текущее значения свечи. 
        private List<Guid> _guidList; //guid роботов использующих данный таймфрейм, необходим только для временной коллекции
        private ICandlesSeries _candleSeries; //Конечная коллекция серий данных свечей базовая или компрессионная . 
        private ICandlesSeries _candleSeriesSource; //Серии свечей источника. 
        private ICandlesSeries _candleSeriesDecompress; //Декомпрессованя серия относительно источника, используется только временной коллекцией
        private ICandlesSeries _candleSeriesStepTest; //Коллекция серий только для пошагового тестирования. Напирмер при сжатии из минуток в 10 минутки, какждая последущее значение серии будет учитывать предыдущие значения для формирования новой и учитывать текущее значение. 
        private bool _isInitCandleSeries; //Первичная инициализация серии закончена , чтоб другой поток не начинал формировать дополнительные свечи, пока не будет закона работа над созданием первичной серии
        private bool _isInitCandleSeriesSource; //Первичная инициализация серии закончена, чтоб другой поток не начинал формировать дополнительные свечи, пока не будет закона работа над созданием первичной серии
        private bool _isInitCandleSeriesDecompress; //Первичная инициализация серии закончена, чтоб другой поток не начинал формировать дополнительные свечи, пока не будет закона работа над созданием первичной серии

        private DateTime _lastTickUpdate;
        private int _curTickCompress;//если идет компрессия в тики, то в этом случае необходимо считать тики в свечах
        private bool _removeLastCandle;
        /// <summary>
        /// Количество посчитанных дней, недель, когда сжатие идет в несколько дней
        /// </summary>
        public int CountAdd { get; set; }
        /// <summary>
        /// Дата по которой было посчитано
        /// </summary>
        public DateTime LasDateTimeCountAdd { get; set; }

        private List<int> _istCandleBigFrameToSmall;
        /// <summary>
        /// Начало времени, с которого формируются данные для свечей
        /// </summary>
        public DateTime TimeStartData { get; set; }
        /// <summary>
        /// Время, по которое формируются данные для свечей
        /// </summary>
        public DateTime TimeEndData { get; set; }

        #endregion

        #region public

        /// <summary>
        /// Первичная инициализация серии закончена, чтоб другой поток не начинал формировать дополнительные свечи, пока не будет закона работа над созданием первичной серии
        /// </summary>
        [XmlIgnore]
        public bool IsInitCandleSeries
        {
            get { return _isInitCandleSeries; }
            set
            {
                if (_isInitCandleSeries != value)
                {
                    _isInitCandleSeries = value;
                    RaisePropertyChanged("IsInitCandleSeries");
                }
            }
        }

        /// <summary>
        /// Первичная инициализация серии закончена, чтоб другой поток не начинал формировать дополнительные свечи, пока не будет закона работа над созданием первичной серии
        /// </summary>
        [XmlIgnore]
        public bool IsInitCandleSeriesSource
        {
            get { return _isInitCandleSeriesSource; }
            set
            {
                if (_isInitCandleSeriesSource != value)
                {
                    _isInitCandleSeriesSource = value;
                    RaisePropertyChanged("IsInitCandleSeriesSource");
                }
            }
        }

        /// <summary>
        /// Первичная инициализация серии закончена, чтоб другой поток не начинал формировать дополнительные свечи, пока не будет закона работа над созданием первичной серии
        /// </summary>
        [XmlIgnore]
        public bool IsInitCandleSeriesDecompress
        {
            get { return _isInitCandleSeriesDecompress; }
            set
            {
                if (_isInitCandleSeriesDecompress != value)
                {
                    _isInitCandleSeriesDecompress = value;
                    RaisePropertyChanged("IsInitCandleSeriesDecompress");
                }
            }
        }

        /// <summary>
        /// Первоначальная компрессия закончена
        /// </summary>
        public bool InitCompressComplete
        {
            get { return _initCompressComplete; }
            set
            {
                if (_initCompressComplete != value)
                {
                    _initCompressComplete = value;
                    RaisePropertyChanged("InitCompressComplete");
                }
            }
        }


        /// <summary>
        /// Последняя добавленная свеча в больший таймфрейм. Чтоб не добавлялась лишняя свеча и не сбивался расчет на одну свечу
        /// </summary>
        [XmlIgnore]
        public int LastCompressIndexAdd
        {
            get { return _lastCompressIndexAdd; }
            set
            {
                if (_lastCompressIndexAdd != value)
                {
                    _lastCompressIndexAdd = value;
                    RaisePropertyChanged("LastCompressIndexAdd");
                }
            }
        }

        /// <summary>
        /// Индекс последнего добавленного тика из источника по тикам, используется только для нестандартных фреймов
        /// </summary>
        public int LastTickIndex
        {
            get { return _lastTickIndex; }
            set
            {
                if (_lastTickIndex != value)
                {
                    _lastTickIndex = value;
                    RaisePropertyChanged("LastTickIndex");
                }
            }
        }

        /// <summary>
        /// если идет компрессия в тики, то в этом случае необходимо считать тики в свечах
        /// </summary>
        public int CurTickCompress
        {
            get { return _curTickCompress; }
            set
            {
                if (_curTickCompress != value)
                {
                    _curTickCompress = value;
                    RaisePropertyChanged("CurTickCompress");
                }
            }
        }

        /// <summary>
        /// источник, из которого собирается новый фрейм. Используется только во временной коллекции
        /// </summary>
        [XmlIgnore]
        public List<CandleModel> CandleItemsSource
        {
            get { return _candleItemsSource ?? (_candleItemsSource = new List<CandleModel>()); }
            set
            {
                if (_candleItemsSource != value)
                {
                    _candleItemsSource = value;
                    RaisePropertyChanged("CandleItemsSource");
                }
            }
        }

        /// <summary>
        /// guid роботов использующих данный таймфрейм, необходим только для временной коллекции
        /// </summary>
        [XmlIgnore]
        public List<Guid> GuidList
        {
            get { return _guidList ?? (_guidList = new List<Guid>()); }
            set
            {
                if (_guidList != value)
                {
                    _guidList = value;
                    RaisePropertyChanged("GuidList");
                }
            }
        }

        /// <summary>
        /// Конечная коллекция серий данных свечей базовая или компрессионная . 
        /// </summary>
        [XmlIgnore]
        public ICandlesSeries CandleSeries
        {
            get { return _candleSeries ?? (_candleSeries = new CandlesSeries()); }
            set
            {
                if (_candleSeries != value)
                {
                    _candleSeries = value;
                    RaisePropertyChanged("CandleSeries");
                }
            }
        }


        /// <summary>
        /// Декомпрессованя серия относительно источника, используется только временной коллекцией
        /// </summary>
        [XmlIgnore]
        public ICandlesSeries CandleSeriesDecompress
        {
            get { return _candleSeriesDecompress ?? (_candleSeriesDecompress = new CandlesSeries()); }
            set
            {
                if (_candleSeriesDecompress != value)
                {
                    _candleSeriesDecompress = value;
                    RaisePropertyChanged("CandleSeriesDecompress");
                }
            }
        }

        /// <summary>
        /// Коллекция серий только для пошагового тестирования. Напирмер при сжатии из минуток в 10 минутки, какждая последущее значение серии будет учитывать предыдущие значения для формирования новой и учитывать текущее значение. 
        /// </summary>
        [XmlIgnore]
        public ICandlesSeries CandleSeriesStepTest
        {
            get { return _candleSeriesStepTest ?? (_candleSeriesStepTest = new CandlesSeries()); }
            set
            {
                if (_candleSeriesStepTest != value)
                {
                    _candleSeriesStepTest = value;
                    RaisePropertyChanged("CandleSeriesStepTest");
                }
            }
        }

        /// <summary>
        /// Серии свечей  источника
        /// </summary>
        [XmlIgnore]
        public ICandlesSeries CandleSeriesSource
        {
            get { return _candleSeriesSource ?? (_candleSeriesSource = new CandlesSeries()); }
            set
            {
                if (_candleSeriesSource != value)
                {
                    _candleSeriesSource = value;
                    RaisePropertyChanged("CandleSeriesSource");
                }
            }
        }

        /// <summary>
        /// Удалена последняя свеча или нет. Если в течение дня был перезапущен терминал РБКМ, то чтоб повторных тиков в свечу не попало, удалаяем ее. 
        /// </summary>
        [XmlIgnore]
        public bool RemoveLastCandle
        {
            get { return _removeLastCandle; }
            set
            {
                if (_removeLastCandle != value)
                {
                    _removeLastCandle = value;
                    RaisePropertyChanged("RemoveLastCandle");
                }
            }
        }


        /// <summary>
        /// Время последнего тика
        /// </summary>
        public DateTime LastTickUpdate
        {
            get { return _lastTickUpdate; }
            set
            {
                if (_lastTickUpdate != value)
                {
                    _lastTickUpdate = value;
                    RaisePropertyChanged("LastTickUpdate");
                }
            }
        }


        /// <summary>
        /// Используется для массовых удалений таймфреймов
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


        /// <summary>
        /// Последняя строка обработанная данным инструментом из таблицы всех сделок
        /// </summary>
        public long LastNumberDeal
        {
            get { return _lastNumberDeal; }
            set
            {
                if (_lastNumberDeal != value)
                {
                    _lastNumberDeal = value;
                    RaisePropertyChanged("LastNumberDeal");
                }
            }
        }

        /// <summary>
        /// тип таймфрейма (минуты, секунды, тики или дни)
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
        /// Задаваемый таймфрейм, задается в целых числах
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
        /// только для временных коллекций, из какого таймфрейма сделана/ тип таймфрейма (минуты, секунды, тики или дни)
        /// </summary>
        [XmlIgnore]
        public string TmpTypeTimeFrameCompressFrom
        {
            get { return _tmpTypeTimeFrameCompressFrom; }
            set
            {
                if (_tmpTypeTimeFrameCompressFrom != value)
                {
                    _tmpTypeTimeFrameCompressFrom = value;
                    RaisePropertyChanged("TmpTypeTimeFrameCompressFrom");
                }
            }
        }

        /// <summary>
        /// только для временных коллекций, из какого таймфрейма сделана /Задаваемый таймфрейм, задается в целых числах
        /// </summary>
        [XmlIgnore]
        public int TmpTimeFrameCompressFrom
        {
            get { return _tmpTimeFrameCompressFrom; }
            set
            {
                if (_tmpTimeFrameCompressFrom != value)
                {
                    _tmpTimeFrameCompressFrom = value;
                    RaisePropertyChanged("TmpTimeFrameCompressFrom");
                }
            }
        }

        /// <summary>
        /// Комментарий к таймфрейму
        /// </summary>
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    RaisePropertyChanged("Comment");
                }
            }
        }

        /// <summary>
        /// Коллекция свечей по заданному таймфрейму. Компрессионная коллекция свечей или базовая
        /// </summary>
        public List<CandleModel> CandleItems
        {
            get { return _candleItems ?? (_candleItems = new List<CandleModel>()); }
            set
            {
                if (_candleItems != value)
                {
                    _candleItems = value;
                    RaisePropertyChanged("CandleItems");
                }
            }
        }

        /// <summary>
        /// Коллекция декомпресованных свечей с номерами свечей относительно источника
        /// </summary>
        [XmlIgnore]
        public List<CandleModel> CandleItemsDecompress
        {
            get { return _candleItemsDecompress ?? (_candleItemsDecompress = new List<CandleModel>()); }
            set
            {
                if (_candleItemsDecompress != value)
                {
                    _candleItemsDecompress = value;
                    RaisePropertyChanged("CandleItemsDecompress");
                }
            }
        }

        /// <summary>
        /// Коллекция свечей только для пошагового тестирования. Напирмер при сжатии из минуток в 10 минутки, какждая последущая свеча серии будет учитывать предыдущие значения свечей для формирования новой свечи и учитывать текущее значения свечи. 
        /// </summary>
        [XmlIgnore]
        public List<CandleModel> CandleItemsStepTest
        {
            get { return _candleItemsStepTest ?? (_candleItemsStepTest = new List<CandleModel>()); }
            set
            {
                if (_candleItemsStepTest != value)
                {
                    _candleItemsStepTest = value;
                    RaisePropertyChanged("CandleItemsStepTest");
                }
            }
        }


        /// <summary>
        /// Список окончания номеров свечей большего фрейма на малом фрейме
        /// </summary>
        [XmlIgnore]
        public List<int> ListCandleBigFrameToSmall
        {
            get { return _istCandleBigFrameToSmall ?? (_istCandleBigFrameToSmall = new List<int>()); }
            set
            {
                if (_istCandleBigFrameToSmall != value)
                {
                    _istCandleBigFrameToSmall = value;
                    RaisePropertyChanged("ListCandleBigFrameToSmall");
                }
            }
        }


        ///// <summary>
        ///// уникальный номер таймфрейм, чтоб не терялся при сортировке
        ///// </summary>
        //public Guid GuidItem
        //{
        //    get { return _guid; }
        //    set
        //    {
        //        if (_guid != value)
        //        {
        //            _guid = value;
        //            RaisePropertyChanged("GuidItem");
        //        }
        //    }
        //}

        #endregion

        public TimeFrameItem() { }



        public TimeFrameItem(SerializationInfo info, StreamingContext context)
        {
            TimeFrame = info.GetInt32("TimeFrame");
            Comment = info.GetString("Comment");

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("TimeFrame", TimeFrame);
            info.AddValue("Comment", Comment);


        }
    }
}
