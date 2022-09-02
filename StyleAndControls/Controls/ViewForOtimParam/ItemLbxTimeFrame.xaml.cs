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
using System.Collections;

namespace StyleAndControls.Controls.ViewForOtimParam
{
    /// <summary>
    /// Логика взаимодействия для ItemLbxTimeFrame.xaml
    /// </summary>
    public partial class ItemLbxTimeFrame : UserControl
    {
        public ItemLbxTimeFrame()
        {
            InitializeComponent();

            List<string> list = new List<string>();
            //list.Add(CfgSourceEts.TypeTimeFrameTicks);
            //list.Add(CfgSourceEts.TypeTimeFrameSeconds);
            //list.Add(CfgSourceEts.TypeTimeFrameMinutes);
            //list.Add(CfgSourceEts.TypeTimeFrameDays);
            //list.Add(CfgSourceEts.TypeTimeFrameWeek);
            //list.Add(CfgSourceEts.TypeTimeFrameMonth);
            list.Add("Тик");
            list.Add("Секунда");
            list.Add("Минута");
            list.Add("День");
            list.Add("Неделя");
            list.Add("Месяц");

            CmbxTypeTf.ItemsSource = list;
        }

        #region Symbol
        /// <summary>
        /// Источник данных для комбобокс
        /// </summary>
        public IEnumerable CmbxSourceSymbol
        {
            get { return CmbxSymbol.ItemsSource; }
            set { CmbxSymbol.ItemsSource = value; }
        }

        /// <summary>
        /// Элемент в комбобоксе
        /// </summary>
        public string SelectedItemSymbol
        {
            get
            {
                if (CmbxSymbol.SelectedItem == null)
                    return null;
                return CmbxSymbol.SelectedItem.ToString();
            }
            set
            {
                //Cmbx.SelectedItem = value;
                for (int i = 0; i < CmbxSymbol.Items.Count; i++)
                {
                    if (CmbxSymbol.Items[i].ToString() == value)
                    {
                        CmbxSymbol.SelectedIndex = i;
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// Элемент в комбобоксе по идексу
        /// </summary>
        public int SelectedIndexSymbol
        {
            get { return CmbxSymbol.SelectedIndex; }
            set { CmbxSymbol.SelectedIndex = value; }

        }

        #endregion

        #region ClassCode
        /// <summary>
        /// Источник данных для комбобокс
        /// </summary>
        public IEnumerable CmbxSourceClassCode
        {
            get { return CmbxClasscode.ItemsSource; }
            set { CmbxClasscode.ItemsSource = value; }
        }

        /// <summary>
        /// Элемент в комбобоксе
        /// </summary>
        public string SelectedItemClassCode
        {
            get
            {
                if (CmbxClasscode.SelectedItem == null)
                    return null;
                return CmbxClasscode.SelectedItem.ToString();
            }
            set
            {
                //Cmbx.SelectedItem = value;
                for (int i = 0; i < CmbxClasscode.Items.Count; i++)
                {
                    if (CmbxClasscode.Items[i].ToString() == value)
                    {
                        CmbxClasscode.SelectedIndex = i;
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// Элемент в комбобоксе по идексу
        /// </summary>
        public int SelectedIndexClassCode
        {
            get { return CmbxClasscode.SelectedIndex; }
            set { CmbxClasscode.SelectedIndex = value; }

        }

        #endregion

        /// <summary>
        /// Значение параметра в текстбокс
        /// </summary>
        public string TxbxTimeFrame
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

        #region TypeTimeFrame

        /// <summary>
        /// Источник данных для комбобокс
        /// </summary>
        public IEnumerable CmbxSourceTypeTimeFrame
        {
            get { return CmbxTypeTf.ItemsSource; }
            set { CmbxTypeTf.ItemsSource = value; }
        }

        /// <summary>
        /// Элемент в комбобоксе
        /// </summary>
        public string SelectedItemTimeFrame
        {
            get
            {
                if (CmbxTypeTf.SelectedItem == null)
                    return null;
                return CmbxTypeTf.SelectedItem.ToString();
            }
            set
            {
                //Cmbx.SelectedItem = value;
                for (int i = 0; i < CmbxTypeTf.Items.Count; i++)
                {
                    if (CmbxTypeTf.Items[i].ToString() == value)
                    {
                        CmbxTypeTf.SelectedIndex = i;
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// Элемент в комбобоксе по идексу
        /// </summary>
        public int SelectedIndexTimeFrame
        {
            get { return CmbxTypeTf.SelectedIndex; }
            set { CmbxTypeTf.SelectedIndex = value; }

        }

        #endregion




        public string NameControl
        {
            set { if (value == null) throw new ArgumentNullException("value"); }
        }

        private List<string> symbolList = new List<string>();
        /// <summary>
        /// Устанавливаем код и класс
        /// </summary>
        public void SetClassAndSeccode(List<String> listClass, List<string> listSymbolClass)
        {
            symbolList = listSymbolClass;

            CmbxClasscode.ItemsSource = listClass;
            CmbxClasscode.SelectedIndex = 0;
        }

        private void CmbxClasscode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmbx = sender as ComboBox;
            if (sender != null && cmbx.SelectedItem != null)
            {
                List<string> list = new List<string>();
                for (int i = 0; i < symbolList.Count; i++)
                {
                    var arr = symbolList[i].Split('/');
                    if (arr[1] == cmbx.SelectedItem.ToString())
                    {
                        list.Add(arr[0].Trim());
                    }
                }
                list.Sort();
                list.Insert(0, "не выбран");

                CmbxSymbol.ItemsSource = list;
                CmbxSymbol.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Выбор сохраненных данных
        /// </summary>
        public void LoadSymbols(string symbolClassCode)
        {

            var arr = symbolClassCode.Split('/');

            if (arr.Length > 1)
            {
                CmbxClasscode.SelectedItem = arr[1].Trim();
                CmbxSymbol.SelectedItem = arr[0].Trim();
            }

        }


        ///// <summary>
        ///// Устанавливаем код и класс
        ///// </summary>
        //public void SetTimeFrame(List<String> listClass, List<string> listSymbolClass)
        //{

        //    //CmbxClasscode.ItemsSource = listClass;
        //    //CmbxClasscode.SelectedIndex = 0;
        //}

        //private void CmbxClasscode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}



    }
}
