using System.Collections.Generic;

namespace SourceEts.Models.CandleVol
{
    public interface IHorizonalVols
    {
        List<IHorizontalVol> HorizontalVolumes { get; }
    }
}
