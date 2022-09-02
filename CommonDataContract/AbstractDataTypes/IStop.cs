using System;

namespace CommonDataContract.AbstractDataTypes
{
    public interface IStop
    {
        /// <summary>
        /// Номер стоп-заявки
        /// </summary>
        string Number { get; set; }
        /// <summary>
        /// Время выставления
        /// </summary>
        DateTime Time { get; set; }
        /// <summary>
        /// Тип стоп-заявки
        /// </summary>
        String TypeStop { get; set; }
        /// <summary>
        /// Код инструмента
        /// </summary>
        String Symbol { get; set; }
        /// <summary>
        /// Код класса
        /// </summary>
        String ClassCode { get; set; }
        /// <summary>
        /// Аккаунт
        /// </summary>
        String Account { get; set; }
        /// <summary>
        /// Операция
        /// </summary>
        String Operation { get; set; }
        /// <summary>
        /// Стоп-цена
        /// </summary>
        Double StopPrice { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        Double Price { get; set; }
        /// <summary>
        /// Количество
        /// </summary>
        double Quantity { get; set; }
        /// <summary>
        /// Баланс
        /// </summary>
        double Balance { get; set; }
        ///// <summary>
        ///// Исполнено
        ///// </summary>
        //double FilledQuantity { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        String Comment { get; set; }
        /// <summary>
        /// Номер заявки
        /// </summary>
        string NumberOrder { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        String Status { get; set; }
        /// <summary>
        /// Результат (ответ системы)
        /// </summary>
        String Result { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Код клиета
        /// </summary>
        String ClientCode { get; set; }

        /// <summary>
        /// цена активации треллинг профит, используется только когда используется тейк-профит и стоп-лимит
        /// </summary>
        double TpActivationPrice { get; set; }
        /// <summary>
        /// Отступ от мин/макс
        /// </summary>
        double OtstupMaxMin { get; set; }
        /// <summary>
        /// Отступ установлен в процентах, иначе в шагах цены
        /// </summary>
        bool IsPercentOtstupMaxMin { get; set; }
        /// <summary>
        /// Спред
        /// </summary>
        double Spread { get; set; }
        /// <summary>
        /// Spread установлен в процентах, иначе в шагах цены
        /// </summary>
        bool IsPercentSpread { get; set; }

        /// <summary>
        /// Заявка на другом сервере
        /// </summary>
        bool AnotherServer { get; set; }

        /// <summary>
        /// Время отправки транзакции на снятие стоп-заявки
        /// </summary>
        DateTime DateTimeKill { get; set; }

        /// <summary>
        /// Количество попыток снять стоп-заявку, если не получается, то она считается на другом сервере, используется для РМ
        /// </summary>
        int KillCount { get; set; }

    }
}
