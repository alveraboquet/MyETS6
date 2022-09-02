using System.Collections.Generic;

namespace ProviderContract.Data.Entities
{
    /// <summary>
    /// Книга ордеров (Стакан)
    /// </summary>
    public class OrderBook
    {
        public OrderBook()
        {
            Asks = new List<OrderBookEntry>();
            Bids = new List<OrderBookEntry>();
        }

        public string ClassCode { get; set; }
        /// <summary>
        /// Торговый инструмент (стандартный формат)
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Временная метка для снапшота
        /// </summary>
        public long LastUpdateId { get; set; }

        /// <summary>
        /// Заявки на продажу
        /// </summary>
        public List<OrderBookEntry> Asks { get; set; }
        /// <summary>
        /// Заявки на покупку
        /// </summary>
        public List<OrderBookEntry> Bids { get; set; }
    }
}
