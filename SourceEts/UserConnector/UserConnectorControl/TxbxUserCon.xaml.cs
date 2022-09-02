using System.Windows.Controls;

namespace SourceEts.UserConnector.UserConnectorControl
{
    /// <summary>
    /// Логика взаимодействия для TxbxUserCon.xaml
    /// </summary>
    public partial class TxbxUserCon : UserControl
    {
        /// <summary>
        /// Значение параметра в текстбокс
        /// </summary>
        public string TxbxText
        {
            get { return Txbx.Text; }
            set { Txbx.Text = value; }
        }

        /// <summary>
        /// Название параметра для текстблока
        /// </summary>
        public string TxblText
        {
            get { return Txbl.Text; }
            set { Txbl.Text = value; }
        }

        /// <summary>
        /// Тултип
        /// </summary>
        public string TxblToolTipText
        {
            get { return TxblToolTip.Text; }
            set { TxblToolTip.Text = value; }
        }


        /// <summary>
        /// Подсказка, описание параметра
        /// </summary>
        public bool IsEnable
        {
            get { return UserControl.IsEnable; }
            set { UserControl.IsEnable = value; }
        }

        public TxbxUserCon()
        {
            InitializeComponent();
        }
    }
}
