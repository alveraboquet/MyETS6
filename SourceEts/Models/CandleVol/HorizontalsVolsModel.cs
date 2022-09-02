using CommonDataContract;
using CommonDataContract.AbstractDataTypes;
using SourceEts.Models.TimeFrameTransformModel;
using System;
using System.Collections.Generic;

namespace SourceEts.Models.CandleVol
{
    public class HorizontalsVolsModel : ViewModelBase, IHorizonalVols
    {
        public ISecurity Security { get; set; }

        public void SetVol(CandleModel tick)
        {
            if (HorizontalVolumes.Count == 0)
            {
                HorizontalVolumes.Add(new HorizontalVolModel { Price = tick.Close, VolBuy = tick.VolBuy, VolSell = tick.VolSell });
            }
            else if (Security != null)
            {

                while ((tick.Close - HorizontalVolumes[HorizontalVolumes.Count - 1].Price) > CfgSourceEts.MyEpsilon)
                {
                    HorizontalVolumes.Add(new HorizontalVolModel { Price = Math.Round(HorizontalVolumes[HorizontalVolumes.Count - 1].Price + Security.MinStep, Security.Accuracy) });
                }
                while ((HorizontalVolumes[0].Price - tick.Close) > CfgSourceEts.MyEpsilon)
                {
                    HorizontalVolumes.Insert(0, new HorizontalVolModel { Price = Math.Round(HorizontalVolumes[0].Price - Security.MinStep, Security.Accuracy) });
                }

                #region для айсбергов

                int index = (int)(Math.Round((tick.Close - HorizontalVolumes[0].Price) / Security.MinStep, Security.Accuracy));
                var level = (HorizontalVolumes[index] as HorizontalVolModel);
                level.VolBuy += tick.VolBuy;
                level.VolSell += tick.VolSell;

                if (tick.VolBuy > 0)
                {
                    if (Math.Abs(level.Price - 237.51) < 0.0000001)
                    {
                    }
                    if (level.VolSellIceberg > 0)
                        level.IsIceberg = false;
                    level.VolBuyIceberg += tick.VolBuy;
                    level.OrdersBuyIceBerg = 0;
                    level.VolSellIceberg = 0;
                }
                else
                {
                    if (Math.Abs(level.Price - 237.51) < 0.0000001)
                    {
                    }
                    for (int i = index + 1; i < index + 11; i++)
                    {
                        if (i < HorizontalVolumes.Count)
                        {
                            (HorizontalVolumes[i] as HorizontalVolModel).VolSellIceberg = 0;
                            (HorizontalVolumes[i] as HorizontalVolModel).OrdersBuyIceBerg = 0;
                        }
                    }

                    if (level.VolBuyIceberg > 0)
                        level.IsIceberg = false;
                    level.VolBuyIceberg = 0;
                    level.OrdersSellIceBerg = 0;
                    level.VolSellIceberg += tick.VolSell;

                    if (Math.Abs(level.Price - 237.51) < 0.0000001 &&
                        Math.Abs(level.VolSellIceberg - 230) < 0.00000001)
                    {
                    }


                }

                level.AllTrades.Add(tick);
                //if (!level.IsUpdate)
                //{
                //    level.IsUpdate = true;
                //    HorizontalVolUpdates.Add(level);
                //} 

                #endregion
            }

            //LastTimeUpdate = tick.TradeDateTime;

        }




        private List<IHorizontalVol> _horizontalVolumes;

        /// <summary>
        /// 
        /// </summary>
        public List<IHorizontalVol> HorizontalVolumes
        {
            get { return _horizontalVolumes ?? (_horizontalVolumes = new List<IHorizontalVol>()); }
            set
            {
                if (_horizontalVolumes != value)
                {
                    _horizontalVolumes = value;
                    RaisePropertyChanged("HorizontalVolumes");
                }
            }
        }
    }
}
