using System;
using System.Collections;
using System.Windows.Controls;

namespace StyleAndControls.Controls.ViewIndikators
{
    /// <summary>
    /// Interaction logic for ItemLbxTxbx.xaml
    /// </summary>
    public partial class ItemStplCmbx : UserControl
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
                if (Cmbx.SelectedItem==null)
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
        /// Значение в комбобоксе
        /// </summary>
        public string SelectedValue
        {
            get
            {
                if (Cmbx.SelectedValue == null)
                    return null;
                return Cmbx.SelectedValue.ToString();
            }
            set { Cmbx.SelectedValue = value; }

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


        public string NameControl
        {
            
            set { if (value == null) throw new ArgumentNullException("value"); }
        }

        public ItemStplCmbx()
        {
            InitializeComponent();
        }
    }
}
