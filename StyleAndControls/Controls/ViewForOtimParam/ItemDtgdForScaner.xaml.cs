using StyleAndControls.Controls.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для ItemDtgdForScaner.xaml
    /// </summary>
    public partial class ItemDtgdForScaner : UserControl
    {


        public ObservableCollection<ScanerInstrParamDtgdModel> Col = new ObservableCollection<ScanerInstrParamDtgdModel>();

        public ItemDtgdForScaner()
        {
            InitializeComponent();
            DtgdParam.ItemsSource = Col;
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


        private bool _isDouble { get; set; }
        private bool _isInt { get; set; }
        private bool _isBool { get; set; }
        private bool _isString { get; set; }

        public void SetType(bool isDouble, bool isInt, bool isBool, bool isString)
        {
            _isDouble = isDouble;
            _isInt = isInt;
            _isBool = isBool;
            _isString = isString;

            foreach (var item in DtgdParam.Columns)
            {

                if (item.Header.ToString() == "Bool" && isBool)
                {
                    item.Header = Txbl.Text;
                    item.Visibility = Visibility.Visible;
                }

                if (item.Header.ToString() == "Double" && isDouble)
                {
                    item.Header = Txbl.Text;
                    item.Visibility = Visibility.Visible;
                }

                if (item.Header.ToString() == "Int" && isInt)
                {
                    item.Header = Txbl.Text;
                    item.Visibility = Visibility.Visible;
                }

                if (item.Header.ToString() == "String" && isString)
                {
                    item.Header = Txbl.Text;
                    item.Visibility = Visibility.Visible;
                }
            }
        }

        public List<string> GetListString()
        {
            List<string> list = new List<string>();
            if (_isString)
                foreach (var itemCol in Col)
                {
                    list.Add(itemCol.ValueString);
                }

            return list;
        }

        public List<double> GetListDouble()
        {
            List<double> list = new List<double>();
            if (_isDouble)
                foreach (var itemCol in Col)
                {
                    list.Add(itemCol.Value);
                }

            return list;
        }

        public List<int> GetListInt()
        {
            List<int> list = new List<int>();
            if (_isInt)
                foreach (var itemCol in Col)
                {
                    list.Add(itemCol.ValueInt);
                }

            return list;
        }

        public List<bool> GetListBool()
        {
            List<bool> list = new List<bool>();
            if (_isString)
                foreach (var itemCol in Col)
                {
                    list.Add(itemCol.ValueBool);
                }

            return list;
        }


        public void AddInst(ScanerInstrParamDtgdModel item)
        {
            Col.Add(item);
        }

        public void DelInst(string symbol, string classCode)
        {
            foreach (var item in Col)
            {
                if (item.Symbol == symbol && item.ClassCode == classCode)
                {
                    Col.Remove(item);
                    break;
                }
            }
        }
    }
}
