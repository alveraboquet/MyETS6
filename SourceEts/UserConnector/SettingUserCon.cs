using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CommonDataContract;

namespace SourceEts.UserConnector
{

    /// <summary>
    ///  Настройки пользовательского коннекотра
    /// </summary>
    [Serializable]
    public class SettingUserCon : ViewModelBase, ISerializable
    {
        #region Properties

        #region private

        private List<string> _valueListString;//Значение по умолчанию
        private bool _isListString;//Указывает что в текстобксе тестовые дынне. Нужен тольк в коде
        private bool _isDontSave;
        private bool _isChangeAfterConnect;
        //private bool _isEditAfterConnect;

        private bool _valueBool;//Значение по умолчанию
        private bool _isBool;//Указывает что в текстобксе тестовые дынне. Нужен тольк в коде

        private string _valueString;//Значение по умолчанию, текстовое
        private bool _isStirng;//Указывает что в текстобксе тестовые дынне. Нужен тольк в коде
        private bool _isUseInstrumentSort;//Указывает что нужно создавать список с выбора (например инструменты)
        private bool _isUseInstrumentAdd;//Указывает что нужно создавать список с выбора (например инструменты)

        private double _valueDigit;//Значение по умолчанию, текстовое
        private bool _isDigit;//Указывает что в текстобксе тестовые дынне. Нужен тольк в коде

        private string _nameParam;//Название параметра для интерфейса
        private string _description;//Описание параметра
       // private string _nameVariable;//Название переменной 

        #endregion

        #region public

        ///// <summary>
        ///// Разрешен ли данный контрол редактировать после
        ///// </summary>
        //public bool IsEditAfterConnect
        //{
        //    get { return _isEditAfterConnect; }
        //    set
        //    {
        //        if (_isEditAfterConnect != value)
        //        {
        //            _isEditAfterConnect = value;
        //            RaisePropertyChanged("IsEditAfterConnect");
        //        }
        //    }
        //}

        /// <summary>
        /// После соединения запретить изменения
        /// </summary>
        public bool IsChangeAfterConnect
        {
            get { return _isChangeAfterConnect; }
            set
            {
                if (_isChangeAfterConnect != value)
                {
                    _isChangeAfterConnect = value;
                    RaisePropertyChanged("IsChangeAfterConnect");
                }
            }
        }


        /// <summary>
        /// Не сохранять значение
        /// </summary>
        public bool IsDontSave
        {
            get { return _isDontSave; }
            set
            {
                if (_isDontSave != value)
                {
                    _isDontSave = value;
                    RaisePropertyChanged("IsDontSave");
                }
            }
        }


        /// <summary>
        /// Указывает что нужно создавать текстовый контрол
        /// </summary>
        public bool IsStirng
        {
            get { return _isStirng; }
            set
            {
                if (_isStirng != value)
                {
                    _isStirng = value;
                    RaisePropertyChanged("IsStirng");
                }
            }
        }

        /// <summary>
        /// Указывает что нужно создавать цифру
        /// </summary>
        public bool IsDigit
        {
            get { return _isDigit; }
            set
            {
                if (_isDigit != value)
                {
                    _isDigit = value;
                    RaisePropertyChanged("IsDigit");
                }
            }
        }


        /// <summary>
        /// Указывает что нужно создавать чекбокс
        /// </summary>
        public bool IsBool
        {
            get { return _isBool; }
            set
            {
                if (_isBool != value)
                {
                    _isBool = value;
                    RaisePropertyChanged("IsBool");
                }
            }
        }

        /// <summary>
        /// Указывает что нужно создавать combobox
        /// </summary>
        public bool IsListString
        {
            get { return _isListString; }
            set
            {
                if (_isListString != value)
                {
                    _isListString = value;
                    RaisePropertyChanged("IsListString");
                }
            }
        }

        /// <summary>
        /// Указывает что нужно создавать список с выбора (например инструменты)
        /// </summary>
        public bool IsUseInstrumentSort
        {
            get { return _isUseInstrumentSort; }
            set
            {
                if (_isUseInstrumentSort != value)
                {
                    _isUseInstrumentSort = value;
                    RaisePropertyChanged("IsUseInstrumentSort");
                }
            }
        }

