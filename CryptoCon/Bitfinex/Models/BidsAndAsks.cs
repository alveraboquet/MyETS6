using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCon.Bitfinex.Models
{
    public class BidsAndAsks
    {
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public long Id { get; set; }
    }
}
