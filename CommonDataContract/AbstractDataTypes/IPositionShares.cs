using System;
using System.Collections.Generic;

namespace CommonDataContract.AbstractDataTypes
{
    public interface IPositionShares: IPos
    {
        DateTime LastTimeUpdate { get;  }
        /// <summary>
        /// Название Бумаги
        /// </summary>
        String NameSymbol { get;  }

        /// <summary>
        /// Счет
        /// </summary>
        String Account { get; }

        /// <summary>
        /// Входящий остаток
        /// </summary>
        double EnterOst { get;  }
    }
}
