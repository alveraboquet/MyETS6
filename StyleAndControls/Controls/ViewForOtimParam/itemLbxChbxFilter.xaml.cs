using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для itemLbxChbxFilter.xaml
    /// </summary>
    public partial class itemLbxChbxFilter : UserControl
    {
        public itemLbxChbxFilter()
        {
            InitializeComponent();
        }

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


        public string NameControl
        {

            set { if (value == null) throw new ArgumentNullException("value"); }
        }


    }
}
