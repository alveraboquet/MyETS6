using CommonDataContract.AbstractDataTypes;
using Adapter;
using SourceEts;

namespace TerminalRbkm.View.Table
{
    public partial class TableMoneyFutures
    {
        private static readonly DataStorage storage = DataStorage.Instance;
        public TableMoneyFutures(AllTerminals allTerminal)
        {
            InitializeComponent();
            DataContext = this;
            //_absTerm = absTerm;
            //DtgdLimtiMoneyFutures.ItemsSource = absTerm.GetTableLimitMoneyFurures();
            MoneyFutures = storage.LimitMoneyFuturesList;

            //TerminalChange.Instance.OnCnahgeTerminal -= ChangeTerminal;
            //TerminalChange.Instance.OnCnahgeTerminal += ChangeTerminal;
        }

        public ObservableList<IMoneyFutures> MoneyFutures { get; }

        ///// <summary>
        ///// Происходит при смене терминала
        ///// </summary>
        ///// <param name="term"></param>
        //private void ChangeTerminal(AbstractTerminal term)
        //{
        //    DtgdLimtiMoneyFutures.ItemsSource = term.GetTableLimitMoneyFurures();
        //    _absTerm = term;

        //}


        //private void TableMoneyFutures_OnClosing(object sender, CancelEventArgs e)
        //{
        //    TerminalChange.Instance.OnCnahgeTerminal -= ChangeTerminal;
        //}
    }
}
