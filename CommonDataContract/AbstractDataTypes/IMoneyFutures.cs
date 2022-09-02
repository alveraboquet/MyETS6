using System;

namespace CommonDataContract.AbstractDataTypes
{
    /// <summary>
    /// Получение информации о денежных средствах по фьючерсам
    /// </summary>
    public interface IMoneyFutures
    {
        /// <summary>
        /// Идентификатор фирмы-дилера в торговой системе
        /// </summary>
        String Account { get;  }
        /// <summary>
        /// Тип лимита для рынка FORTS: «Ден.средства» - стоимость денежных средств в обеспечении, 
        /// </summary>
        String TypeLimit { get;  }
        /// <summary>
        ///Лимит открытых позиций по всем инструментам предыдущей торговой сессии в денежном выражении
        /// </summary>
        double LastLimitOpenPosition { get;  }
        /// <summary>
        /// Текущий лимит открытых позиций по всем инструментам в денежном выражении Для рынка RTS Standard отображается лимит на покупку спот-активов
        /// </summary>
        double LimitOpenPosition { get;  }
        /// <summary>
        /// Совокупное денежное обеспечение, резервируемое под открытые позиции и торговые операции текущей сессии. Для рынка RTS Standard учитываются только позиции по главным спот-активам*
        /// </summary>
        double CurrentEmptyPosition { get;  }
        /// <summary>
        /// Величина гарантийного обеспечения, зарезервированного под активные заявки, в денежном выражении
        /// </summary>
        double CurrentEmptyOrder { get;  }
        /// <summary>
        /// Величина гарантийного обеспечения, зарезервированного под открытые позиции, в денежном выражении
        /// </summary>
        double CurrentEmptyOpen { get;  }
        /// <summary>
        /// Планируемые чистые позиции по всем инструментам в денежном выражении Соответствует параметру «Свободные средства» рынка FORTS.
        /// </summary>
        double PlanEmptyPosition { get;  }
        /// <summary>
        /// Вариационная маржа по позициям клиента, по всем инструментам
        /// </summary>
        double VariableMarga { get;  }
        /// <summary>
        /// Накопленный доход на клиентском счете, рассчитываемый для операций со срочными контрактами
        /// </summary>
        double Dohod { get;  }
        /// <summary>
        /// Сумма, взимаемая биржевым комитетом за проведение биржевых сделок. Параметр рынка FORTS.
        /// </summary>
        double ExchangeFee { get;  }
        

    }
}
