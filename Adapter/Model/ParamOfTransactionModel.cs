using SourceEts.Models.Orders;

namespace Adapter.Model
{
    /// <summary>
    /// Параметры для отправки транзакции
    /// </summary>
    public class ParamOfTransactionModel
    {
        /// <summary>
        /// Счет
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// Код клиента
        /// </summary>
        public string ClientCode { get; set; }
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public string TrandId { get; set; }
        /// <summary>
        /// Идентификатор в IbTws, для определения по какой заявке будет выставляться связанная заявка
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// Код класса
        /// </summary>
        public string ClassCode { get; set; }
        /// <summary>
        /// Код инструмента
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// Время действия стоп-заявки
        /// </summary>
        public string DurationStop { get; set; }
        /// <summary>
        /// Дата действия стоп-заявки
        /// </summary>
        public string DurationStopDate { get; set; }
        /// <summary>
        /// Операция
        /// </summary>
        public char Operation { get; set; }
        /// <summary>
        /// Цена заявки на открытие
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Номер заявки, которую необходимо снять
        /// </summary>
        public string OrderNumberForKill { get; set; }
        /// <summary>
        /// Id заявки, которую необходимо снять
        /// </summary>
        public string OrderIdForKill { get; set; }
        ///// <summary>
        ///// Цена заявки на закрытие
        ///// </summary>
        //public double PriceClose { get; set; }
        /// <summary>
        /// Номер стоп-заявки, которую необходимо снять
        /// </summary>
        public string StopOrderNumberForKill { get; set; }
        /// <summary>
        /// Id стоп-заявки, которую необходимо снять
        /// </summary>
        public string StopOrderIdForKill { get; set; }
        /// <summary>
        /// Цена Стоп-заявки
        /// </summary>
        public double StopPrice { get; set; }
        /// <summary>
        /// Используется в стпо-заявке по другой бумаге.
        /// </summary>
        public double StopPriceSeccode { get; set; }
        /// <summary>
        /// Используется в стпо-заявке по другой бумаге.
        /// </summary>
        public double StopPriceClassCode { get; set; }
        /// <summary>
        /// Стоп цена
        /// </summary>
        public double StopPriceFilled { get; set; }
        /// <summary>
        /// Цена аткивации терллинг профит
        /// </summary>
        public double TrallingprofitPrice { get; set; }
        /// <summary>
        /// Отступ от мин/макс для терллинг профит
        /// </summary>
        public double TrallingProfiotstup { get; set; }
        /// <summary>
        /// Отступ рассчитывается в процентах или пунктах
        /// </summary>
        public string TrallingProfiotstupUnits { get; set; }
        /// <summary>
        /// Защитный спред для терллинг профит
        /// </summary>
        public double TrallingProfitSpred { get; set; }
        /// <summary>
        /// Защитный рассчитывается в процентах или пунктах
        /// </summary>
        public string TrallingProfitSpredUnits { get; set; }
        /// <summary>
        /// Количество лотов
        /// </summary>
        public double Quantity { get; set; }
        /// <summary>
        /// Тип оредар (limit order, stop order, linked order and etc.)
        /// </summary>
        public string TypeOrder { get; set; }
        ///// <summary>
        ///// Пользовательский тип ордера
        ///// </summary>
        //public string TypeOrderUser { get; set; }
        /// <summary>
        /// заявка рыночный или нет для ордера на открытие
        /// </summary>
        public string IsMarketOrder { get; set; }
        ///// <summary>
        ///// заявка рыночный или нет для ордера на открытие
        ///// </summary>
        //public string IsCloseMarketOrder { get; set; }
        /// <summary>
        /// Стоп-заявка рыночная или нет
        /// </summary>
        public string IsMarketStopOrder { get; set; }
        /// <summary>
        /// Треллинг профит рыночный или нет
        /// </summary>
        public string IsMarketTrallingProfit { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Единый брокеский счет для транзака коннектора
        /// </summary>
        public bool IsUnion { get; set; }


        public IbTwsParamSetting IbTwsOrder { get; set; }
    }



}
