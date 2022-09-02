using System;

namespace CommonDataContract.AbstractDataTypes
{
    /// <summary>
    /// Интерфейс для сделок
    /// </summary>
    public interface IDeals
    {
        /// <summary>
        /// Дата время
        /// </summary>
        DateTime DateTrade { get; set; }
        /// <summary>
        /// Номер заявки
        /// </summary>
        string Order { get; set; }
        /// <summary>
        /// Номер сделки
        /// </summary>
        string NumberTrade { get; set; }
        /// <summary>
        /// Код класса
        /// </summary>
        String ClassCode { get; set; }
        /// <summary>
        /// Код инструмента
        /// </summary>
        String Symbol { get; set; }
        /// <summary>
        /// Операция покупка или продажа
        /// </summary>
        String Operation { get; set; }
        /// <summary>
        /// Счет
        /// </summary>
        String Account { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        Double Price { get; set; }
        /// <summary>
        /// Количество
        /// </summary>
        double Quantity { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        String Comment { get; set; }
        /// <summary>
        /// Код клиента
        /// </summary>
        String ClientCode { get; set; }
        /// <summary>
        /// Объем
        /// </summary>
        Double Volume { get; set; }
    }
}
