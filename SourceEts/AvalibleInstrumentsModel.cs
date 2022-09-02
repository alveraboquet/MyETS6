using CommonDataContract;
using CommonDataContract.AbstractDataTypes;
using SourceEts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SourceEts
{
    /// <summary>
    /// Запоминаем какие инструменты нужно отоборажать в форме робота и на какие нужна подписка на события
    /// </summary>
    [Serializable]
    public class AvalibleInstrumentsModel : ViewModelBase, ISerializable
    {
        /// <summary>
        /// Добавление инструментов
        /// </summary>
        [XmlIgnore]
        public bool IsAdd { get; set; }
        /// <summary>
        /// Удаление инструментов
        /// </summary>
        [XmlIgnore]
        public bool IsDel { get; set; }
        /// <summary>
        /// Необходим для форвардного анализа
        /// </summary>
        [XmlIgnore]
        public ISymbolDetailModel SymbolDetail { get; set; }



        [XmlIgnore]
        public ISecurity Security { get; set; }
        [XmlIgnore]
        public AvalibleInstrumentsModel Inst { get; set; }
        /// <summary>
        /// Подписка на данные была произведена
        /// </summary>
        [XmlIgnore]
        public bool IsSubscribeQoute { get; set; }

        #region private

        private string _classCode;
        private string _classCodeVisible;
        private string _symbol;
        private string _shortName;
        private bool _isSelected;
        private long _lastNumberTick;

        //для IBTWS и прочих терминалов, где нужно вводить тикеры
        private string _typeSymbol;
        private string _currency;
        private string _primaryExch;
        private string _dateExpire;
        private double _minStep;
        private double _margin;
        private string _priceQuotationString;
        private double _priceQuotation;

        //для даунлоадера
        private DateTime _lastUpdate;
        private DateTime _nextUpdate;
        private double _progressUpdate;

        #endregion




        #region public



        /// <summary>
        /// . 
        /// </summary>
        [XmlIgnore]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// Дата последнего обновления по инструменту, в источниках данных
        /// </summary>
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set
            {
                if (_lastUpdate != value)
                {
                    _lastUpdate = value;
                    RaisePropertyChanged("LastUpdate");
                }
            }
        }


        /// <summary>
        /// Дата сделующего обновления по инструменту, в источниках данных
        /// </summary>
        public DateTime NextUpdate
        {
            get { return _nextUpdate; }
            set
            {
                if (_nextUpdate != value)
                {
                    _nextUpdate = value;
                    RaisePropertyChanged("NextUpdate");
                }
            }
        }

        /// <summary>
        /// Рассчитываем сколько осталось до завершения скачивания
        /// </summary>
        /// <param name="Time"></param>
        public void SetProgress(DateTime curDate, DateTime dateStart, DateTime dateEnd)
        {
            double interval = (dateEnd - dateStart).TotalDays;
            double curInterval = (curDate - dateStart).TotalDays;
            ProgressUpdate = Math.Round(curInterval / interval * 100, 1);
            if (ProgressUpdate > 100)
                ProgressUpdate = 100;
        }

        /// <summary>
        /// Дата сделующего обновления по инструменту, в источниках данных
        /// </summary>
        [XmlIgnore]
        public double ProgressUpdate
        {
            get { return _progressUpdate; }
            set
            {
                if (_progressUpdate != value)
                {
                    _progressUpdate = value;
                    RaisePropertyChanged("ProgressUpdate");
                }
            }
        }


        /// <summary>
        /// . 
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
        /// . 
        /// </summary>
        public string ClassCodeVisible
        {
            get { return _classCodeVisible; }
            set
            {
                if (_classCodeVisible != value)
                {
                    _classCodeVisible = value;
                    RaisePropertyChanged("ClassCodeVisible");
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
        /// . 
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
        /// номер последнего тика
        /// </summary>
        public long LastNumberTick
        {
            get { return _lastNumberTick; }
            set
            {
                if (_lastNumberTick != value)
                {
                    _lastNumberTick = value;
                    RaisePropertyChanged("LastNumberTick");
                }
            }
        }

        #region IbTws

        /// <summary>
        /// тип символа
        /// </summary>
        public string TypeSymbol
        {
            get { return _typeSymbol; }
            set
            {
                if (_typeSymbol != value)
                {
                    _typeSymbol = value;
                    RaisePropertyChanged("TypeSymbol");
                }
            }
        }

        /// <summary>
        /// валюта 
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
        /// основная биржа
        /// </summary>
        public string PrimaryExch
        {
            get { return _primaryExch; }
            set
            {
                if (_primaryExch != value)
                {
                    _primaryExch = value;
                    RaisePropertyChanged("PrimaryExch");
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
        /// шаг инструмента
        /// </summary>
        public double MinStep
        {
            get { return _minStep; }
            set
            {
                if (Math.Abs(_minStep - value) > 0.0000001)
                {
                    _minStep = value;
                    RaisePropertyChanged("MinStep");
                }
            }
        }

        /// <summary>
        /// Гарантийное обеспечение
        /// </summary>
        public double Margin
        {
            get { return _margin; }
            set
            {
                if (Math.Abs(_margin - value) > 0.0000001)
                {
                    _margin = value;
                    RaisePropertyChanged("Margin");
                }
            }
        }

        /// <summary>
        /// Гарантийное обеспечение
        /// </summary>
        public double PriceQuotation
        {
            get { return _priceQuotation; }
            set
            {
                if (Math.Abs(_priceQuotation - value) > 0.0000001)
                {
                    _priceQuotation = value;
                    RaisePropertyChanged("PriceQuotation");
                }
            }
        }

        /// <summary>
        /// котировка
        /// </summary>
        public string PriceQuotationString
        {
            get { return _priceQuotationString; }
            set
            {
                if (_priceQuotationString != value)
                {
                    _priceQuotationString = value;
                    RaisePropertyChanged("PriceQuotationString");
                }
            }
        }



        #endregion

        #endregion

        public AvalibleInstrumentsModel()
        {
            Inst = this;
        }
        public AvalibleInstrumentsModel(SerializationInfo info, StreamingContext context)
        {
            ClassCode = info.GetString("ClassCode");
            LastNumberTick = info.GetInt64("LastNumberTick");
            ClassCodeVisible = info.GetString("ClassCodeVisible");
            Symbol = info.GetString("Symbol");
            ShortName = info.GetString("ShortName");
            ShortName = info.GetString("ShortName");
            LastUpdate = info.GetDateTime("LastUpdate");
            NextUpdate = info.GetDateTime("NextUpdate");
            Inst = (AvalibleInstrumentsModel)info.GetValue("Inst", typeof(AvalibleInstrumentsModel));

            TypeSymbol = info.GetString("TypeSymbol");
            Currency = info.GetString("Currency");
            PrimaryExch = info.GetString("PrimaryExch");
            DateExpire = info.GetString("DateExpire");
            MinStep = info.GetDouble("MinStep");
            Margin = info.GetDouble("Margin");
            PriceQuotationString = info.GetString("PriceQuotationString");
            PriceQuotation = info.GetDouble("PriceQuotation");

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ClassCode", ClassCode);
            info.AddValue("LastNumberTick", LastNumberTick);
            info.AddValue("ClassCodeVisible", ClassCodeVisible);
            info.AddValue("Symbol", Symbol);
            info.AddValue("ShortName", ShortName);
            info.AddValue("LastUpdate", LastUpdate);
            info.AddValue("NextUpdate", NextUpdate);
            info.AddValue("Inst", Inst);

            info.AddValue("TypeSymbol", TypeSymbol);
            info.AddValue("Currency", Currency);
            info.AddValue("PrimaryExch", PrimaryExch);
            info.AddValue("DateExpire", DateExpire);
            info.AddValue("MinStep", MinStep);
            info.AddValue("Margin", Margin);
            info.AddValue("PriceQuotationString", PriceQuotationString);
            info.AddValue("PriceQuotation", PriceQuotation);

        }
    }

    public class AvalibleInstrumentsModelComparer : IEqualityComparer<AvalibleInstrumentsModel>
    {
        public bool Equals(AvalibleInstrumentsModel? x, AvalibleInstrumentsModel? y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x?.Symbol == y?.Symbol;
        }

        public int GetHashCode(AvalibleInstrumentsModel obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;

            int hashProductName = obj.Symbol == null ? 0 : obj.Symbol.GetHashCode();

            return hashProductName;
        }
    }
}
