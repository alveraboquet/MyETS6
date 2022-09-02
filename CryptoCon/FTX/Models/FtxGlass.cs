using System;
using System.Collections.Generic;

namespace CryptoCon.FTX.Models
{
    public class FtxGlass
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
