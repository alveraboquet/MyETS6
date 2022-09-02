using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SourceEts
{
    [Serializable]
    public class AllInstrForTrwModel : ISerializable
    {
        /// <summary>
        /// Код класса
        /// </summary>
        public string ClassCode { get; set; }
        /// <summary>
        /// Код класса
        /// </summary>
        public string ClassCodeVisible { get; set; }
        /// <summary>
        /// Выбранный класс
        /// </summary>
        public bool IsSelected { get; set; }

        public List<AvalibleInstrumentsModel> SeccodeList { get; set; }

        /// <summary>
        /// Коллекция для отображения в форме
        /// </summary>
        public ObservableCollection<AvalibleInstrumentsModel> SeccodeListSave { get; set; }
        /// <summary>
        /// Коллекция для отображения в форме
        /// </summary>
        public ObservableCollection<AvalibleInstrumentsModel> SeccodeListForForm { get; set; }

        public AllInstrForTrwModel()
        {

        }
        public AllInstrForTrwModel(SerializationInfo info, StreamingContext context)
        {

            ClassCode = info.GetString("ClassCode");
            ClassCodeVisible = info.GetString("ClassCodeVisible");
            IsSelected = info.GetBoolean("IsSelected");
            SeccodeList = (List<AvalibleInstrumentsModel>)info.GetValue("SeccodeList", typeof(List<AvalibleInstrumentsModel>));
            SeccodeListForForm = (ObservableCollection<AvalibleInstrumentsModel>)info.GetValue("SeccodeListForForm", typeof(ObservableCollection<AvalibleInstrumentsModel>));
            SeccodeListSave = (ObservableCollection<AvalibleInstrumentsModel>)info.GetValue("SeccodeListSave", typeof(ObservableCollection<AvalibleInstrumentsModel>));

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ClassCode", ClassCode);
            info.AddValue("ClassCodeVisible", ClassCodeVisible);
            info.AddValue("IsSelected", IsSelected);
            info.AddValue("SeccodeList", SeccodeList);
            info.AddValue("SeccodeListForForm", SeccodeListForForm);
            info.AddValue("SeccodeListSave", SeccodeListSave);
        }
    }
}
