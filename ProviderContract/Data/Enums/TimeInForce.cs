using System;

namespace ProviderContract.Data.Enums
{
    /// <summary>
    /// Стратегия снятия заявки
    /// </summary>
    public enum TimeInForce
    {
        /// <summary>
        /// Другое
        /// </summary>
        OTHER = 0,
        /// <summary>
        /// До снятия пользователем
        /// </summary>
        GTC = 1,
        /// <summary>
        /// Немедленное исполнение, иначе отмена остатка
        /// </summary>
        IOC = 2,
        /// <summary>
        /// Немедленное исполнение или отмена полностью
        /// </summary>
        FOK = 3
    }
}
