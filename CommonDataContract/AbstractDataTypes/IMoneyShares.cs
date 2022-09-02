using System.Collections.Generic;

namespace CommonDataContract.AbstractDataTypes
{
    /// <summary>
    /// Получение информации о денежных средствах по акциям
    /// </summary>
    public interface IMoneyShares
    {

        /// <summary>
        /// Набор допольнительных параметров для конкретного темринала, для доступа в роботе
        /// </summary>
        Dictionary<string, object> DicValue { get; }
        /// <summary>
        /// Код клиента
        /// </summary>
        string ClientCode{ get;}

        /// <summary>
        /// Счет
        /// </summary>
        string Account { get; }

        ///// <summary>
        ///// Счет депо, требуется в основном для квика, т.к. клиентские коды могут быть одинаковыми
        ///// </summary>
        //string Account { get; }

        /// <summary>
        ///Вид лимита. Значение «Tx» соответствует позиции клиента после совершения всех расчетов
        /// </summary>
        string LimitKind { get; }

        /// <summary>
        ///Сумма собственных средств клиента до совершения операций
        /// </summary>
        double OpenBalance { get; }

        /// <summary>
        /// Итоговый баланс клиента с учетом свободных средств и залоченных в ордерах
        /// </summary>
        double TotalBalance { get; }

        /// <summary>
        ///Регистр учета (Идентификатор торговой сессии, в которой ведется лимит, например EQTV – Фондовая Московская биржа)
        /// </summary>
        string Group { get; }
        ///// <summary>
        /////Разрешенная сумма заемных средств до совершения операций
        ///// </summary>
        //double OpenLimit { get; }
        ///// <summary>
        /////Сумма собственных средств клиента на текущий момент (с учетом исполненных сделок)
        ///// </summary>
        //double CurrentBalance { get;  }
        ///// <summary>
        /////Разрешенная сумма заемных средств на текущий момент (с учетом сделок)
        ///// </summary>
        //double CurrentLimit { get;  }
        ///// <summary>
        /////Сумма средств, заблокированных под исполнение заявок клиента
        ///// </summary>
        //double Locked { get; }
        ///// <summary>
        /////Сумма собственных и заемных средств. «Всего» = «Текущий остаток» + «Текущий лимит» Total=CurrentBalance+CurrentLimit
        ///// </summary>
        //double Total { get;  }
        ///// <summary>
        /////Сумма средств, доступных для заявок на покупку. «Доступно» = «Всего» - «Заблокировано» Available=Total-Locked
        ///// </summary>
        //double Available { get;  }
        /// <summary>
        ///Средства клиента после совершения сделок, за вычетом заемных средств. «Баланс» = «Всего» - «Входящий лимит» Balance=Total-OpenLimit
        /// </summary>
        double Balance { get;  }
        ///// <summary>
        /////Значение плеча, заданное при загрузке лимитов по денежным средствам
        ///// </summary>
        //double Leverage { get;  }
        /// <summary>
        /// Валюта
        /// </summary>
        string Currency { get; }
    }
}
