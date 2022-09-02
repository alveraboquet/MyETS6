using Binance.Net.Enums;
using CryptoCon.Binance.Enums;
using System;

namespace CryptoCon.Binance.Models
{
    public class Order
    {
        public long OrderId { get; set; }
        public string? ClientOrderId { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public OrderSide OrderSide { get; set; }
        public OrderType OrderType { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityFilled { get; set; }
        public decimal QuantityRemaining => Quantity - QuantityFilled;
    }
}
