using System;
using System.Collections.ObjectModel;

namespace SourceEts.Table
{
    /// <summary>
    /// Котировки стакана
    /// </summary>
    public interface IGlass
    {
        /// <summary>
        /// Стакан 
        /// </summary>
        ObservableCollection<IGlassQuotation> QuotationsFull { get; }

        DateTime LasTimeUpdateUtc { get; }

    }
}
