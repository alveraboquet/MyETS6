using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Adapter;
using CommonDataContract.AbstractDataTypes;
using Adapter.Config;
using SourceEts;
using SourceEts.Table;

namespace TerminalRbkm.View.Table
{
    public partial class TableDeals 
    {
        MainWindow _main;
        private static readonly DataStorage storage = DataStorage.Instance;
        public TableDeals(MainWindow main)
        {
            InitializeComponent();
            DataContext = this;
            _main = main;
            Deals = storage.DealsList;
        }

        public ObservableList<IDeals> Deals { get; }


        ////private void TableDeals_OnClosing(object sender, CancelEventArgs e)
        ////{
        ////    TerminalChange.Instance.OnCnahgeTerminal -= ChangeTerminal;
        ////}


        private void MnitNewOrder_OnClick(object sender, RoutedEventArgs e)
        {
            OpenOrderForm(DtgdDeals.SelectedItem as IDeals);
        }

        private void DtgdDeals_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dtgd = sender as DataGrid;
            if (dtgd != null && DtgdDeals.SelectedItem != null)
                OpenOrderForm(DtgdDeals.SelectedItem as IDeals);
            else
                OpenOrderForm(null);
        }

        private SendOrder1 _sendOrder { get; set; }
        private void OpenOrderForm(IDeals deal)
        {
            SendOrder(deal);
        }

        private void SendOrder(IDeals deal)
        {
            if (deal == null)
            {
                SendOrder1 sendOrder = new SendOrder1(_main.AllTerminal);
                sendOrder.Show();
                _sendOrder = sendOrder;
            }

            if (deal != null)
            {
                var win = _main.GetActiveWindowNewOrder("Новая заявка");
                if (win == null)
                {
                    SendOrder1 sendOrder = new SendOrder1(_main.AllTerminal, deal.Symbol, deal.ClientCode, deal.Account,
                        deal.Operation == ConfigTermins.Buy, deal.Quantity, deal.Price);
                    sendOrder.Show();
                    _sendOrder = sendOrder;
                }
                else
                {
                    _sendOrder.UpdateData(deal.Symbol, deal.ClientCode, deal.Account,
                        deal.Operation == ConfigTermins.Buy, deal.Quantity, deal.Price);
                }
            }
        }



        /////// <summary>
        /////// Определение параметров для снятия заявки  
        /////// </summary>
        /////// <param name="order"></param>
        /////// <returns></returns>
        ////public ParamOfTransactionModel GetParamOfTranscationModelForKillOrder(IOrders order)
        ////{
        ////    var paramOfTransaction = new ParamOfTransactionModel();
        ////    paramOfTransaction.Account = order.Account;
        ////    paramOfTransaction.ClientCode = order.ClientCode;
        ////    paramOfTransaction.TrandId = 600;
        ////    paramOfTransaction.OrderNumberForKill = order.Number;
        ////    paramOfTransaction.OrderIdForKill = order.Id;
        ////    paramOfTransaction.Symbol = order.Symbol;
        ////    paramOfTransaction.ClassCode = _absTerm.GetClasscode(paramOfTransaction.Symbol);
        ////    paramOfTransaction.TypeOrder = ConfigTermins.TypeRequestForTransactionKillOrder;

        ////    return paramOfTransaction;
        ////}

    }
}
