using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonDataContract.AbstractDataTypes
{
    public interface IOrdersLog
    {
        long ReplRev { get; set; }
        long IdOrd { get; set; }
        int SessId { get; set; }
        int IsinId { get; set; }
        string ClassCode { get; set; }
        string Symbol { get; set; }
        int Amount { get; set; }
        int AmountRest { get; set; }
        long IdDeal { get; set; }
        int Status { get; set; }
        double Price { get; set; }
        DateTime Moment { get; set; }
        int Dir { get; set; }
        int Action { get; set; }
        double DealPrice { get; set; }
    }
}
