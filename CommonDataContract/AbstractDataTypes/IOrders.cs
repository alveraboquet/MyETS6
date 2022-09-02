using System;

namespace CommonDataContract.AbstractDataTypes
{
    /// <summary>
    /// Интерфейсы для заявок
    /// </summary>
    public interface IOrders
    {
        /// <summary>
        /// Счет
        /// </summary>
        string Account { get; set; }
        /// <summary>
        /// Код инструмента
        /// </summary>
        string Symbol { get; set; }
        /// <summary>
        /// Код класса
        /// </summary>
        string ClassCode { get; set; }
        /// <summary>
        /// Номер
        /// </summary>
        string Number { get; set; }
        /// <summary>
        /// Операция ConfigTermins.Buy
        /// </summary>
        string Operation { get; set; }
        /// <summary>
        /// Баланс, количество лотов оставшееся в заявке
        /// </summary>
        double Balance { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        double Price { get; set; }
        /// <summary>
        /// Количество лотов
        /// </summary>
        double Quantity { get; set; }
        /// <summary>
        /// Статус заявки
        /// </summary>
        string Status { get; set; }
        /// <summary>
        /// Дата время выставляения заявки
        /// </summary>
        DateTime Time { get; set; }
        /// <summary>
        /// Код клиента
        /// </summary>
        string ClientCode { get; set; }
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        string Comment { get; set; }
        /// <summary>
        /// Заявка на другом сервере
        /// </summary>
        bool AnotherServer { get; set; }

        /// <summary>
        /// Время отправки транзакции на снятие
        /// </summary>
        DateTime DateTimeKill { get; set; }
    }
}
