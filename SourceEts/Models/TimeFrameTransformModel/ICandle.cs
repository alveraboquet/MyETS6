using System;

namespace SourceEts.Models.TimeFrameTransformModel
{

    /// <summary>
    /// Интерфейс свечи
    /// </summary>
    public interface ICandle
    {
        /// <summary>
        /// Время начала формирования свечи
        /// </summary>
        DateTime TradeDateTime { get; }
        /// <summary>
        /// High свечи
        /// </summary>
        double High { get; }
        /// <summary>
        /// Low свечи
        /// </summary>
        double Low { get; }
        /// <summary>
        /// Open свечи
        /// </summary>
        double Open { get; }
        /// <summary>
        /// Close свечи
        /// </summary>
        double Close { get; }
        /// <summary>
        /// Общий объем
        /// </summary>
        double Volume { get; }
        /// <summary>
        /// Объем на покупку
        /// </summary>
        double VolBuy { get; }
        /// <summary>
        /// Объем на продажу
        /// </summary>
        double VolSell { get; }
        /// <summary>
        /// Открытый интерес
        /// </summary>
        int Oi { get; }

        ///// <summary>
        ///// номер свечи в большей серии, при декомпрессии, формируется в коде  программой
        ///// </summary>
        //int NumberCandleDecompres { get; set; }

    }
}
