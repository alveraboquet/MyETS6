using System;
using System.Collections;
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
    /// Логика взаимодействия для ItemLbsCmbxSymbol.xaml
    /// </summary>
    public partial class ItemLbxCmbxSymbol : UserControl
    {
        public ItemLbxCmbxSymbol()
        {
            InitializeComponent();
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

        #region ClientCodes
        /// <summary>
        /// Источник данных для комбобокс
        /// </summary>
        public IEnumerable CmbxSourceClientCodes
        {
            get { return CmbxClientCode.ItemsSource; }
            set { CmbxClientCode.ItemsSource = value; }
        }

        /// <summary>
        /// Элемент в комбобоксе
        /// </summary>
        public string SelectedItemCleintCodes
        {
            get
            {
                if (CmbxClientCode.SelectedItem == null)
                    return null;
                return CmbxClientCode.SelectedItem.ToString();
            }
            set
            {
                //Cmbx.SelectedItem = value;
                for (int i = 0; i < CmbxClientCode.Items.Count; i++)
                {
                    if (CmbxClientCode.Items[i].ToString() == value)
                    {
                        CmbxClientCode.SelectedIndex = i;
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// Элемент в комбобоксе по идексу
        /// </summary>
        public int SelectedIndexClientCodes
        {
            get { return CmbxClientCode.SelectedIndex; }
            set { CmbxClientCode.SelectedIndex = value; }
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
                list.Insert(0,"не выбран");

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
                if (arr.Length > 2 && !String.IsNullOrEmpty(arr[2]))
                    CmbxClientCode.SelectedItem = arr[2];
            }
            
        }



        /// <summary>
        /// Загрузка счетов
        /// </summary>
        public void SetClientCodes(List<string> clientCodes, bool visibleAccounts)
        {
            CmbxClientCode.ItemsSource = clientCodes;
            clientCodes.Insert(0, "не выбран");
            CmbxClientCode.SelectedIndex = 0;
            if (visibleAccounts)
            Height = 55;
        }
    }
}
