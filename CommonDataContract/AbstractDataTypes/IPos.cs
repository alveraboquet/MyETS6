using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonDataContract.AbstractDataTypes
{
    public interface IPos
    {
        /// <summary>
        /// Код Бумаги
        /// </summary>
        String Symbol { get; }

        /// <summary>
        /// Код клента
        /// </summary>
        String ClientCode { get; }

        /// <summary>
        /// Баланс
        /// </summary>
        double Balance { get; }
    }
}
