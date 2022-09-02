using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace StyleAndControls.Controls.Models
{
    public class ScanerInstrParamDtgdModel : INotifyPropertyChanged
    {
        #region Private

        private string _symbol;
        private string _classCode;
        private string _valueString;
        private double _value;
        private int _valueInt;
        private bool _valueBool;

        #endregion

        #region public

        /// <summary>
        /// . 
        /// </summary>
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                if (_symbol != value)
                {
                    _symbol = value;
                    RaisePropertyChanged("Symbol");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public string ClassCode
        {
            get { return _classCode; }
            set
            {
                if (_classCode != value)
                {
                    _classCode = value;
                    RaisePropertyChanged("ClassCode");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public string ValueString
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
        /// . 
        /// </summary>
        public double Value
        {
            get { return _value; }
            set
            {
                if (Math.Abs(_value - value) > 0.0000001)
                {
                    _value = value;
                    RaisePropertyChanged("Value");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public int ValueInt
        {
            get { return _valueInt; }
            set
            {
                if (_valueInt != value)
                {
                    _valueInt = value;
                    RaisePropertyChanged("ValueInt");
                }
            }
        }

        /// <summary>
        /// . 
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



        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
