using System;

namespace SourceEts.Robot
{
    public interface IService
    {
        /// <summary>
        /// Время сервера
        /// </summary>
        DateTime ServerTime { get; }

        /// <summary>
        /// реальная торговля
        /// </summary>
        bool IsRealAction { get; }
        ///// <summary>
        ///// Количество свечей отображаемое на графике
        ///// </summary>
        //int CandleCountVisibleOnChart { get; }
        ///// <summary>
        ///// Точное тестирование
        ///// </summary>
        //bool IsExactTesting { get; }

        /// <summary>
        /// Лонг, Шорт, Лонг и шорт
        /// </summary>
        string TypeTorg { get; }
        /// <summary>
        /// 
        /// </summary>
        string Account { get; }
        /// <summary>
        /// 
        /// /// </summary>
        string ClientCode { get; }

        /// <summary>
        /// тип таймфрейма (секунда, минута, день, неделя, месяц)
        /// </summary>
        string TypeTimeFrame { get; }

        /// <summary>
        /// инетрвал таймфрейма
        /// </summary>
        int TimeFrame { get; }

        ///// <summary>
        ///// сжимать таймфрейм
        ///// </summary>
        //bool IsDecompress { get; }

        /// <summary>
        /// источник типа таймфрейма для сжатия
        /// </summary>
        string TypeTimeFrameSource { get; }

        /// <summary>
        /// источник интервала таймфрейма для сжатия
        /// </summary>
        int TimeFrameSource { get; }


        /// <summary>
        /// тип робота ручной или полуавтоматический
        /// </summary>
        bool IsTypeRobotAdviser { get; }

        /// <summary>
        /// идет сейчас предторговая эмуляция сделок или нет. 
        /// </summary>
        bool IsPretradeEmulation { get; }

        /// <summary>
        /// Настройки изменены
        /// </summary>
        bool IsParamChange { get; }

        /// <summary>
        /// Если открытие позиции идет по свече за несколько секунд до ее закрытия
        /// </summary>
        bool IsOpenByCloseCandleBeforeAnySeconds { get; }

        /// <summary>
        /// Если закрытие позции идет по свече за несколько секунд до ее закрытия
        /// </summary>
        bool IsCloseByCloseCandleBeforeAnySeconds { get; }

        //OrdersActiveDeactive OpenOrdersParam { get; }
        //OrdersActiveDeactive CloseOrdersParam { get; }
    }
}
