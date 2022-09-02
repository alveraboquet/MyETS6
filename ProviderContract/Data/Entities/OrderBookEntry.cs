using System;

namespace ProviderContract.Data.Entities
{
    /// <summary>
    /// Запись книги ордеров
    /// </summary>
    public class OrderBookEntry
    {
        /// <summary>
        /// Цена
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Объем
        /// </summary>
        public double Quantity { get; set; }
    }
}
