using Adapter;
using Adapter.Config;
using ClassControlsAndStyle.Dialogs;
using CryptoCon;
using ModulSolution.View;
using NLog;
using ScriptSolution.Model;
using SourceEts.UserConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TerminalRbkm.Config;
using TerminalRbkm.Infrastructure;
using TerminalRbkm.View;
using TerminalRbkm.View.Table;

namespace TerminalRbkm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //#region Events
        public static Logger Log;
        public Dispatcher dispatcher;
        private DispatcherTimer _timer = new DispatcherTimer();
        List<string> _listWindow = new List<string>();
        public AllTerminals AllTerminal = new AllTerminals();


        ///**********************************************************
        #region NEW CODE
        public event PropertyChangedEventHandler PropertyChanged;
        private TabModel selectedTab;
        public ObservableCollection<TabModel> Tabs { get; set; } = new ObservableCollection<TabModel>();
        public TabModel SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTab)));
            }
        }

        private SolidColorBrush tabBackground = new SolidColorBrush()
        {
            Color = new Color() { R = 145, G = 158, B = 167 }
        };

        public ICommand OpenNewTabCommand { get; }
        private bool CanOpenNewTabCommandExecuted(object param) => true;
        private void OnOpenNewTabCommandExecute(object param)
        {
            string tabHeader = param as string;

            TabModel tab = Tabs.FirstOrDefault(item => item.Header == tabHeader);

            if (tab != null)
            {
                SelectedTab = tab;
                return;
            }

            UserControl uc = null;

            #region Settings

            if (tabHeader == Cfg.MainTerminalsSetting)
            {
                uc = new TerminalSettingNew(this);
            }

            #endregion

            #region Table

            if (tabHeader == ConfigTermins.MainTableTtp)
            {
                uc = new TableCurrentParam(this) { Name = "TableTTPTrCon", Background = tabBackground };
            }

            if (tabHeader == ConfigTermins.MainTableAllTrades)
            {
                uc = new TableAllTradesOrTick(this) { Name = "TableAllTradesOrTick", Background = tabBackground };
            }
            if (tabHeader == ConfigTermins.MainTablePositionMmvb)
            {
                uc = new TablePositionShares(this) { Name = "TablePositionShares", Background = tabBackground };
            }

            if (tabHeader == ConfigTermins.MainTableFortsPosition)
            {
                uc = new TablePositionFutures(this) { Name = "TablePositionFutures", Background = tabBackground };
            }

            if (tabHeader == ConfigTermins.MainTableLimitMoneyShares)
            {
                uc = new TableMoneyShares(AllTerminal) { Name = "TableMoneyShares", Background = tabBackground };
            }
            if (tabHeader == ConfigTermins.MainTableLimitMoneyFutures)
            {
                uc = new TableMoneyFutures(AllTerminal) { Name = "TableMoneyFutures", Background = tabBackground };
            }

            if (tabHeader == ConfigTermins.MainTableDeal)
            {
                uc = new TableDeals(this) { Name = "TableDeals", Background = tabBackground };
            }

            if (tabHeader == ConfigTermins.MainTableOrder)
            {
                uc = new TableOrders(this) { Name = "TableOrders", Background = tabBackground };
            }

            if (tabHeader == ConfigTermins.MainTableStopOrder)
            {
                uc = new TableStopOrders(AllTerminal) { Name = "TableStopOrders", Background = tabBackground };
            }
            #endregion

            tab = new TabModel(uc, tabHeader);
            Tabs.Add(tab);
            SelectedTab = tab;

            if (_isLoadWindow)
            {
                try
                {
                    //SetWindow();
                }
                catch (Exception ex)
                {
                    AllTerminal.Raise_OnSomething("Ошибка сохранения при открытии окна: " + ex.Message);
                }
            }
        }

        public ICommand CloseTabCommand { get; }
        private bool CanCloseTabCommandExecuted(object param) => Tabs.Count > 1;
        private void OnCloseTabCommandExecute(object param)
        {
            string header = param as string;

            TabModel found = Tabs.FirstOrDefault(item => item.Header == header);
            _ = Tabs.Remove(found);
            if (Tabs.Count > 0)
            {
                SelectedTab = Tabs[Tabs.Count - 1];
            }
        }

        public ICommand ConnectCommand { get; }
        private bool CanConnectCommandExecuted(object param) => true;
        private void OnConnectCommandExecute(object param)
        {
            int countConnectors = 0;

            #region Формирование надписи для текущих соединений
            bool isQuik = false;
            bool isTrCon = false;
            bool isPlaz2 = false;
            bool isIbTws = false;
            bool isExante = false;
            bool isIqFeed = false;
            bool isUserCon = false;

            var list = AllTerminal.GetAllTerminals();
            string msg = "";

            foreach (var item in AllTerminal.CryptoAdapters)
            {
                foreach (var itemTerm in item.ListTerminalInfo)
                {
                    if (itemTerm.IsUse)
                    {
                        msg = msg + itemTerm.Terminal + "; ";
                        countConnectors += 1;
                        break;
                    }
                }
            }

            if (msg.Split(';').Count() == 1)
                msg = msg.Replace(";", "");

            Properties.Settings.Default.adapter = msg;

            if (list.Count == 0)
            {
                new DialogMessage("Необходимо добавить соединение с терминалом или биржей", "Внимание");
                CheckOpenDocument(Cfg.MainTerminalsSetting);
                return;
            }


            if (String.IsNullOrEmpty(msg))
            {
                new DialogMessage("Не выбрано ни одного соединения.", "Внимание");
                CheckOpenDocument(Cfg.MainTerminalsSetting);
                return;
            }

            #endregion



            _lastPushBtnConnect = DateTime.Now;

            AllTerminal.IsPushConnect = true;

            foreach (var item in AllTerminal.CryptoAdapters)
            {
                item.IsPushBtnConnect = true;
            }

            //SetEnableControl(true);

            AllTerminal.Connect();
        }

        #endregion
        ///**************************************************************


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            dispatcher = Dispatcher.CurrentDispatcher;

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);

            OpenNewTabCommand = new RelayCommand(OnOpenNewTabCommandExecute, CanOpenNewTabCommandExecuted);
            CloseTabCommand = new RelayCommand(OnCloseTabCommandExecute, CanCloseTabCommandExecuted);
            ConnectCommand = new RelayCommand(OnConnectCommandExecute, CanConnectCommandExecuted);

            TabModel messages = new TabModel(new TableMessages(LogsCollection), "Сообщения");

            Tabs.Add(messages);
            SelectedTab = messages;
        }


        private bool _initialLoadEts; //первичный запуск ЕТС
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            //CreatLog();

            try
            {

                LoadCryptoMarket();

                _initialLoadEts = false;
                int countFilesAndDir = 0;
                if (Directory.Exists(Properties.Settings.Default.PathSaveSetting))
                {
                    var dir = new DirectoryInfo(Properties.Settings.Default.PathSaveSetting);

                    var files = dir.GetFiles();
                    var direct = dir.GetDirectories();
                    countFilesAndDir = files.Count() + direct.Count();
                    if (countFilesAndDir <= 2)
                        _initialLoadEts = true;

                }

                if (String.IsNullOrEmpty(Properties.Settings.Default.PathSaveSetting))
                {
                    Properties.Settings.Default.PathSaveSetting =
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ConnectroSample";
                    Properties.Settings.Default.Save();
                    if (!Directory.Exists(Properties.Settings.Default.PathSaveSetting))
                        _initialLoadEts = true;
                }

                Title = "ConnectorsSample v." +
                                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                AllTerminal.MainDispatcher = dispatcher;
                CreatAllTerminals();



                _listWindow = GetWindow();


                foreach (var item in _listWindow)
                {
                    OnOpenNewTabCommandExecute(item);
                }



                //цикл для оборажение текущего времени сервера
                _timer.Interval = new TimeSpan(0, 0, 1);
                _timer.Start();
                _timer.Tick += SetTime;



                _isLoadWindow = true;

            }
            catch (Exception ex)
            {
                _error = true;
                new DialogOkCancel("При инициализации произоша ошибка: " + ex.Source, "Ошибка");
            }

        }


        private bool _error = false;

        bool _isLoadWindow = false;

        CfgCryptoCon _cfgCryptoCon = new CfgCryptoCon();

        /// <summary>
        /// Подключение крипто рынков
        /// </summary>
        private void LoadCryptoMarket()
        {
            _cfgCryptoCon.CreateListConnectors();
            foreach (var item in _cfgCryptoCon.Connectors)
            {
                AllTerminal.CryptoAdapters.Add(item);
            }
        }


        #region Создание терминала и подписка на события терминалов и поставщиков данных


        private void OpenStakan(object sender, RoutedEventArgs e)
        {
            Stakan stakan = new Stakan(AllTerminal);
            stakan.Show();
        }

        private void CreatAllTerminals()
        {
            try
            {
                foreach (var item in AllTerminal.CryptoAdapters)
                {
                    item.BaseMon.OnConnectionResultEvent += SetEnableConnctBtn;
                    item.PathSave = Properties.Settings.Default.PathSaveSetting;
                    SetEvents(item);
                    foreach (var termInfo in item.ListTerminalInfo)
                    {
                        (termInfo as TerminalInfo).AllInstruments = item.AllInstrForTrwModelList;
                        (termInfo as TerminalInfo).AvalibleInstruments = item.AvalibleInstruments;
                    }
                    item.ExecuteUserConnector(Properties.Settings.Default.PathSaveSetting);
                }
            }
            catch (Exception ex)
            {
                new DialogOkCancel("Ошибка создание адаптеров: " + ex.Source, "Ошибка");
            }
        }


        private void SetEvents(AbstractTerminal terminal)
        {
            terminal.BaseMon.OnWriteToLog += Writing;
            terminal.BaseMon.OnSomething += Loging;
            //terminal.BaseMon.OnAddAllTradeDealsOrTick += JobAtTable.AllTradeDealsOrTickTable;

            terminal.SetTableAllTerminals(AllTerminal);
            terminal.LoadSettings();
        }

        #endregion

        #region Логгирование

        /// <summary>
        /// Запись данных в текстовый файл
        /// </summary>
        /// <param name="s"></param>
        public void Writing(string s)
        {
            Cfg.SetLog(s);
        }

        public ObservableCollection<PersonalLog> LogsCollection = new ObservableCollection<PersonalLog>();

        /// <summary>
        /// Вывод в лог, событий происходящих в терминалах
        /// </summary>
        /// <param name="s"></param>
        public void Loging(string s)
        {
            //!!!!!!!!!!!!!!!!!!!!!!!!!!Необхоидмо передвавать PersonalLog
            if (!this.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    LogsCollection.Insert(0, new PersonalLog()
                    {
                        NumberMessage = LogsCollection.Count + 1,
                        Message = s,
                        DateTimeMessage = DateTime.Now
                    });
                    TxbxStatus.Text = DateTime.Now + " " + s;
                }
                ), DispatcherPriority.Normal, null);
            }
            else
            {
                LogsCollection.Insert(0, new PersonalLog()
                {
                    NumberMessage = LogsCollection.Count + 1,
                    Message = s,
                    DateTimeMessage = DateTime.Now
                });
                TxbxStatus.Text = DateTime.Now + " " + s;
            }


            //AllTerminal.Raise_AlertSignals(ConfigTermins.NoticePesonalLogs, new NoticeModel
            //{
            //    Comment = s,
            //    Time = AllTerminal.TimeServer,
            //    NameRobot = brmModel.Name,
            //    NameStrategy = brmModel.NameScript,
            //    IsModul = brmModel.IsDemka || brmModel.IsLicens
            //});
            //Cfg.SetLog(s);
        }


        //NLog.Targets.FileTarget tar = (NLog.Targets.FileTarget)LogManager.Configuration.FindTargetByName("run_log");
        //NLog.Targets.FileTarget tarInfo = (NLog.Targets.FileTarget)LogManager.Configuration.FindTargetByName("info_log");
        /// <summary>
        /// Создание логов
        /// </summary>
        //private void CreatLog()
        //{
        //    try
        //    {
        //        Log = LogManager.GetCurrentClassLogger();


        //        Log.Trace("Version: {0}", Environment.Version.ToString());
        //        Log.Trace("OS: {0}", Environment.OSVersion.ToString());
        //        Log.Trace("Command: {0}", Environment.CommandLine.ToString());
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show("Ошибка работы с логом!\n" + e.Message);
        //    }

        //    AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        //    LogManager.Flush();


        //    tar.KeepFileOpen = false;
        //    tarInfo.KeepFileOpen = false; //держим файл постоянно открытым, чтоб увеличить производительность

        //    tar.DeleteOldFileOnStartup = false;
        //    tarInfo.DeleteOldFileOnStartup = false;
        //    if (!Directory.Exists(Properties.Settings.Default.PathSaveSetting + "\\Logs"))
        //    {
        //        DirectoryInfo dir = new DirectoryInfo(Properties.Settings.Default.PathSaveSetting + "\\Logs");
        //        dir.Create();
        //    }

        //    tarInfo.FileName = Properties.Settings.Default.PathSaveSetting + "\\Logs\\Info " + DateTime.Now.ToShortDateString() + ".log";
        //    tar.FileName = Properties.Settings.Default.PathSaveSetting + "\\Logs\\Application " + DateTime.Now.ToShortDateString() + ".log";
        //    Log.Info("Включение приложения ConnectorSmaple v." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        //}


        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal("Unhandled exception: {0}", e.ExceptionObject);
            LogManager.Flush();
        }




        #endregion

        #region Работа с новыми окнами

        //public bool CheckWindowForUserModul(string name)
        //{
        //    if (!_isLoadWindow)
        //    {
        //        return GetWindow().Contains(name);
        //    }

        //    return CheckOpenDocumentBool(name);
        //}

        public List<string> GetWindow()
        {
            return (List<string>)XmlHistory.GetXmlData(XmlHistory.FilenameWindow);
        }

        //private void CreateNewDocument(object sender, RoutedEventArgs e)
        //{
        //    PublishNewDocument(((MenuItem)sender).Header.ToString());
        //    //CheckOpenDocument(((MenuItem)sender).Header.ToString());

        //}

        /// <summary>
        /// Проверяем открытые DocumentContent и в случае отсутствия добавляем
        /// </summary>
        /// <param name="sender">name DocumentContent</param>
        public void CheckOpenDocument(string sender)
        {

            string baseDocTitle = sender;
            //for (int i = 0; i < DockManager.Documents.Count; i++)
            //{
            //    if (DockManager.Documents[i].Title == baseDocTitle)
            //    {
            //        DockManager.Documents[i].Activate();
            //        return;
            //    }
            //}
            //var panel = FindPanel(baseDocTitle);

            //if (panel != null)
            //{
            //    panel.IsActive = true;
            //    return;
            //}

            //PublishNewDocument(baseDocTitle);
        }

        ///// <summary>
        ///// Проверка открытых окон для модулей
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <returns></returns>
        //public bool CheckOpenDocumentBool(string namePanel)
        //{
        //    bool result = true;
        //    var panel = FindPanel(namePanel);

        //    if (panel != null)
        //    {
        //        panel.IsActive = true;
        //        return false;
        //    }

        //    //string baseDocTitle = sender;
        //    //for (int i = 0; i < DockManager.Documents.Count; i++)
        //    //{
        //    //    if (DockManager.Documents[i].Title == baseDocTitle)
        //    //    {
        //    //        DockManager.Documents[i].Activate();

        //    //        result = false;
        //    //    }
        //    //}
        //    return result;
        //}

        ///// <summary>
        ///// Поиск панели
        ///// </summary>
        ///// <param name="name"></param>
        //internal DocumentPanel FindPanel(string name)
        //{
        //    if (DlmMain.LayoutRoot.Items.Count > 0)
        //    {
        //        foreach (var item in DlmMain.LayoutRoot.Items)
        //        {
        //            var itemGroup = item as DocumentGroup;
        //            if (itemGroup != null)
        //                foreach (var itempanel in itemGroup.Items)
        //                {
        //                    if (itempanel.Caption != null)
        //                    {
        //                        if (itempanel.Caption.ToString() == name)
        //                            return itempanel as DocumentPanel;
        //                    }
        //                }
        //        }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Поиск панели
        ///// </summary>
        ///// <param name="name"></param>
        //internal void ClosePanel(string name)
        //{
        //    if (DlmMain.LayoutRoot.Items.Count > 0)
        //    {
        //        foreach (var item in DlmMain.LayoutRoot.Items)
        //        {
        //            var itemGroup = item as DocumentGroup;
        //            if (itemGroup != null)
        //                foreach (var itempanel in itemGroup.Items)
        //                {
        //                    if (itempanel.Caption != null)
        //                    {
        //                        if (itempanel.Caption.ToString() == name)
        //                        {

        //                            itemGroup.Remove(itempanel);

        //                            //itempanel.Closed = true;
        //                            break;
        //                        }
        //                    }
        //                }
        //        }
        //    }
        //}




        /// <summary>
        /// Creat list documentContent for save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveAll(true);
            //Properties.Settings.Default.Top = (int)Top;
            //Properties.Settings.Default.Left = (int)Left;
            //Properties.Settings.Default.Height = (int)Height;
            //Properties.Settings.Default.Width = (int)Width;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Save open documentContent
        /// </summary>
        public void SetWindow()
        {
            _listWindow.Clear();
            var panels = Tabs.ToList();


            foreach (var item in panels)
            {
                if (item as TabModel != null)
                    if (!String.IsNullOrEmpty((item as TabModel).Header.ToString()))
                        _listWindow.Add((item as TabModel).Header.ToString());
            }
            XmlHistory.SetXmlData(XmlHistory.FilenameWindow, (Object)(_listWindow));

        }

        #endregion

        #region Подключение, отключение
        public void SetEnableConnctBtn()
        {
            try
            {
                bool existConnect = false;
                bool existConnectCrush = false;
                var color = Brushes.DarkOrange;

                foreach (var info in AllTerminal.AllTerminalInfos)
                {
                    if (info.IsUse && !info.IsConnect)
                        existConnectCrush = true;
                    if (info.IsUse && info.IsConnect)
                        existConnect = true;

                    if (!AllTerminal.IsPushConnect)
                        info.Comment = "";
                }
                if (AllTerminal.IsPushConnect)
                {
                    if (existConnect)
                        color = Brushes.LimeGreen;
                    if (existConnectCrush)
                        color = Brushes.Yellow;
                }



                //if (Dispatcher.CheckAccess())
                //{
                //    LblStatusConnected.Foreground = color;
                //    LblTypeAdapter.Foreground = color;

                //}
                //else
                //{
                //    Dispatcher.Invoke(new Action(delegate
                //    {
                //        LblStatusConnected.Foreground = color;
                //        LblTypeAdapter.Foreground = color;
                //    }));
                //}
            }
            catch (Exception)
            {

            }
        }


        private DateTime _lastPushBtnConnect = new DateTime();
        /// <summary>
        /// создание транзакционного соединия 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BtnConnect_Click(object sender, RoutedEventArgs e)
        {

            int countConnectors = 0;

            #region Формирование надписи для текущих соединений
            bool isQuik = false;
            bool isTrCon = false;
            bool isPlaz2 = false;
            bool isIbTws = false;
            bool isExante = false;
            bool isIqFeed = false;
            bool isUserCon = false;

            var list = AllTerminal.GetAllTerminals();
            string msg = "";

            foreach (var item in AllTerminal.CryptoAdapters)
            {
                foreach (var itemTerm in item.ListTerminalInfo)
                {
                    if (itemTerm.IsUse)
                    {
                        msg = msg + itemTerm.Terminal + "; ";
                        countConnectors += 1;
                        break;
                    }
                }
            }

            if (msg.Split(';').Count() == 1)
                msg = msg.Replace(";", "");

            Properties.Settings.Default.adapter = msg;

            if (list.Count == 0)
            {
                new DialogMessage("Необходимо добавить соединение с терминалом или биржей", "Внимание");
                CheckOpenDocument(Cfg.MainTerminalsSetting);
                return;
            }


            if (String.IsNullOrEmpty(msg))
            {
                new DialogMessage("Не выбрано ни одного соединения.", "Внимание");
                CheckOpenDocument(Cfg.MainTerminalsSetting);
                return;
            }

            #endregion



            _lastPushBtnConnect = DateTime.Now;

            AllTerminal.IsPushConnect = true;

            foreach (var item in AllTerminal.CryptoAdapters)
            {
                item.IsPushBtnConnect = true;
            }

            SetEnableControl(true);

            AllTerminal.Connect();


        }

        private void SetEnableControl(bool tmp)
        {
            BtnConnect.IsEnabled = !tmp;
            BtnDisconnect.IsEnabled = tmp;
            MenuItemRunConnect.IsEnabled = !tmp;
            MenuItemRunDisconnect.IsEnabled = tmp;
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AllTerminal.IsPushConnect = false;

                foreach (var item in AllTerminal.CryptoAdapters)
                {
                    item.IsPushBtnConnect = false;
                }
                AllTerminal.Disconnect();
                SetEnableControl(false);
            }
            catch (Exception ex)
            {
                Log.Error("Ошибка. Отключение:" + ex.Message);
            }
        }
        #endregion

        #region Работа с пунктами меню и элементами формы



        bool errorLoadMemory = false;



        /// <summary>
        /// Время сервера узнаем
        /// </summary>
        /// <param name = "sender" ></ param >
        /// < param name="e"></param>
        private void SetTime(object sender, EventArgs e)
        {
            DateTime time = AllTerminal.GetServerTime();
            if (AllTerminal.IsPushConnect)
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(new Action(() => LblTime.Content = time.ToLongTimeString()),
                        DispatcherPriority.Normal, null);

                }
                else
                {
                    // LblTime.Content = time.ToLongTimeString();
                    LblTime.Content = time.ToString();

                }
            }
            else
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(new Action(() => LblTime.Content = "(Локальное время) " + DateTime.Now.ToLongTimeString()),
                        DispatcherPriority.Normal, null);

                }
                else
                {
                    LblTime.Content = "(Локальное время) " + DateTime.Now.ToLongTimeString();

                }
            }

            string load = "";
            try
            {
                if (!errorLoadMemory)
                {
                    string prcName = Process.GetCurrentProcess().ProcessName;
                    var counter = new PerformanceCounter("Process", "Working Set - Private", prcName);
                    //MainWindow.Log.Info("Запись2");
                    load = " (" + (counter.RawValue / 1024 / 1024).ToString() + " MB)";
                }
            }
            catch (Exception)
            {
                errorLoadMemory = true;
            }



            MenuItemRunConnect.IsEnabled = GridConnection.IsEnabled && BtnConnect.IsEnabled;

            //if (tarInfo.FileName.ToString().Replace("'", "") != (Properties.Settings.Default.PathSaveSetting + "\\Logs\\Info " + DateTime.Now.ToShortDateString() + ".log"))
            //{
            //    tarInfo.FileName = Properties.Settings.Default.PathSaveSetting + "\\Logs\\Info " +
            //                       DateTime.Now.ToShortDateString() + ".log";
            //    tar.FileName = Properties.Settings.Default.PathSaveSetting + "\\Logs\\Application " +
            //                   DateTime.Now.ToShortDateString() + ".log";
            //}


        }


        /// <summary>
        /// Дата последнего сохранения, чтоб сохранялось все при переходе через ночь
        /// </summary>
        private DateTime _lastDateSave { get; set; }



        /// <summary>
        /// Открываем папку с логами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_LogFolder_Click(object sender, RoutedEventArgs e)
        {
            String path = Properties.Settings.Default.PathSaveSetting + "\\Logs";
            if (Directory.Exists(path))
                Process.Start("explorer", path);
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if ((new DialogYesNo("Вы действительно хотите выйти?", "Внимание!")).Result == MessageBoxResult.Yes)
            {
                foreach (Window f in Application.Current.Windows)
                {
                    if (!f.Title.Contains("ConnectorsSample "))
                        f.Close();
                }

                if (AllTerminal.IsPushConnect)
                    AllTerminal.Disconnect();
                //Log.Info("Выключение приложения ConnectorSmaple v." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

                //SaveAll(true);
                ClearLogs();

                Properties.Settings.Default.Top = (int)Top;
                Properties.Settings.Default.Left = (int)Left;
                Properties.Settings.Default.Height = (int)Height;
                Properties.Settings.Default.Width = (int)Width;
                Properties.Settings.Default.Save();

                //if ((new DialogYesNo("Сохранить данные?", "Внимание!")).Result == MessageBoxResult.Yes)
                //{
                //    _history.Save();
                //}
                //TrayIcon.Visible = false;
            }
            else
                e.Cancel = true;

        }


        internal Window GetActiveWindowNewOrder(string title)
        {
            foreach (Window f in Application.Current.Windows)
            {
                if (f.Title == title)
                {
                    return f;
                }
            }

            return null;

        }



        #endregion

        #region Сохранение данных и загрузка данных

        #region Сохранение данных
        private void MenuExitClick(object sender, RoutedEventArgs e)
        {
            if ((new DialogYesNo("Вы действительно хотите выйти?", "Внимание!")).Result == MessageBoxResult.Yes)
            {
                SaveAll(true);
                ClearLogs();
            }
            else
                return;

            Environment.Exit(9);
        }

        /// <summary>
        /// Очищаем папку с логами, чтоб не более 20 штук
        /// </summary>
        private void ClearLogs()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.PathSaveSetting) &&
                Directory.Exists(Properties.Settings.Default.PathSaveSetting + "\\Logs"))
            {
                var files = Directory.GetFiles(Properties.Settings.Default.PathSaveSetting + "\\Logs");

                foreach (var item in files)
                {
                    var file = new FileInfo(item);
                    if (file.CreationTime.AddDays(20) < DateTime.Now)
                        File.Delete(item);
                }
            }
        }

        public void SaveAll(bool isExit)
        {
            if (_error)
                return;
            try
            {
                SetWindow();
            }
            catch (Exception ex)
            {
                AllTerminal.Raise_OnSomething("Ошибка сохранения: " + ex.Message);
            }

            AllTerminal.SaveSetting();
        }

        #endregion

        #endregion

        private void LblAdapter_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckOpenDocument(Cfg.MainTerminalsSetting);
        }
    }
}