        /// <summary>
        /// Контрол с добавлением инструментов
        /// </summary>
        public bool IsUseInstrumentAdd
        {
            get { return _isUseInstrumentAdd; }
            set
            {
                if (_isUseInstrumentAdd != value)
                {
                    _isUseInstrumentAdd = value;
                    RaisePropertyChanged("IsUseInstrumentAdd");
                }
            }
        }

        /// <summary>
        /// Значения принимающие правда или ложь (для чекбокс)
        /// </summary>
        public bool ValueBool
        {
            get { return _valueBool; }
            set
            {
                if (_valueBool != value)
                {
                    _valueBool = value;
                    RaisePropertyChanged("ValueBool");
                }
            }
        }

        /// <summary>
        /// Значения принимающие 
        /// </summary>
        public double ValueDigit
        {
            get { return _valueDigit; }
            set
            {
                if (Math.Abs(_valueDigit - value) > CfgSourceEts.MyEpsilon)
                {
                    _valueDigit = value;
                    RaisePropertyChanged("ValueDigit");
                }
            }
        }

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public String ValueString
        {
            get { return _valueString; }
            set
            {
                if (_valueString != value)
                {
                    _valueString = value;
                    RaisePropertyChanged("ValueString");
                }
            }
        }

        /// <summary>
        /// Список
        /// </summary>
        public List<string> ValueListString
        {
            get { return _valueListString; }
            set
            {
                if (_valueListString != value)
                {
                    _valueListString = value;
                    RaisePropertyChanged("ValueListString");
                }
            }
        }



        /// <summary>
        /// Название параметра для интерфейса. 
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public string NameParam
        {
            get { return _nameParam; }
            set
            {
                if (_nameParam != value)
                {
                    _nameParam = value;
                    RaisePropertyChanged("NameParam");
                }
            }
        }


        /// <summary>
        /// Описание параметра. 
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }


        ///// <summary>
        ///// Название переменной. 
        ///// </summary>
        //public string NameVariable
        //{
        //    get { return _nameVariable; }
        //    set
        //    {
        //        if (_nameVariable != value)
        //        {
        //            _nameVariable = value;
        //            RaisePropertyChanged("NameVariable");
        //        }
        //    }
        //}



        #endregion

        #endregion

        public SettingUserCon()
        {

        }

        public SettingUserCon(SettingUserCon set)
        {
            IsDontSave = set.IsDontSave;
            if (!IsDontSave)
            {
                ValueBool = set.ValueBool;
                ValueListString = set.ValueListString;
                ValueString = set.ValueString;
            }
            IsStirng = set.IsStirng;
            IsBool = set.IsBool;
            IsListString = set.IsListString;
            NameParam = set.NameParam;
            Description = set.Description;
            //NameVariable = set.NameVariable;
        }


        public SettingUserCon(SerializationInfo info, StreamingContext context)
        {
            IsDontSave = info.GetBoolean("IsDontSave");
            IsChangeAfterConnect = info.GetBoolean("IsChangeAfterConnect");
            ValueBool = info.GetBoolean("ValueBool");
            ValueListString = (List<string>)info.GetValue("ValueListString", typeof(List<string>));
            ValueString = info.GetString("ValueString");
            ValueDigit = info.GetDouble("ValueDigit");
            IsStirng = info.GetBoolean("IsStirng");
            IsBool = info.GetBoolean("IsBool");
            IsDigit = info.GetBoolean("IsDigit");
            IsListString = info.GetBoolean("IsListString");
            NameParam = info.GetString("NameParam");
            Description = info.GetString("Description");
            //NameVariable = info.GetString("NameVariable");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsDontSave", IsDontSave);
            info.AddValue("ValueBool", ValueBool);
            info.AddValue("IsStirng", IsStirng);
            info.AddValue("IsChangeAfterConnect", IsChangeAfterConnect);
            info.AddValue("IsBool", IsBool);
            info.AddValue("IsDigit", IsDigit);
            info.AddValue("IsListString", IsListString);
            info.AddValue("ValueString", ValueString);
            info.AddValue("ValueListString", ValueListString);
            info.AddValue("ValueString", ValueString);
            info.AddValue("ValueDigit", ValueDigit);
            info.AddValue("NameParam", NameParam);
            info.AddValue("Description", Description);
            //info.AddValue("NameVariable", NameVariable);
        }
    }
}
