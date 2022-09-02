using System;
using System.Windows.Controls;

namespace StyleAndControls.Controls.ViewIndikators
{
    /// <summary>
    /// Interaction logic for ItemLbxTxbx.xaml
    /// </summary>
    public partial class ItemStplTxbx : UserControl
    {
        /// <summary>
        /// Значение параметра в текстбокс
        /// </summary>
        public string TxbxText
        {
            get { return Txbx.Text; }
            set { Txbx.Text =  value; }
        }

        /// <summary>
        /// Название параметра для текстблока
        /// </summary>
        public string TxblText
        {
            get { return Txbl.Text; }
            set { Txbl.Text = value; }
        }


        public string NameControl
        {

            set { if (value == null) throw new ArgumentNullException("value"); }
        }

        public ItemStplTxbx()
        {
            InitializeComponent();
        }
    }
}
