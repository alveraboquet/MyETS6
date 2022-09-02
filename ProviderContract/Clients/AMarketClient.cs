namespace ProviderContract.Clients
{
    /// <summary>
    /// Общий клиент для типа рынка
    /// </summary>
    public abstract class AMarketClient
    {
        /// <summary>
        /// Клиент, отправляющий данные по запросу
        /// </summary>
        public AApiClient Api { get; set; }
        /// <summary>
        /// Клиент, отправляющий данные в реальном времени
        /// </summary>
        public AStreamClient Stream { get; set; }
        public ApiType Type { get; set; }
        public bool IsDemo { get; set; }
    }
}
