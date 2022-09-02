using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Adapter;
using ClassControlsAndStyle.Dialogs;
using SourceEts.Terminals;
using SourceEts.UserConnector;

namespace TerminalRbkm.View
{
    /// <summary>
    /// Логика взаимодействия для TerminalSettingNew.xaml
    /// </summary>
    public partial class TerminalSettingNew
    {
        private AllTerminals allTerminals;
        private ObservableCollection<ITerminalInfo> _colTerminals = new ObservableCollection<ITerminalInfo>();
        private MainWindow mainWindow;
        public TerminalSettingNew(MainWindow parent)
        {
            InitializeComponent();
            mainWindow = parent;
            allTerminals = mainWindow.AllTerminal;

            SetSettingsTerminal();
            DtgdTerminals.ItemsSource = _colTerminals;

            if (allTerminals.CryptoAdapters.Count > 0)
            {
                for (int i = 0; i < allTerminals.CryptoAdapters.Count; i++)
                {
                    MenuItem item = new MenuItem();
                    item.Header = allTerminals.CryptoAdapters[i].NameUserAdapter;
                    item.Foreground = new SolidColorBrush(Colors.Black);

                    MnitCryptoConnectors.Items.Add(item);
                }
            }
        }


        private void SetSettingsTerminal()
        {
            foreach (var cryptoCon in allTerminals.CryptoAdapters)
            {
                var listTerm = cryptoCon.GetTerminalSetting();
                for (int i = 0; i < listTerm.Count; i++)
                {
                    if (listTerm[i] != null)
                        _colTerminals.Add(listTerm[i]);
                    else
                    {
                        cryptoCon.DelTerminalSetting(listTerm[i]);
                    }
                }

            }
        }





        private void BtnAddRow_Click(object sender, RoutedEventArgs e)
        {
            if (allTerminals.IsPushConnect)
            {
                new DialogMessage(
                    "Редактирование, удаление и добавление новых соединений, возможно только при отключенном соединении",
                    "Внимание!");
                return;
            }
            string terminal = (e.Source as MenuItem).Header.ToString();

            for (int i = 0; i < allTerminals.CryptoAdapters.Count; i++)
            {
                if (terminal == allTerminals.CryptoAdapters[i].NameUserAdapter)
                {


                    var item = new TerminalInfo();
                    foreach (var itemTerm in allTerminals.CryptoAdapters[i].UserConSetting)
                    {
                        item.UserConSettings.Add(new SettingUserCon(itemTerm));
                    }
                    item.AllInstruments = allTerminals.CryptoAdapters[i].AllInstrForTrwModelList;
                    item.AvalibleInstruments = allTerminals.CryptoAdapters[i].AvalibleInstruments;

                    AddNewTerminal add = new AddNewTerminal(mainWindow, item, allTerminals.CryptoAdapters[i]);
                    if (add.ShowDialog() == true)
                    {
                        _colTerminals.Add(item);
                        allTerminals.CryptoAdapters[i].AddTerminalSetting(item);
                        allTerminals.CryptoAdapters[i].SaveSettings();

                    }
                }
            }

        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            if (allTerminals.IsPushConnect)
            {
                new DialogMessage(
                    "Редактирование, удаление и добавление новых соединений, возможно только при отключенном соединении",
                    "Внимание!");
                return;
            }

            if (DtgdTerminals.SelectedItem == null)
                return;
            if (new DialogOkCancel("Вы действительно хотите удалить выделенные соединения?",
                "Внимание!").Result != MessageBoxResult.OK)
                return;
            if (DtgdTerminals.SelectedItems.Count >= 1)
            {
                var count = DtgdTerminals.SelectedItems.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (DtgdTerminals.SelectedItem is TerminalInfo)
                    {
                        var item = DtgdTerminals.SelectedItem as TerminalInfo;

                        for (int j = 0; j < allTerminals.CryptoAdapters.Count; j++)
                        {
                            if (item.Terminal == allTerminals.CryptoAdapters[j].NameUserAdapter)
                            {
                                allTerminals.CryptoAdapters[j].DelTerminalSetting(DtgdTerminals.SelectedItems[i] as ITerminalInfo);
                                _colTerminals.Remove(DtgdTerminals.SelectedItems[i] as ITerminalInfo);
                                allTerminals.CryptoAdapters[i].SaveSettings();

                            }
                        }
                    }
                }
            }
        }

        private void DtgdTerminals_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = (sender as DataGrid);
            if (grid == null || grid.SelectedItem == null)
                return;

            if (grid.SelectedItem is TerminalInfo)
            {
                var item = grid.SelectedItem as TerminalInfo;

                for (int i = 0; i < allTerminals.CryptoAdapters.Count; i++)
                {

                    if (item.Terminal == allTerminals.CryptoAdapters[i].NameUserAdapter)
                    {
                        AddNewTerminal add = new AddNewTerminal(mainWindow, item, allTerminals.CryptoAdapters[i]);
                        add.ShowDialog();
                        allTerminals.CryptoAdapters[i].SaveSettings();

                    }
                }
                return;
            }

        }

        private void MnitEditQuik_OnClick(object sender, RoutedEventArgs e)
        {


            if (DtgdTerminals.SelectedItem == null)
                return;

          
            if (DtgdTerminals.SelectedItem is TerminalInfo)
            {
                var item = DtgdTerminals.SelectedItem as TerminalInfo;

                for (int i = 0; i < allTerminals.CryptoAdapters.Count; i++)
                {
                    if (item.Terminal == allTerminals.CryptoAdapters[i].NameUserAdapter)
                    {
                        AddNewTerminal add = new AddNewTerminal(mainWindow, item, allTerminals.CryptoAdapters[i]);
                        add.ShowDialog();
                        allTerminals.CryptoAdapters[i].SaveSettings();
                    }
                }

                return;
            }


        
        }

        private void MnitDel_Click(object sender, RoutedEventArgs e)
        {
            if (allTerminals.IsPushConnect)
            {
                new DialogMessage(
                    "Редактирование, удаление и добавление новых соединений, возможно только при отключенном соединении",
                    "Внимание!");
                return;
            }

            if (DtgdTerminals.SelectedItem == null)
                return;
            if (new DialogOkCancel("Вы действительно хотите удалить данное соединения?",
    "Внимание!").Result == MessageBoxResult.OK)

                if (DtgdTerminals.SelectedItems.Count == 1)
                {
                    var count = DtgdTerminals.SelectedItems.Count;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        

                        if (DtgdTerminals.SelectedItem is TerminalInfo)
                        {
                            var item = DtgdTerminals.SelectedItem as TerminalInfo;

                            for (int j = 0; j < allTerminals.CryptoAdapters.Count; j++)
                            {
                                if (item.Terminal == allTerminals.CryptoAdapters[j].NameUserAdapter)
                                {
                                    allTerminals.CryptoAdapters[j].DelTerminalSetting(DtgdTerminals.SelectedItems[i] as ITerminalInfo);
                                    _colTerminals.Remove(DtgdTerminals.SelectedItems[i] as ITerminalInfo);
                                    allTerminals.CryptoAdapters[i].SaveSettings();
                                }
                            }
                        }
                    }
                }
        }


        private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (allTerminals.IsPushConnect)
            {
                allTerminals.Raise_AddMessage("Редактирование, удаление и добавление новых соединений, возможно только при отключенном соединении",
                    "Внимание!");
                e.Handled = true;
            }
        }

    }
}
