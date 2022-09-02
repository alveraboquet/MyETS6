using CommonDataContract.AbstractDataTypes;
using Adapter;
using SourceEts;

namespace TerminalRbkm.View.Table
{
    /// <summary>
    /// Логика взаимодействия для TableMoneyShares.xaml
    /// </summary>
    public partial class TableMoneyShares 
    {
        private static readonly DataStorage storage = DataStorage.Instance;
        public TableMoneyShares(AllTerminals allTerminal)
        {
            InitializeComponent();
            //DataContext = this;
            //DtgdLimtiMoneyShares.ItemsSource = absTerm.GetTableLimitMoneyShares();
            DtgdLimtiMoneyShares.ItemsSource = storage.LimitMoneySharesList;


        }

        public ObservableList<IMoneyShares> MoneyShares { get; }



        //private void TableMoneyShares_OnClosing(object sender, CancelEventArgs e)
        //{
        //    TerminalChange.Instance.OnCnahgeTerminal -= ChangeTerminal;
        //}
    }
}
