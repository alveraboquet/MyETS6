using Bitfinex.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCon.FTX.Models
{
    public class CancelOrderEntity
    {
        public long OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string ClientOrderId { get; set; }
        public string Symbol { get; set; }
        public DateTime? CreateOrderTime { get; set; }
        public DateTime? CancelOrderTime { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public OrderSide Side { get; set; }
        public decimal QuantityRemaining { get; set; }
    }
}
