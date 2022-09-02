using System;

namespace CommonDataContract.AbstractDataTypes
{
    public interface IPositionFutures: IPos
    {
        ///// <summary>
        ///// Компания
        ///// </summary>
        //String Company { get; }

        ///// <summary>
        ///// Дата экспирации
        ///// </summary>
        //DateTime DataExpiration { get;  }
        ///// <summary>
        ///// Входящие длинные позиции
        ///// </summary>
        //int EnterLongPos { get;  }
        ///// <summary>
        ///// Входящие короткие позиции
        ///// </summary>
        //int EnterShortPos { get;  }
        /// <summary>
        /// Входящие чистые позиции
        /// </summary>
        double EnterEmptyPos { get; }
        /// <summary>
        /// Текущие длинные позиции
        /// </summary>
        double CurrentLongPos { get; }
        /// <summary>
        /// Текущие короткие позиции
        /// </summary>
        double CurrentShortPos { get; }

        /// <summary>
        /// В заявках на покупку
        /// </summary>
        double ActiveBuy { get; }
        /// <summary>
        /// В заявках на продажу
        /// </summary>
        double ActiveSell { get; }

        //double MarkCurrentEmptyPos { get;  }
        ///// <summary>
        ///// Плановые чистые позиции
        ///// </summary>
        //double PlanEmptyPos { get;  }
        /// <summary>
        /// Вариационная маржа
        /// </summary>
        double VariableMarga { get;  }
        ///// <summary>
        ///// эффективная цена позиции
        ///// </summary>
        //double EffectPricePos { get;  }
        ///// <summary>
        ///// Стоимость опзиции
        ///// </summary>
        //double CostPos { get;  }
    }
}
