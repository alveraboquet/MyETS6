using SourceEts;
using SourceEts.Table.TableClass;

namespace TerminalRbkm.View.Table
{
    /// <summary>
    /// Interaction logic for AllTradesOrTick.xaml
    /// </summary>
    public partial class TableAllTradesOrTick 
    {
        private static readonly DataStorage storage = DataStorage.Instance;
        public TableAllTradesOrTick(MainWindow _main)
        {
            InitializeComponent();
            DataContext = this;
            //TradesOrTick = storage.LoadHisrotyTickCommonTable;

            // ???

            //if (absTerm is TransaqConnectorClass)
            //    dtgdSecurities.ItemsSource = absTerm.GetTableAllTradesOrTick();

            //if (absTerm is QuikClass)
            //    dtgdSecurities.ItemsSource = absTerm.GetTableAllTradesOrTick();

        }

        public ObservableList<LoadHisrotyTick> TradesOrTick { get; }
    }
}
