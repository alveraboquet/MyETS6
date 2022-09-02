using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Adapter;
using CommonDataContract.AbstractDataTypes;
using SourceEts;
using SourceEts.Table;

namespace TerminalRbkm.View.Table
{
    /// <summary>
    /// Interaction logic for TableCurretnParam.xaml
    /// </summary>
    public partial class TablePositionShares
    {
        private static readonly DataStorage storage = DataStorage.Instance;
        MainWindow _main;
        public TablePositionShares(MainWindow main)
        {
            InitializeComponent();
            DataContext = this;
            _main = main;
            //dtgdPosShares.ItemsSource = _main.AllTerminal.PositionSharesList;
            PositionShares = storage.PositionSharesList;
        }

        public ObservableList<IPositionShares> PositionShares { get; }

        private void MnitNewOrder_OnClick(object sender, RoutedEventArgs e)
        {
            OpenOrderForm(dtgdPosShares.SelectedItem as IPositionShares);
        }

        private void DtgdPosShares_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dtgd = sender as DataGrid;
            if (dtgd != null)
                OpenOrderForm(dtgdPosShares.SelectedItem as IPositionShares);
            else
                OpenOrderForm(null);
        }

        private SendOrder1 _sendOrder { get; set; }
        private void OpenOrderForm(IPositionShares sec)
        {
            SendOrder(sec);
        }

        private void SendOrder(IPositionShares sec)
        {
            if (sec == null)
            {
                SendOrder1 sendOrder = new SendOrder1(_main.AllTerminal);
                sendOrder.Show();
                _sendOrder = sendOrder;
            }

            if (sec != null)
            {
                var win = _main.GetActiveWindowNewOrder("Новая заявка");
                if (win == null)
                {
                    SendOrder1 sendOrder = new SendOrder1(_main.AllTerminal, sec.Symbol, sec.ClientCode, sec.Account);
                    sendOrder.Show();
                    _sendOrder = sendOrder;
                }
                else
                {
                    _sendOrder.UpdateData(sec.Symbol, sec.ClientCode, sec.ClientCode);
                }
            }
        }

    }

}
