using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;


namespace StyleAndControls.Controls.ViewForOtimParam
{
    /// <summary>
    /// Interaction logic for ItemLbxPath.xaml
    /// </summary>
    public partial class ItemLbxPath 
    {
        public ItemLbxPath()
        {
            InitializeComponent();
        }

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

        private void BtnPath_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Файлы *|*.*";

            if (file.ShowDialog() == true)
            {
                Txbx.Text = file.FileName;

              
            }
        }
    }
}
