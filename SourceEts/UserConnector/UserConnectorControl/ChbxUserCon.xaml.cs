using System;
using System.Windows.Controls;

namespace SourceEts.UserConnector.UserConnectorControl
{
    /// <summary>
    /// Логика взаимодействия для ChbxUserCon.xaml
    /// </summary>
    public partial class ChbxUserCon : UserControl
    {
        /// <summary>
        /// Значение для чекбокса, галочка
        /// </summary>
        public bool ChbxChecked
        {
            get
            {
                if (Chbx.IsChecked == null)
                    Chbx.IsChecked = false;
                return Convert.ToBoolean(Chbx.IsChecked);
            }
            set { Chbx.IsChecked = value; }
        }

        /// <summary>
        /// Название параметра
        /// </summary>
        public string TxblText
        {
            get { return Txbl.Text; }
            set { Txbl.Text = value; }
        }

        /// <summary>
        /// Подсказка, описание параметра
        /// </summary>
        public string TxblToolTipText
        {
            get { return TxblToolTip.Text; }
            set { TxblToolTip.Text = value; }
        }

        public string NameControl
        {

            set { if (value == null) throw new ArgumentNullException("value"); }
        }

        /// <summary>
        /// Подсказка, описание параметра
        /// </summary>
        public bool IsEnable
        {
            get { return UserControl.IsEnable; }
            set { UserControl.IsEnable = value; }
        }

        public ChbxUserCon()
        {
            InitializeComponent();
        }
    }
}
