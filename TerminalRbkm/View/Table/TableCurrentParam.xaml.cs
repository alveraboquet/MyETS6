using Adapter;
using CommonDataContract.AbstractDataTypes;
using SourceEts;
using System.Windows.Controls;
using System.Windows.Input;
using TerminalRbkm.Infrastructure;

namespace TerminalRbkm.View.Table
{
    public partial class TableCurrentParam : UserControl
    {
        private MainWindow mainWindow;
        private AllTerminals allTerminal;
        private static readonly DataStorage storage = DataStorage.Instance;

        internal TableCurrentParam(MainWindow parent)
        {
            InitializeComponent();
            DataContext = this;
            mainWindow = parent;
            allTerminal = parent.AllTerminal;

            CurrentParams = storage.CurrentParamModelList;
            NewOrderCommand = new RelayCommand(OnNewOrderCommandExecute, CanNewOrderCommandExecuted);
        }

        public ObservableList<ISecurity> CurrentParams { get; }

        public ICommand NewOrderCommand { get; }
        private bool CanNewOrderCommandExecuted(object param) => true;
        private void OnNewOrderCommandExecute(object param)
        {
            OpenOrderForm(dtgdSecurities.SelectedItem as ISecurity);
        }

        private void DtgdSecurities_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dtgd = sender as DataGrid;
            if (dtgd != null)
                OpenOrderForm(dtgd.SelectedItem as ISecurity);
            else
                OpenOrderForm(null);
        }

        private SendOrder1 _sendOrder { get; set; }
        private void OpenOrderForm(ISecurity sec)
        {
            SendOrder(sec);
        }

        private void SendOrder(ISecurity sec)
        {
            if (sec == null)
            {
                SendOrder1 sendOrder = new SendOrder1(mainWindow.AllTerminal);
                sendOrder.Show();
                _sendOrder = sendOrder;
            }

            if (sec != null)
            {
                var win = mainWindow.GetActiveWindowNewOrder("Новая заявка");
                if (win == null)
                {
                    SendOrder1 sendOrder = new SendOrder1(mainWindow.AllTerminal, sec.Seccode, sec.ClassCode);
                    sendOrder.Show();
                    _sendOrder = sendOrder;
                }
                else
                {
                    _sendOrder.UpdateData(sec.Seccode, sec.ClassCode);
                }
            }
        }

        private void MnitNewOrder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
