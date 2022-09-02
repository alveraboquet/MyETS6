using System;

namespace SourceEts.Models.CandleVol
{
    public interface IHorizontalVol
    {
        bool IsIceberg { get; }
        double Price { get; }
        double VolBuy { get; }
        double VolSell { get; }
    }
}
