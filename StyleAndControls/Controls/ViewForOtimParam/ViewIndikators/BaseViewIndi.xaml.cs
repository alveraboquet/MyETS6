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
using System.Windows.Shapes;

namespace StyleAndControls.Controls.ViewIndikators
{
    /// <summary>
    /// Interaction logic for BaseViewIndi.xaml
    /// </summary>
    public partial class BaseViewIndi
    {

        /// <summary>
        /// SelectedIndex cmbxTypeInterval
        /// </summary>
        public int CmbxTypeIntervalSelectedIndex
        {
            get { return CmbxTypeInterval.SelectedIndex; }
            set { CmbxTypeInterval.SelectedIndex = value; }
        }

        /// <summary>
        /// SelectedIndex cmbxTypeInterval
        /// </summary>
        public object CmbxTypeIntervalSelectedItem
        {
            get { return CmbxTypeInterval.SelectedItem; }
            set
            {
                for (int i = 0; i < CmbxTypeInterval.Items.Count; i++)
                {
                    if (CmbxTypeInterval.Items[i].ToString() == (string)value)
                    {
                        CmbxTypeInterval.SelectedIndex = i;
                        break;
                    }
                }
                //CmbxTypeInterval.SelectedItem = value;
            }
        }

        /// <summary>
        /// ItemSource cmbx Type Interval
        /// </summary>
        public IEnumerable CmbxTypeIntervalItemSource
        {
            get { return CmbxTypeInterval.ItemsSource; }
            set { CmbxTypeInterval.ItemsSource = value; }
        }


        #region CmbxSymbol



        ///// <summary>
        ///// SelectedIndex CmbxSymbol
        ///// </summary>
        //public int CmbxSymbolSelectedIndex
        //{
        //    get { return CmbxSymbol.SelectedIndex; }
        //    set { CmbxSymbol.SelectedIndex = value; }
        //}

        ///// <summary>
        ///// SelectedIndex CmbxSymbol
        ///// </summary>
        //public object CmbxSymbolSelectedItem
        //{
        //    get { return CmbxSymbol.SelectedItem; }
        //    set
        //    {
        //        for (int i = 0; i < CmbxSymbol.Items.Count; i++)
        //        {
        //            if (CmbxSymbol.Items[i] != null)
        //                if (CmbxSymbol.Items[i].ToString() == (string)value)
        //                {
        //                    CmbxSymbol.SelectedIndex = i;
        //                    break;
        //                }
        //        }
        //        //CmbxTypeInterval.SelectedItem = value;
        //    }
        //}

        ///// <summary>
        ///// ItemSource cmbx CmbxSymbol
        ///// </summary>
        //public IEnumerable CmbxSymbolItemSource
        //{
        //    get { return CmbxSymbol.ItemsSource; }
        //    set
        //    {
        //        CmbxSymbol.ItemsSource = value;
        //    }
        //}

        //public void LoadSymbolAndClassCode(List<string> listClass, List<string> listSymbolAndClass, string symbol, string classCode)
        //{
        //    UcSymbol.SetClassAndSeccode(listClass, listSymbolAndClass);
        //    UcSymbol.LoadSymbols(symbol + "/" + classCode);

        //}

        //public string GetClassCode()
        //{
        //    return UcSymbol.CmbxClasscode.SelectedItem != null ? UcSymbol.CmbxClasscode.SelectedItem.ToString() : "";
        //}

        //public string GetSymbol()
        //{
        //    return UcSymbol.CmbxSymbol.SelectedItem != null ? UcSymbol.CmbxSymbol.SelectedItem.ToString() : "";
        //}

        //public void ChangeInstr(string symbol, string classcode)
        //{
        //    UcSymbol.LoadSymbols(symbol + "/" + classcode);
        //}

        #endregion


        /// <summary>
        /// Title GroupBox
        /// </summary>
        public object GpbxHeader
        {
            get { return GpbxIndi.Header; }
            set { GpbxIndi.Header = value; }
        }

        /// <summary>
        /// Текст интеравал
        /// </summary>
        public string TxbxIntervalText
        {
            get { return TxbxInterval.Text; }
            set { TxbxInterval.Text = value; }
        }

        /// <summary>
        /// Добавляем в стекпанель настраевыемые параметры индикатора
        /// </summary>
        public UIElement StplChildren
        {
            get { return null; }
            set { StpnLines.Children.Add(value); }
        }

        public UIElementCollection GetShildrenStplBase()
        {
            return StpnLines.Children;
        }

        public BaseViewIndi()
        {
            InitializeComponent();


        }

        /// <summary>
        /// Идентификатор контрола
        /// </summary>
        public string GpbxName
        {
            get { return GpbxIndi.Name; }
            set { GpbxIndi.Name = value; }
        }

        /// <summary>
        /// Показывать настройки таймфрейма индикатора
        /// </summary>
        public void VisibleTimeFrame()
        {
            DtgdRowTimeFrame.Height = new GridLength(28);
        }
    }
}
