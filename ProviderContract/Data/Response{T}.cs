namespace ProviderContract.Data
{
    /// <summary>
    /// Ответ сервера
    /// </summary>
    /// <typeparam name="T">Тип данных содержимого</typeparam>
    public class Response<T>
    {
        /// <summary>
        /// Содержимое ответа
        /// </summary>
        public T Content { get; set; }
        /// <summary>
        /// Текст ошибки
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// Индикатор ошибки
        /// </summary>
        public bool HasResponseError { get; set; }
        /// <summary>
        /// Исходный Json
        /// </summary>
        public string RawJson { get; set; }
    }
}
