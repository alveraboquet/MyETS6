using Bitfinex.Net.Objects.Models.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCon.Bitfinex.Models
{
    public class BitfinexTickerModel : BitfinexStreamSymbolOverview
    {
        public string Symbol { get; set; }
    }
}
