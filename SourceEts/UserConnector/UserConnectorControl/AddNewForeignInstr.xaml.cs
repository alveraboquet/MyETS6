using ClassControlsAndStyle.Dialogs;
using CommonDataContract;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SourceEts.UserConnector.UserConnectorControl
{
    /// <summary>
    /// Логика взаимодействия для AddNewForeignInstr.xaml
    /// </summary>
    public partial class AddNewForeignInstr
    {
        private AvalibleInstrumentsModel _instr;
        public AddNewForeignInstr(AvalibleInstrumentsModel instr)
        {
            InitializeComponent();
            _instr = instr;
            CmbxCurrencyType.ItemsSource = CfgSourceEts.IbTwsListCurrency;
            CmbxCurrencyType.SelectedIndex = 0;
            CmbxTypeInstrument.ItemsSource = CfgSourceEts.IbTwsListTypeInstrument;
            CmbxTypeInstrument.SelectedIndex = 0;
            TxbxExchange.Text = _instr.ClassCode;
            TxbxPrimaryExchange.Text = _instr.PrimaryExch;

            TxbxCodeInstrument.Text = _instr.Symbol;
            DtpcDateExpire.SelectedDate = DateTime.Now;
            if (!String.IsNullOrEmpty(_instr.DateExpire))
                DtpcDateExpire.SelectedDate = new DateTime(
                    Convert.ToInt32(_instr.DateExpire.Substring(0, 4)),
                    Convert.ToInt32(_instr.DateExpire.Substring(4, 2)),
                    Convert.ToInt32(_instr.DateExpire.Substring(6, 2)));


            if (!String.IsNullOrEmpty(_instr.Currency))
                CmbxCurrencyType.SelectedItem = _instr.Currency;
            if (!String.IsNullOrEmpty(_instr.TypeSymbol))
                CmbxTypeInstrument.SelectedItem = _instr.TypeSymbol;

            TxbxMinStep.Text = _instr.MinStep.ToString();
            TxbxMargin.Text = _instr.Margin.ToString();
            CmbxPriceQuotation.ItemsSource = CfgSourceEts.FormatCodesList;
            CmbxPriceQuotation.SelectedIndex = 0;
            if (!String.IsNullOrEmpty(_instr.PriceQuotationString))
                CmbxPriceQuotation.SelectedItem = _instr.PriceQuotationString;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TxbxCodeInstrument.Text))
            {
                new DialogMessage("Код инструмента не может быть пустым", "Внимание!");
                return;
            }

            _instr.Symbol = TxbxCodeInstrument.Text;

            _instr.Currency = CmbxCurrencyType.SelectedItem.ToString();
            _instr.ClassCode = TxbxExchange.Text;
            _instr.PrimaryExch = TxbxPrimaryExchange.Text;
            _instr.ClassCodeVisible = _instr.ClassCode;
            _instr.ShortName = _instr.PrimaryExch;
            _instr.TypeSymbol = CmbxTypeInstrument.SelectedItem.ToString();
            if (_instr.TypeSymbol == "FUT" || _instr.TypeSymbol == "OPT")
            {
                DateTime date = Convert.ToDateTime(DtpcDateExpire.SelectedDate);

                _instr.DateExpire = date.Year.ToString() +
                    (date.Month < 9 ? ("0" + date.Month.ToString()) : date.Month.ToString()) +
                    (date.Day < 9 ? ("0" + date.Day.ToString()) : date.Day.ToString());

                _instr.MinStep = Convert.ToDouble(TxbxMinStep.Text);
                if (_instr.MinStep <= 0)
                {
                    new DialogMessage("Шаг цены инструмента должен быть больше нуля", "Внимание!");
                    return;
                }

                _instr.PriceQuotationString = CmbxPriceQuotation.SelectedItem.ToString();
                _instr.PriceQuotation = CfgSourceEts.GetFormatCodesOnNumber(_instr.PriceQuotationString);
                _instr.Margin = Convert.ToDouble(TxbxMargin.Text);

            }
            DialogResult = true;
            Close();
        }

        private void CmbxTypeInstrument_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DtpcDateExpire.IsEnabled = false;
            TxbxMinStep.IsEnabled = false;
            CmbxPriceQuotation.IsEnabled = false;
            TxbxMargin.IsEnabled = false;
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                var item = comboBox.SelectedItem.ToString();
                if (item == "FUT" || item == "OPT")
                {
                    DtpcDateExpire.IsEnabled = true;
                    TxbxMinStep.IsEnabled = true;
                    CmbxPriceQuotation.IsEnabled = true;
                    TxbxMargin.IsEnabled = true;
                }
            }
        }
    }
}
