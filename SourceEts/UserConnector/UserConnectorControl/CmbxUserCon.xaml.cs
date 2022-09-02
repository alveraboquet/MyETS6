using System;
using System.Collections;
using System.Windows.Controls;

namespace SourceEts.UserConnector.UserConnectorControl
{
    /// <summary>
    /// Логика взаимодействия для CmbxUserCon.xaml
    /// </summary>
    public partial class CmbxUserCon : UserControl
    {

        /// <summary>
        /// Источник данных для комбобокс
        /// </summary>
        public IEnumerable CmbxSource
        {
            get { return Cmbx.ItemsSource; }
            set { Cmbx.ItemsSource = value; }
        }

        /// <summary>
        /// Элемент в комбобоксе
        /// </summary>
        public string SelectedItem
        {
            get
            {
                if (Cmbx.SelectedItem == null)
                    return null;
                return Cmbx.SelectedItem.ToString();
            }
            set
            {
                //Cmbx.SelectedItem = value;
                for (int i = 0; i < Cmbx.Items.Count; i++)
                {
                    if (Cmbx.Items[i].ToString() == value)
                    {
                        Cmbx.SelectedIndex = i;
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// Элемент в комбобоксе по идексу
        /// </summary>
        public int SelectedInex
        {
            get { return Cmbx.SelectedIndex; }
            set { Cmbx.SelectedIndex = value; }

        }

        /// <summary>
        /// Навзание параметра
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

        public CmbxUserCon()
        {
            InitializeComponent();
        }
    }
}
