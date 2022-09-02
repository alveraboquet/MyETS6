using System;

namespace ProviderContract.Data.Entities
{
    public class BookTicker
    {
        public string Symbol { get; set; }
        public double BestAskPrice { get; set; }
        public double BestBidPrice { get; set; }
        public double AskQuantity { get; set; }
        public double BidQuantity { get; set; }
    }
}
