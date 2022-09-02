using System;
using System.Collections.Generic;

namespace ProviderContract.Data.Entities
{
    /// <summary>
    /// Свеча
    /// </summary>
    public class Kline
    {
        /// <summary>
        /// Цена закрытия периода
        /// </summary>
        public double Close { get; set; }
        /// <summary>
        /// Максимальная цена периода
        /// </summary>
        public double High { get; set; }
        /// <summary>
        /// Минимальная цена периода
        /// </summary>
        public double Low { get; set; }
        /// <summary>
        /// Название рынка (e.g. Exchange Spot)
        /// </summary>
        public string ClassCode { get; set; }
        /// <summary>
        /// Цена открытия периода
        /// </summary>
        public double Open { get; set; }
        /// <summary>
        /// Время начала периода (секунды)
        /// </summary>
        public long OpenTime { get; set; }
        /// <summary>
        /// Торговый инструмент (стандартный формат)
        /// </summary>
        public string Pair { get; set; }
        /// <summary>
        /// Объем торгов за период
        /// </summary>
        public double Volume { get; set; }
    }

    public class KlineData
    {
        public Kline Kline { get; set; }
        public double[] Volumes { get; set; }
    }
}
