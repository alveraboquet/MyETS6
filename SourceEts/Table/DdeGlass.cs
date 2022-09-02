using System;
using System.Runtime.Serialization;

namespace SourceEts.Table
{
    /// <summary>
    /// Модель каналов для стакана
    /// </summary>
    [Serializable]
    public class DdeGlass : ISerializable
    {
        /// <summary>
        /// Тикер инструмента
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// название канала
        /// </summary>
        public string NameDdeChannelGlass { get; set; }

        public DdeGlass()
        {
        }

        public DdeGlass(SerializationInfo info, StreamingContext context)
        {
            Symbol = info.GetString("Symbol");
            NameDdeChannelGlass = info.GetString("NameDdeChannelGlass");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Symbol", Symbol);
            info.AddValue("NameDdeChannelGlass", NameDdeChannelGlass);
        }
    }
}
