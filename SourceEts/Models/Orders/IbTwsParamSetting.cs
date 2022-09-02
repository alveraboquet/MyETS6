using System;

namespace SourceEts.Models.Orders
{
    public class IbTwsParamSetting
    {
        /// <summary>
        /// Модификация ордера
        /// </summary>
        public string UpdateId { get; set; }
        public string FaProfile { get; set; }

        public string TypeOrder { get; set; }
        public int ScaleInitLevelSize { get; set; }
        public double ScalePriceIncrement { get; set; }
        public int ScaleSubsLevelSize { get; set; }

        #region TRAIL

        public double TrailStopPrice { get; set; }
        public double AuxPrice { get; set; }
        public bool IsPercentOtstupMaxMin { get; set; }

        #endregion
    }
}
