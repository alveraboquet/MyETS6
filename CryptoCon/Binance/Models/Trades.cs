using System;

namespace CryptoCon.Binance.Models
{
    public class Trades
    {
        public string Symbol { get; set; }
        public long Id { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public bool IsSell { get; set; }
        public DateTime TradeTime { get; set; }
    }
}
