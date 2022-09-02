using System;

namespace SourceEts.Table
{
    /// <summary>
    /// Котировки стакана
    /// </summary>
    public interface IGlassQuotation
    {

        /// <summary>
        /// Количество лотов в заявке на покупку
        /// </summary>
        double BuyQty { get; }
        /// <summary>
        /// Количество лотов в заявке на продажу
        /// </summary>
        double SellQty { get; }
        /// <summary>
        /// Цена 
        /// </summary>
        double Price { get; }

        ///// <summary>
        ///// 
        ///// </summary>
        //IHorizontalVol Detail { get; }

    }
}
