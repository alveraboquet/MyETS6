using FTX.Net.Enums;
using FTX.Net.Objects.Models;

namespace CryptoCon.FTX.Models
{
    public class Order : FTXOrder
    {
        public OrderSide OrderSide { get; set; }
        public OrderType OrderType { get; set; }
        public string ClientOrderId { get; set; }
    }
}
