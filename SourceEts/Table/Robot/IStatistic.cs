using System;

namespace SourceEts.Robot
{
    /// <summary>
    /// Статистика по стартегии
    /// </summary>
    public interface IStatistic
    {
        /// <summary>
        /// Капитал, с учетом текущей позиции и совершенных сделок
        /// </summary>
        double Equity { get; }

        /// <summary>
        /// Начальные денежные средства при расчете стартегии
        /// </summary>
        double Capital { get; }

        /// <summary>
        /// Денежные средства в позиции
        /// </summary>
        double FreeMoney { get; }

        /// <summary>
        /// Общая прибыль
        /// </summary>
        double NetProfitLoss { get; }

        /// <summary>
        /// реальная общая прибыль
        /// </summary>
        double RealNetProfitLoss { get; }


        //#region Дневные показатели статистики

        ///// <summary>
        ///// Прибыль убыток за день
        ///// </summary>
        //double ProfitLossDay { get;  }
        ///// <summary>
        ///// Количество сделок за день
        ///// </summary>
        //int CountDealDay { get;  }
        ///// <summary>
        ///// Количество проигрышных сделок за день
        ///// </summary>
        //int CountLossDealDay { get; }
        ///// <summary>
        ///// Максимальная серия проигрышных сделок подряд за день
        ///// </summary>
        //int CountLossContractDealDay { get;}

        //#endregion


    }
}
