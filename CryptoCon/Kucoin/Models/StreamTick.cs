using System;

namespace CryptoCon.Kucoin.Models
{
    public class StreamTick
    {
        public string Symbol { get; set; }
        public decimal? LastPrice { get; set; }
        public decimal? BestAskPrice { get; set; }
        public decimal? BestBidPrice { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
