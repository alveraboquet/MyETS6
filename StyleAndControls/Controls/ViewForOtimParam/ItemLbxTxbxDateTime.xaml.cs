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

namespace StyleAndControls.Controls.ViewForOtimParam
{
    /// <summary>
    /// Interaction logic for ItemLbxTxbxString.xaml
    /// </summary>
    public partial class ItemLbxTxbxDateTime
    {
        /// <summary>
        /// Значение параметра в текстбокс
        /// </summary>
        public string TxbxText
        {
            get { return Txbx.Text; }
            set
            {
                //var tmp = value.Split(':');
                //if (Convert.ToInt32(tmp[0]) > 23)
                //    tmp[0] = 23.ToString();
                //if (Convert.ToInt32(tmp[1]) > 59)
                //    tmp[1] = 59.ToString();
                //if (Convert.ToInt32(tmp[2]) > 59)
                //    tmp[2] = 59.ToString();

                //var val = tmp[0] + ":" + tmp[1] + ":" + tmp[2];

                Txbx.Text = value;
            }
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

        public ItemLbxTxbxDateTime()
        {
            InitializeComponent();
        }
    }
}
