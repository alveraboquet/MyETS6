using System;

namespace ProviderContract.Data.Enums
{
    /// <summary>
    /// Тип заявки
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Лимитная
        /// </summary>
        LIMIT = 1,
        /// <summary>
        /// Рыночная
        /// </summary>
        MARKET = 2
    }
}
