using System;

namespace SourceEts.Models
{
    public interface ISymbolDetailModel
    {
        string TypeFormat { get; set; }
        string NamePlaceStorages { get; set; }
        string ShortName { get; set; }
        string TypeInstrument { get; set; }
        string Symbol { get; set; }
        string ClassCode { get; set; }
        double MinStep { get; set; }
        double PointCost { get; set; }
        double LotSize { get; set; }
        int Accuracy { get; set; }
        double GoForTest { get; set; }
        int Qoutation { get; set; }
        double Comission { get; set; }
        int Slipping { get; set; }
        bool IsPercnetsComission { get; set; }

    }
}
