using Bitfinex.Net.Objects.Models.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCon.Bitfinex.Models
{
    public class Assets : BitfinexSymbolDetails
    {
        public string Symbol { get; set; }
    }
}
