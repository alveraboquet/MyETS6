using Binance.Net.Enums;

namespace CryptoCon.Binance.Models
{
    public class ExchangeInfo
    {
        public string Symbol { get; set; }
        public string QuoteAsset { get; set; }
        public string BaseAsset { get; set; }
        public decimal TickSize { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal MinPrice { get; set; }
        public SymbolStatus Status { get; set; }
    }
}
