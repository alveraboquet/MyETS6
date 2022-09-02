using Adapter;
using Adapter.Config;
using Adapter.Model;
using CommonDataContract;
using CommonDataContract.AbstractDataTypes;
using SourceEts.Table;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TerminalRbkm.View
{
    /// <summary>
    /// Логика взаимодействия для SendOrder.xaml
    /// </summary>
    public partial class SendOrder1 : Window
    {

        private AllTerminals _absTerminal;
        public SendOrder1(AllTerminals absTerm)
        {
            InitializeComponent();

            _absTerminal = absTerm;

            SetStandartParam();
        }

        public SendOrder1(AllTerminals absTerm, string symbol, string classCode)
        {
            InitializeComponent();
            _absTerminal = absTerm;

            SetStandartParam();

            //SetSec(sec);
            UpdateData(symbol, classCode);

        }

        public SendOrder1(AllTerminals absTerm, string symbol, string clientCode, string account)
        {
            InitializeComponent();
            _absTerminal = absTerm;

            SetStandartParam();

            //SetSec(sec);
            UpdateData(symbol, clientCode, account);

        }

        public SendOrder1(AllTerminals absTerm, string symbol, string clientCode, string account, bool operBuy, double qty, double price)
        {
            InitializeComponent();
            _absTerminal = absTerm;

            SetStandartParam();

            //SetSec(sec);

            UpdateData(symbol, clientCode, account, operBuy, qty, price);

        }

        internal void UpdateData(string symbol, string classCode)
        {
            var sec = _absTerminal.GetSecurity(symbol, classCode);
            if (sec != null)
                if (!String.IsNullOrEmpty(sec.ClassCode))
                    CmbxClassCode.SelectedItem = sec.ClassCode;

            if (!String.IsNullOrEmpty(symbol))
                CmbxSymbol.SelectedItem = symbol;
        }

        internal void UpdateData(string symbol, string clientCode, string account)
        {
            var sec = _absTerminal.GetSecurity(symbol);
            if (sec != null)
                if (!String.IsNullOrEmpty(sec.ClassCode))
                    CmbxClassCode.SelectedItem = sec.ClassCode;

            if (!String.IsNullOrEmpty(symbol))
                CmbxSymbol.SelectedItem = symbol;

            //if (!String.IsNullOrEmpty(clientCode) &&
            //    !String.IsNullOrEmpty(account))
            //    if (clientCode != account)
            //        CmbxAccount.SelectedItem = clientCode + " (" + account + ")";
            //    else
            //        CmbxAccount.SelectedItem = clientCode;


            for (int i = 0; i < CmbxAccount.Items.Count; i++)
            {
                if ((CmbxAccount.Items[i] is AccountsPair))
                    if ((CmbxAccount.Items[i] as AccountsPair).Account == account &&
                        (CmbxAccount.Items[i] as AccountsPair).ClientCode == clientCode)
                    {

                        CmbxAccount.SelectedIndex = i;
                        break;
                    }

            }
        }

        internal void UpdateData(string symbol, string clientCode, string account, bool operBuy, double qty, double price)
        {
            var sec = _absTerminal.GetSecurity(symbol);
            if (sec != null)
                if (!String.IsNullOrEmpty(sec.ClassCode))
                    CmbxClassCode.SelectedItem = sec.ClassCode;

            if (!String.IsNullOrEmpty(symbol))
                CmbxSymbol.SelectedItem = symbol;

            for (int i = 0; i < CmbxAccount.Items.Count; i++)
            {
                if ((CmbxAccount.Items[i] is AccountsPair))
                    if ((CmbxAccount.Items[i] as AccountsPair).Account == account &&
                        (CmbxAccount.Items[i] as AccountsPair).ClientCode == clientCode)
                    {

                        CmbxAccount.SelectedIndex = i;
                        break;
                    }

            }


            //if (!String.IsNullOrEmpty(clientCode) &&
            //    !String.IsNullOrEmpty(account))
            //    if (clientCode != account)
            //        CmbxAccount.SelectedItem = clientCode + " (" + account + ")";
            //    else
            //        CmbxAccount.SelectedItem = clientCode;

            if (operBuy)
            {
                ChbxBuy.IsChecked = true;
                ChbxSell.IsChecked = false;
            }
            else
            {
                ChbxBuy.IsChecked = false;
                ChbxSell.IsChecked = true;
            }
            if (sec.LotSize > 1)
                qty = Math.Round(qty, 0);

            TxbxQty.Text = qty.ToString();

            TxbxPrice.Text = price.ToString("F8").TrimEnd('0');
        }

        private void SetStandartParam()
        {
            CmbxAccount.ItemsSource = _absTerminal.GetPairClienCodesAccount();
            CmbxAccount.SelectedIndex = 0;

            CmbxClassCode.ItemsSource = _absTerminal.GetClasscodeList();
            CmbxClassCode.SelectedIndex = 0;

            if (CmbxClassCode.SelectedItem != null)
            {
                CmbxSymbol.ItemsSource = _absTerminal.GetSeccodeList(CmbxClassCode.SelectedItem.ToString());
                CmbxSymbol.SelectedIndex = 0;
            }


        }


        private void BtnRevers_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TxblCurPos.Text))
            {
                double pos = ConfigTermins.ConvertToDoubleMy(TxblCurPos.Text);

                if (pos > 0)
                {
                    ChbxSell.IsChecked = true;
                    ChbxBuy.IsChecked = false;
                    TxbxQty.Text = Math.Abs(pos + pos).ToString();
                }

                if (pos < 0)
                {
                    ChbxBuy.IsChecked = true;
                    ChbxSell.IsChecked = false;
                    TxbxQty.Text = Math.Abs(pos + pos).ToString();
                }
            }

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TxblCurPos.Text))
            {
                double pos = ConfigTermins.ConvertToDoubleMy(TxblCurPos.Text);

                if (pos > 0)
                {
                    ChbxSell.IsChecked = true;
                    ChbxBuy.IsChecked = false;
                    TxbxQty.Text = Math.Abs(pos).ToString();
                }

                if (pos < 0)
                {
                    ChbxBuy.IsChecked = true;
                    ChbxSell.IsChecked = false;
                    TxbxQty.Text = Math.Abs(pos).ToString();
                }
            }


        }




        private void CmbxSymbol_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetPos(sender as ComboBox, CmbxAccount);
        }

        private void CmbxAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetPos(CmbxSymbol, sender as ComboBox);
        }

        private void SetPos(ComboBox cmbxSec, ComboBox cmbxAcc)
        {
            if (cmbxSec != null && cmbxSec.SelectedItem != null && cmbxAcc != null)
            {
                var sec = _absTerminal.GetSecurity(cmbxSec.SelectedItem.ToString());
                if (cmbxAcc.SelectedItem is AccountsPair && sec != null)
                {

                    var client = cmbxAcc.SelectedItem as AccountsPair;
                    IPos pos = _absTerminal.GetPositionSharesItem(client.ClientCode, client.Account, sec.Seccode);

                    IPos posFut = _absTerminal.GetPositionFuturesItem(client.ClientCode, sec.Seccode);
                    double curPos = 0;
                    if (pos != null)
                        curPos = pos.Balance;
                    if (posFut != null)
                        curPos = posFut.Balance;

                    TxblCurPos.Text = curPos.ToString();
                    TxblCurPos.Foreground = Brushes.White;

                    if (curPos > 0)
                        TxblCurPos.Foreground = Brushes.LightGreen;
                    if (curPos < 0)
                        TxblCurPos.Foreground = Brushes.LightCoral;

                    DataContext = sec;
                    TxbxPrice.Text = sec.LastPrice.ToString("F8").TrimEnd('0');
                }

            }
        }




        private void CmbxClassCode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbxClassCode.SelectedItem != null)
            {
                var itemSource = _absTerminal.GetSeccodeList(CmbxClassCode.SelectedItem.ToString());
                if (itemSource != null)
                {
                    CmbxSymbol.ItemsSource = itemSource.OrderBy(a => a);
                    CmbxSymbol.SelectedIndex = 0;
                }
            }
        }

        private void ChbxMarketPrice_OnClick(object sender, RoutedEventArgs e)
        {
            var item = sender as CheckBox;
            if (item != null)
            {
                TxbxPrice.IsEnabled = !Convert.ToBoolean(item.IsChecked);
                TxblPrice.IsEnabled = !Convert.ToBoolean(item.IsChecked);
            }
        }

        private void ChbxSell_OnClick(object sender, RoutedEventArgs e)
        {
            ChbxBuy.IsChecked = false;
            ChbxSell.IsChecked = true;
        }

        private void ChbxBuy_OnClick(object sender, RoutedEventArgs e)
        {
            ChbxSell.IsChecked = false;
            ChbxBuy.IsChecked = true;

        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            //_absTerminal.SendTransaction(GetParamOfTranscationModelOrder())
            try
            {
                var param = GetParamOfTranscationModelOrder();
                if (param == null)
                    return;
                var absTerm = _absTerminal.GetTerminal(param.Account, param.ClientCode);
                if (absTerm == null)
                    return;

                _absTerminal.Raise_OnSomething("Отправка заявки в ручную: " + param.Symbol + "(" + param.ClassCode + ")");
                if (absTerm.IsConnect)
                    absTerm.TransactionOrder(param);

                //Close();
            }
            catch (Exception ex)
            {
                _absTerminal.Raise_OnSomething("Ошибка при попытке отправить заявку в ручную: " + ex.Message);

            }
        }
        #region Транзакции


        /// <summary>
        /// Получаем параметры для транзакции
        /// </summary>
        /// <returns></returns>
        public ParamOfTransactionModel GetParamOfTranscationModelOrder()
        {
            AccountsPair client = null;
            //var clientArr = CmbxAccount.SelectedItem.ToString().Split(' ');
            //if (clientArr.Any())
            //    client = clientArr[0];
            if (CmbxAccount.SelectedItem is AccountsPair)
                client = (CmbxAccount.SelectedItem as AccountsPair);

            if (client == null ||
                CmbxSymbol.SelectedItem == null ||
                CmbxClassCode.SelectedItem == null)
                return null;
            bool isMarket = false;
            //string account = client;
            //if (clientArr.Count() > 1)
            //    account = clientArr[1].Replace("(", "").Replace(")", "").Trim();
            var paramOfTransaction = new ParamOfTransactionModel();
            paramOfTransaction.Operation = ChbxBuy.IsChecked == true ? 'B' : 'S';
            string oper = ChbxBuy.IsChecked == true ? ConfigTermins.Buy : ConfigTermins.Sell;
            paramOfTransaction.IsMarketOrder = "L";
            if (ChbxMarketPrice.IsChecked == true)
            {
                paramOfTransaction.IsMarketOrder = "M";
                isMarket = true;
            }

            paramOfTransaction.Account = client.Account;
            paramOfTransaction.ClientCode = client.ClientCode;
            paramOfTransaction.TrandId = 601.ToString();
            paramOfTransaction.Symbol = CmbxSymbol.SelectedItem.ToString();
            paramOfTransaction.ClassCode = CmbxClassCode.SelectedItem.ToString();

            paramOfTransaction.Price = ConfigTermins.ConvertToDoubleMy(TxbxPrice.Text);
            paramOfTransaction.Quantity = ConfigTermins.ConvertToDoubleMy(TxbxQty.Text);
            var finInfo = _absTerminal.GetSecurity(CmbxSymbol.SelectedItem.ToString(), CmbxClassCode.SelectedItem.ToString());
            if (isMarket)
                paramOfTransaction.Price = GetMarketPrice(finInfo, true, oper);

            paramOfTransaction.TypeOrder = CfgSourceEts.TypeRequestForTransactionOrder;

            return paramOfTransaction;
        }






        #endregion

        private double GetMarketPrice(ISecurity sec, bool isMarket, string oper)
        {
            double result = 0;
            if (isMarket)
            {
                if (sec.ClassCode == CfgSourceEts.ClasscodeSpbfut)
                    result = oper == CfgSourceEts.Buy ? sec.MaxPrice : sec.MinPrice;
                if (sec.ClassCode != CfgSourceEts.ClasscodeSpbfut && sec.ClassCode != CfgSourceEts.ClassCodeSpbopt)
                    result = oper == CfgSourceEts.Buy ? sec.LastPrice + sec.MinStep * 20 : sec.LastPrice - sec.MinStep * 20;
            }

            return result;
        }



    }
}
