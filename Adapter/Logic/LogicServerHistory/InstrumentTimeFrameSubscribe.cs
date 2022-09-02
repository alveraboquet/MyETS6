namespace Adapter.Logic.LogicServerHistory
{
    /// <summary>
    /// Класс для учета информации по каким инструментам получили данные по свечам, чтоб еще раз не запрашивать
    /// </summary>
    public class InstrumentTimeFrameSubscribe
    {
        public string Symbol { get; set; }
        public string ClassCode { get; set; }
        public int Interval { get; set; }
        public string TypeInterval { get; set; }

        /// <summary>
        /// Загружена или нет история по данному инструменту
        /// </summary>
        public bool LoadHisory { get; set; }
    }
}
