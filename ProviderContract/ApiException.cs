using System;

namespace ProviderContract.Exceptions
{
    /// <summary>
    /// Ошибка коннектора
    /// </summary>
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }
    }

    /// <summary>
    /// Попытка вызова приватного метода без ключей
    /// </summary>
    public class ApiAccessException : ApiException
    {
        public ApiAccessException(string message) : base(message) { }
    }
}
