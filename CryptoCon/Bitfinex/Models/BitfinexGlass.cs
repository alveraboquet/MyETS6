using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCon.Bitfinex.Models
{
    public class BitfinexGlass
    {
        public string Symbol { get; set; }
        public string Action { get; set; }

        public SortedDictionary<decimal, decimal> DictionaryAskGlass = new SortedDictionary<decimal, decimal>();
        public SortedDictionary<decimal, decimal> DictionaryBidGlass = new SortedDictionary<decimal, decimal>();

        public SortedDictionary<decimal, decimal> DictionaryUpdateBidGlass = new SortedDictionary<decimal, decimal>();
        public SortedDictionary<decimal, decimal> DictionaryUpdateAskGlass = new SortedDictionary<decimal, decimal>();

        public DateTime Timestamp { get; set; }
    }
}
