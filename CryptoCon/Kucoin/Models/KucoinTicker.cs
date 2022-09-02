namespace CryptoCon.Kucoin.Models
{
    public class KucoinTicker
    {
        public string Symbol { get; set; }
        public string QuoteAsset { get; set; }
        public string BaseAsset { get; set; }
        public decimal LotSize { get; set; }
        public decimal LotSizeAccuracy { get; set; }
        public decimal PriceSize { get; set; }
        public decimal PriceSizeAccuracy { get; set; }
        public bool Status { get; set; }
    }
}
