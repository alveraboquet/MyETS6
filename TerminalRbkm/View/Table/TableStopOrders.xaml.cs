using CommonDataContract.AbstractDataTypes;
using Adapter;
using SourceEts;

namespace TerminalRbkm.View.Table
{
    /// <summary>
    /// Interaction logic for TableStopOrders.xaml
    /// </summary>
    public partial class TableStopOrders
        {
            private static readonly DataStorage storage = DataStorage.Instance;
            public TableStopOrders(AllTerminals allTerminals)
            {
                InitializeComponent();
                DataContext = this;
                //DtgdStopOrders.ItemsSource = absTerm.GetTableStopOrders();
                StopOrders = storage.StopOrderList;

                //TerminalChange.Instance.OnCnahgeTerminal -= ChangeTerminal;
                //TerminalChange.Instance.OnCnahgeTerminal += ChangeTerminal;
            }

            public ObservableList<IStop> StopOrders { get; }

        //    /// <summary>
        //    /// Происходит при смене терминала
        //    /// </summary>
        //    /// <param name="term"></param>
        //    private void ChangeTerminal(AbstractTerminal term)
        //    {
        //        DtgdStopOrders.ItemsSource = term.GetTableStopOrders();
        //    }


        //    private void TableStopOrders_OnClosing(object sender, CancelEventArgs e)
        //    {
        //        TerminalChange.Instance.OnCnahgeTerminal -= ChangeTerminal;
        //    }
    }
}
