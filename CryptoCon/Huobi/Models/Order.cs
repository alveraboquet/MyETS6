using Huobi.Net.Enums;

namespace CryptoCon.Huobi.Models
{
    public class Order
    {
        public long OrderId { get; set; }
        public string? Symbol { get; set; }
        public OrderSide OrderSide { get; set; }
        public OrderType OrderType { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ClientOrderId { get; set; }
    }
}
