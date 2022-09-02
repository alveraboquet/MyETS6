using System;

namespace SourceEts.Robot
{
    public interface IMoveOrder
    {
        #region Base

        /// <summary>
        /// Тип заявки на открытие позиции (лимитная или рыночная)
        /// </summary>
        string OrderType { get; }
        /// <summary>
        /// Проскальзываие для заявки на открытие.. Используется только в случае лимитоной заявки.
        /// </summary>
        double SlippageInitialorder { get; }
        /// <summary>
        /// Если заявка не исполнилась за указанное время, то выполнить следующие условия
        /// </summary>
        bool IsNotPerfomedOrder { get; }
        /// <summary>
        /// Время ожидания исполнения условий по заявки
        /// </summary>
        int PeriodWaitOrder { get; }
        /// <summary>
        /// снять заявку
        /// </summary>
        bool IsKillOrder { get; }
        /// <summary>
        /// Перевыставить заявку с новыми параметрами на остаток
        /// </summary>
        bool IsMoveOrder { get; }
        /// <summary>
        /// Исполнить по рынку
        /// </summary>
        bool IsMarketPerfomedOrder { get; }
        /// <summary>
        /// Цена, от которой следует рассчитать цену перевыставляемой заявки
        /// </summary>
        string PriceMoveOrderType { get; }
        /// <summary>
        /// Проскальзывание для перевыставления
        /// </summary>
        double SlippageSecondorder { get; }
        /// <summary>
        /// Количество перевыставлений
        /// </summary>
        int CountMoveOrder { get; }
        /// <summary>
        /// Интервал перевыставлений в секундах
        /// </summary>
        int PeriodMoveOrder { get; }
        /// <summary>
        /// Исполнить по рынку на последнем перевыставлении
        /// </summary>
        bool IsMarketOnLastMoveOrder { get; }
        /// <summary>
        /// Снять выставленный ордер, если цена отколонислась на больше или равное количество шагов цены от цены выставления.
        /// </summary>
        bool IsKillIfPriceOrder { get; }
        /// <summary>
        /// Отклонение в шагах, при котором будет снята заявка
        /// </summary>
        double MinStepForKillOrder { get; }
        /// <summary>
        /// Заявки совершаемые на закрытии или открытии свечи
        /// </summary>
        bool IsOrderCloseCandleOrder { get; }
        /// <summary>
        /// Количество секунд, за которое следует исполнить заявку на закрытии свечи
        /// </summary>
        int BeforeCloseCandleOrder { get; }
        /// <summary>
        /// Количество секунд, после октрытия свечи, после этого отправить заявку на открытие позиции
        /// </summary>
        int AfterCloseCandleOrder { get; }
        /// <summary>
        /// Количество свечей ожидания исполнения заявки. 0 - не работает, 1 - текущая свеча и т.д.
        /// </summary>
        int CandleWaitPerfomedForKillOrder { get; }
        /// <summary>
        /// Использовать снятие заявки по свечам
        /// </summary>
        bool IsCandleWaitPerfomedForKillOrder { get; }

        #endregion

        #region Additional
        /// <summary>
        /// Физически выставлять отложенные заявки
        /// </summary>
        bool IsPhysicalOrder { get; }
        /// <summary>
        /// Применять базовые условия после частичного лимитной заявки на открытие позиции
        /// </summary>
        bool IsLimitOrderOrder { get; }
        /// <summary>
        /// Стоп-лимит. Начать использовать основные настройки, после активации стоп-заявки и выставлении заявки в терминал.
        /// </summary>
        bool IsStopLimiOrder { get; }
        /// <summary>
        /// Треллинг-профит. Начать использовать основные настройки, после активации стоп-заявки и выставлении заявки в терминал.
        /// </summary>
        bool IsTrellingProfitOrder { get; }

        #endregion

    }
}
