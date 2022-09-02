using SourceEts.Table.CandleVol;
using System;
using System.Collections.Generic;

namespace SourceEts.BaseClass
{
    /// <summary>
    /// Информация о свечах. Содежит серии каждого из параметров свечи
    /// </summary>
    public interface ICandlesSeries
    {
        /// <summary>
        /// Количество свечей
        /// </summary>
        int CandleCount { get; }
        
        /// <summary>
        /// High
        /// </summary>
        List<double> HighSeries { get; }
        /// <summary>
        /// Время
        /// </summary>
        List<DateTime> DateTimeCandle { get; }
        /// <summary>
        /// Low
        /// </summary>
        List<double> LowSeries { get; }
        /// <summary>
        /// Open
        /// </summary>
        List<double> OpenSeries { get; }
        /// <summary>
        /// Close
        /// </summary>
        List<double> CloseSeries { get; }
        /// <summary>
        /// (high+low)/2
        /// </summary>
        List<double> MedianSeries { get; } //(h+l)/2
        /// <summary>
        /// (high+low+close)/3
        /// </summary>
        List<double> TypicalSeries { get; }//(h+l+c)/3
        /// <summary>
        /// Объем
        /// </summary>
        List<double> Volume { get; }//объем
        List<double> VolBuy { get; }//объем
        List<double> VolSell { get; }//объем 
        /// <summary>
        /// Открытый интерес для срочного рынка
        /// </summary>
        List<int> Oi { get; }//открытый интерес

        /// <summary>
        /// Кластерные свечи по времени
        /// </summary>
        List<ClasterCandleModel> ClasterTimeSeries { get; }
    }
}
