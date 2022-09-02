using System;
using System.Collections;
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

namespace StyleAndControls.Controls.ViewForOtimParam
{
    /// <summary>
    /// Interaction logic for ItemLbxTxbx.xaml
    /// </summary>
    public partial class ItemLbxChbx : UserControl
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

        public ItemLbxChbx()
        {
            InitializeComponent();
        }
    }
}
