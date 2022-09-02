using Adapter.Model;
using CommonDataContract;
using CommonDataContract.AbstractDataTypes;
using SourceEts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TerminalRbkm.View.Table
{
    /// <summary>
    /// Interaction logic for TableDeals.xaml
    /// </summary>
    public partial class TableOrders
    {
        private static readonly DataStorage storage = DataStorage.Instance;
        MainWindow _main;
        public TableOrders(MainWindow main)
        {
            InitializeComponent();
            DataContext = this;
            _main = main;
            Orders = storage.OrdersList;
        }

        public ObservableList<IOrders> Orders { get; }

        private void MnitNewOrder_OnClick(object sender, RoutedEventArgs e)
        {
            OpenOrderForm(DtgdOrders.SelectedItem as IOrders);
        }

        private void DtgdOrders_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dtgd = sender as DataGrid;
            if (dtgd != null && dtgd.SelectedItem != null)
                OpenOrderForm(dtgd.SelectedItem as IOrders);
            else
                OpenOrderForm(null);
        }

        private SendOrder1 _sendOrder { get; set; }
        private void OpenOrderForm(IOrders order)
        {
            SendOrder(order);
        }

        private void SendOrder(IOrders order)
        {
            if (order == null)
            {
                SendOrder1 sendOrder = new SendOrder1(_main.AllTerminal);
                sendOrder.Show();
                _sendOrder = sendOrder;
            }

            if (order != null)
            {
                var win = _main.GetActiveWindowNewOrder("Новая заявка");
                if (win == null)
                {
                    SendOrder1 sendOrder = new SendOrder1(_main.AllTerminal, order.Symbol, order.ClientCode, order.Account,
                        order.Operation == CfgSourceEts.Buy, order.Quantity, order.Price);
                    sendOrder.Show();
                    _sendOrder = sendOrder;
                }
                else
                {
                    _sendOrder.UpdateData(order.Symbol, order.ClientCode, order.Account,
                        order.Operation == CfgSourceEts.Buy, order.Quantity, order.Price);
                }
            }
        }

        private void MintChangeOrder_OnClick(object sender, RoutedEventArgs e)
        {
            if (DtgdOrders.SelectedItem != null)
            {
                var item = DtgdOrders.SelectedItem as IOrders;
                KillOrder(item);
                SendOrder(item);
            }
        }


        private void MnitCancelOrder_OnClick(object sender, RoutedEventArgs e)
        {
            if (DtgdOrders.SelectedItem != null)
            {
                KillOrder(DtgdOrders.SelectedItem as IOrders);
            }
        }

        /// <summary>
        /// Снятие заявки
        /// </summary>
        /// <param name="item"></param>
        private void KillOrder(IOrders item)
        {
            if (item != null)// && item.Status == ConfigTermins.Active)
            {
                var param = GetParamOfTranscationModelForKillOrder(item);
                var absTerm = _main.AllTerminal.GetTerminal(param.Account, param.ClientCode);

                if (absTerm.IsConnect)
                    absTerm.TransactionOrder(param);
            }
        }


        /// <summary>
        /// Определение параметров для снятия заявки  
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ParamOfTransactionModel GetParamOfTranscationModelForKillOrder(IOrders order)
        {
            var paramOfTransaction = new ParamOfTransactionModel();
            paramOfTransaction.Account = order.Account;
            paramOfTransaction.ClientCode = order.ClientCode;
            paramOfTransaction.TrandId = "600";
            paramOfTransaction.OrderNumberForKill = order.Number;
            paramOfTransaction.OrderIdForKill = order.Id;
            paramOfTransaction.Symbol = order.Symbol;
            paramOfTransaction.ClassCode = _main.AllTerminal.GetClasscode(paramOfTransaction.Symbol);
            paramOfTransaction.TypeOrder = CfgSourceEts.TypeRequestForTransactionKillOrder;
            paramOfTransaction.Operation = order.Operation == CfgSourceEts.Buy ? 'B' : 'S';

            return paramOfTransaction;
        }


    }
}
