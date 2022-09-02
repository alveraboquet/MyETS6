using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommonDataContract.AbstractDataTypes;
using SourceEts.Models.TimeFrameTransformModel;

namespace Adapter.Logic.LogicServerHistory
{
    /// <summary>
    /// Класс созданный для проверки подписи на котировки, тики и получения данных с сервреа
    /// </summary>
    public class QouteSubscribe
    {
        public List<string> ColSubscribeQuote = new List<string>(); //коллекция инструментов, на которые подписаны
        public List<InstrumentsTicks> ColInstrumentTick = new List<InstrumentsTicks>(); //коллекция инструментов, на которые нужны тики
        public List<InstrumentTimeFrameSubscribe> ColInstrumentTimeFrame = new List<InstrumentTimeFrameSubscribe>(); //коллекция инструментов с типами интервалов, на которые уже посылался запрос на исторические дынные
        public ObservableCollection<Candles> CandlesItems = new ObservableCollection<Candles>();
        /// <summary>
        /// Ссылка на таблицу текущих параметров
        /// </summary>
        public List<ISecurity> Securities = new List<ISecurity>();
    }

    /// <summary>
    /// Класс подписанных на тики инструментов
    /// </summary>
    public class InstrumentsTicks
    {
        private string _symbol;

        /// <summary>
        /// инструмент
        /// </summary>
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                if (_symbol != value)
                {
                    _symbol = value;
                }
            }
        }


        public string ClassCode { get; set; }
        /// <summary>
        /// Загрузка тиков по текущий момент завершена
        /// </summary>
        public bool IsLoad { get; set; }
        /// <summary>
        /// Время отправки запроса на получение тиков
        /// </summary>
        public DateTime LoadTimeReqest { get; set; }

        public ISecurity Sec { get; set; }


        private long _lastNumberTick;

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
                }
            }
        }
    }


}
