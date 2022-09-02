using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalRbkm.Plugins
{
    public class PluginInitializationData: INotifyPropertyChanged
    {
        private String _pathToSave;
        private String _nameRobot;
        private String _desription;
      //  public String Some;
       //public  something;


        
        /// <summary>
        ///  Путь сохранения данных
        /// </summary>
        public String PathToSave
        {
            get { return _pathToSave; }
            set
            {
                if (_pathToSave != value)
                {
                    _pathToSave = value;
                    RaisePropertyChanged("PathToSave");
                }
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public string NameRobot
        {
            get { return _nameRobot; }
            set
            {
                if (_nameRobot != value)
                {
                    _nameRobot = value;
                    RaisePropertyChanged("NameRobot");
                }
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public string Desription
        {
            get { return _desription; }
            set
            {
                if (_desription != value)
                {
                    _desription = value;
                    RaisePropertyChanged("Desription");
                }
            }
        }
           
           

        #region event PropertyChangedEventHandler

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
