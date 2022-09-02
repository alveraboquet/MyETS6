using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCon.Bitfinex.Models
{
    public class OrderBooks
    {
        public List<BidsAndAsks> Bids { get; set; }
        public List<BidsAndAsks> Asks { get; set; }
    }
}
