using CommonDataContract;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SourceEts.Table
{
    /// <summary>
    /// Пары счетов клиенткод и аккаунт
    /// </summary>
    [Serializable]
    public class AccountsPair : ViewModelBase, ISerializable
    {
        /// <summary>
        /// Выставляются ли стопы на сервер
        /// </summary>
        [XmlIgnore]
        public bool IsRealStop { get; set; }
        /// <summary>
        /// Являются ли счета криптоавлютными
        /// </summary>
        public bool IsCryptoExchange { get; set; }
        private String _account;
        private String _clientCode;
        private String _typeClass;//площадка. Для текущего режима ФОРТС или ММВБ
        /// <summary>
        /// Общее название
        /// </summary>
        public string AccountClientCode { get; set; }
        /// <summary>
        /// Нужно для крипто бирж (спот, маржинальный или фьючерсный)
        /// </summary>
        public string TypeAccount { get; set; }

        /// <summary>
        /// площадка. Для текущего режима ФОРТС или ММВБ
        /// </summary>
        public String TypeClass
        {
            get { return _typeClass; }
            set
            {
                if (_typeClass != value)
                {
                    _typeClass = value;
                    RaisePropertyChanged("TypeClass");

                    
                }
            }
        }

        public String Account
        {
            get { return _account; }
            set
            {
                if (_account != value)
                {
                    _account = value;
                    RaisePropertyChanged("Account");
                }
            }
        }

        public String ClientCode
        {
            get { return _clientCode; }
            set
            {
                if (_clientCode != value)
                {
                    _clientCode = value;
                    RaisePropertyChanged("ClientCode");
                }
            }
        }

        

           

        public AccountsPair()
        {
        }

        protected AccountsPair(SerializationInfo info, StreamingContext context)
        {
            Account = (string) info.GetValue("Account", typeof (string));
            ClientCode = (string) info.GetValue("ClientCode", typeof (string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Account", _account);
            info.AddValue("ClientCode", _clientCode);
        }
    }
}
