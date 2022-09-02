using CommonDataContract;
using SourceEts.Table;
using SourceEts.Terminals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SourceEts.UserConnector
{
    public class TerminalInfo : ViewModelBase, ISerializable, ITerminalInfo
    {
        #region Общие настройки


        private List<SettingUserCon> _userConSettings;

        private String _name;
        private String _path;
        private ObservableCollection<AccountsPair> _accountsPairs;
        private String _accountsString;

        private bool _isConnect;
        private bool _isUse;
        private string _comment;
        private string _terminal;
        private string _typeTerminal;
        private string _addInfo;
        private string _colorStatusConnection;

        private List<AvalibleInstrumentsModel> _avalibleInstruments;

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public List<AvalibleInstrumentsModel> AvalibleInstruments
        {
            get { return _avalibleInstruments ?? (_avalibleInstruments = new List<AvalibleInstrumentsModel>()); }
            set
            {
                if (_avalibleInstruments != value)
                {
                    _avalibleInstruments = value;
                    RaisePropertyChanged("AvalibleInstruments");
                }
            }
        }


        private List<AllInstrForTrwModel> _allInstruments;

        /// <summary>
        /// Все инструменты на бирже
        /// </summary>
        [XmlIgnore]
        public List<AllInstrForTrwModel> AllInstruments
        {
            get { return _allInstruments ?? (_allInstruments = new List<AllInstrForTrwModel>()); }
            set
            {
                if (_allInstruments != value)
                {
                    _allInstruments = value;
                    RaisePropertyChanged("AllInstruments");
                }
            }
        }

        private ObservableCollection<AllInstrForTrwModel> _allSymbolsSave;

        /// <summary>
        /// Все инструменты на бирже добавленные пользователем, например для терминала IBTWS
        /// </summary>
        public ObservableCollection<AllInstrForTrwModel> AllSymbolsSave
        {
            get { return _allSymbolsSave ?? (_allSymbolsSave = new ObservableCollection<AllInstrForTrwModel>()); }
            set
            {
                if (_allSymbolsSave != value)
                {
                    _allSymbolsSave = value;
                    RaisePropertyChanged("AllSymbolsSave");
                }
            }
        }


        /// <summary>
        /// Дополнительная информация для отображения
        /// </summary>
        [XmlIgnore]
        public string ColorStatusConnection
        {
            get { return _colorStatusConnection; }
            set
            {
                if (_colorStatusConnection != value)
                {
                    _colorStatusConnection = value;
                    RaisePropertyChanged("ColorStatusConnection");
                }
            }
        }

        /// <summary>
        /// Дополнительная информация для отображения
        /// </summary>
        [XmlIgnore]
        public string AddInfo
        {
            get { return _addInfo; }
            set
            {
                if (_addInfo != value)
                {
                    _addInfo = value;
                    RaisePropertyChanged("AddInfo");
                }
            }
        }


        /// <summary>
        /// . 
        /// </summary>
        public String Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public String Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    RaisePropertyChanged("Path");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public List<SettingUserCon> UserConSettings
        {
            get { return _userConSettings ?? (_userConSettings = new List<SettingUserCon>()); }
            set
            {
                if (_userConSettings != value)
                {
                    _userConSettings = value;
                    RaisePropertyChanged("UserConSettings");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public ObservableCollection<AccountsPair> AccountsPairs
        {
            get { return _accountsPairs ?? (_accountsPairs = new ObservableCollection<AccountsPair>()); }
            set
            {
                if (_accountsPairs != value)
                {
                    _accountsPairs = value;
                    RaisePropertyChanged("AccountsPairs");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public String AccountsString
        {
            get { return _accountsString; }
            set
            {
                if (_accountsString != value)
                {
                    _accountsString = value;
                    RaisePropertyChanged("AccountsString");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        [XmlIgnore]
        public bool IsConnect
        {
            get { return _isConnect; }
            set
            {
                if (_isConnect != value)
                {
                    _isConnect = value;
                    RaisePropertyChanged("IsConnect");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public bool IsUse
        {
            get { return _isUse; }
            set
            {
                if (_isUse != value)
                {
                    _isUse = value;
                    RaisePropertyChanged("IsUse");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        [XmlIgnore]
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    RaisePropertyChanged("Comment");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public string Terminal
        {
            get { return _terminal; }
            set
            {
                if (_terminal != value)
                {
                    _terminal = value;
                    RaisePropertyChanged("Terminal");
                }
            }
        }

        /// <summary>
        /// Поставщик, Транзакции или Поставщик/транзакции
        /// </summary>
        public string TypeTerminal
        {
            get { return _typeTerminal; }
            set
            {
                if (_typeTerminal != value)
                {
                    _typeTerminal = value;
                    RaisePropertyChanged("TypeTerminal");
                }
            }
        }



        [XmlIgnore]
        public double TotalSecond { get; set; }
        [XmlIgnore]
        public int CurCounTransaction { get; set; }
        #endregion


        public TerminalInfo() { }

        protected TerminalInfo(SerializationInfo info, StreamingContext context)
        {
            Name = (string)info.GetValue("Name", typeof(string));
            Path = (string)info.GetValue("Path", typeof(string));
            AccountsString = (string)info.GetValue("AccountsString", typeof(string));
            AccountsPairs = (ObservableCollection<AccountsPair>)info.GetValue("AccountsPairs", typeof(ObservableCollection<AccountsPair>));
            AllSymbolsSave = (ObservableCollection<AllInstrForTrwModel>)info.GetValue("AllSymbolsSave", typeof(ObservableCollection<AllInstrForTrwModel>));

            IsConnect = info.GetBoolean("IsConnect");
            IsUse = info.GetBoolean("IsUse");
            Terminal = info.GetString("Terminal");
            TypeTerminal = info.GetString("TypeTerminal");


        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", _name);
            info.AddValue("Path", _path);
            info.AddValue("AccountsString", AccountsString);
            info.AddValue("AccountsPairs", AccountsPairs);
            info.AddValue("AllSymbolsSave", AllSymbolsSave);

            info.AddValue("IsConnect", IsConnect);
            info.AddValue("IsUse", IsUse);
            info.AddValue("Terminal", Terminal);
            info.AddValue("TypeTerminal", TypeTerminal);


        }

    }
}
