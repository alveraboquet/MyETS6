using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using SourceEts.Terminals;
using SourceEts.UserConnector;
using SourceEts.UserConnector.UserConnectorControl;
using Adapter;
using Adapter.Config;

namespace TerminalRbkm.View
{
    /// <summary>
    /// Логика взаимодействия для AddNewTerminal.xaml
    /// </summary>
    public partial class AddNewTerminal
    {
        private ITerminalInfo _terminalInfo;
        private AbstractTerminal _absTerminal;
        public AddNewTerminal(MainWindow main, ITerminalInfo terminalInfo, AbstractTerminal absTerminal)
        {
            InitializeComponent();

            _terminalInfo = terminalInfo;
            _absTerminal = absTerminal;
            TxbxName.Text = _terminalInfo.Name;

            if (absTerminal != null && absTerminal.UserConSetting.Count > 0 && !absTerminal.IsCommonTerminl)
            {
                if (_terminalInfo is TerminalInfo)
                {
                    foreach (var itemUserCon in absTerminal.UserConSetting)
                    {
                        SetUserControl(absTerminal, itemUserCon);
                    }
                    LoadUserCon(((TerminalInfo)_terminalInfo).UserConSettings);
                }

                if (absTerminal.CommonConSetting.Count > 0)
                {
                    var chbx = new TextBlock
                    {
                        Text = "Общие настройки",
                        Margin = new Thickness(15, 5, 0, 5),
                        Foreground = System.Windows.Media.Brushes.White
                    };
                    StcControls.Children.Add(chbx);
                }
                foreach (var itemUserCon in absTerminal.CommonConSetting)
                {
                    SetUserControl(absTerminal, itemUserCon);
                }
                LoadUserCon(absTerminal.CommonConSetting);

            }
        }

        private void SetUserControl(AbstractTerminal absTerminal, SettingUserCon itemUserCon)
        {
            if (itemUserCon.IsBool)
            {
                var chbx = new ChbxUserCon
                {
                    ChbxChecked = itemUserCon.ValueBool,
                    TxblText = itemUserCon.NameParam,
                    TxblToolTipText = itemUserCon.Description,
                    IsEnabled = !absTerminal.IsPushBtnConnect || itemUserCon.IsChangeAfterConnect
                };

                StcControls.Children.Add(chbx);
            }
            if (itemUserCon.IsStirng)
            {
                var chbx = new TxbxUserCon
                {
                    TxbxText = itemUserCon.ValueString,
                    TxblText = itemUserCon.NameParam,
                    TxblToolTipText = itemUserCon.Description,
                    IsEnabled = !absTerminal.IsPushBtnConnect || itemUserCon.IsChangeAfterConnect
                };
                StcControls.Children.Add(chbx);
            }

            if (itemUserCon.IsListString)
            {
                var chbx = new CmbxUserCon
                {
                    CmbxSource = itemUserCon.ValueListString,
                    SelectedInex = 0,
                    TxblText = itemUserCon.NameParam,
                    TxblToolTipText = itemUserCon.Description,
                    IsEnabled = !absTerminal.IsPushBtnConnect || itemUserCon.IsChangeAfterConnect
                };
                StcControls.Children.Add(chbx);
            }

            if (itemUserCon.IsDigit)
            {
                var chbx = new TxbxDitgitUserCon
                {
                    TxbxText = itemUserCon.ValueDigit.ToString(),
                    TxblText = itemUserCon.NameParam,
                    TxblToolTipText = itemUserCon.Description,
                    IsEnabled = !absTerminal.IsPushBtnConnect || itemUserCon.IsChangeAfterConnect
                };
                StcControls.Children.Add(chbx);
            }

            if (itemUserCon.IsUseInstrumentSort)
            {
                var trw = new TrwUserCon
                {
                    Name = itemUserCon.NameParam,
                    Description = itemUserCon.Description,
                };
                trw.Load(_terminalInfo as TerminalInfo);
                StcControls.Children.Add(trw);
            }

            if (itemUserCon.IsUseInstrumentAdd)
            {
                var trw = new TrwInstrAdd
                {
                    Name = itemUserCon.NameParam,
                    Description = itemUserCon.Description,
                };
                trw.Load(_terminalInfo as TerminalInfo);
                StcControls.Children.Add(trw);
            }
        }

        private void LoadUserCon(List<SettingUserCon> userSettings)
        {
            //if (_terminalInfo is TerminalInfo)
            {
                //var term = ;

                foreach (var userCon in userSettings)
                {
                    foreach (var item in StcControls.Children)
                    {
                        if (item is ChbxUserCon && userCon.IsBool && (item as ChbxUserCon).TxblText == userCon.NameParam)
                        {
                            (item as ChbxUserCon).ChbxChecked = userCon.ValueBool;
                        }
                        if (item is TxbxUserCon && userCon.IsStirng && (item as TxbxUserCon).TxblText == userCon.NameParam)
                        {
                            (item as TxbxUserCon).TxbxText = userCon.ValueString;
                        }
                        if (item is CmbxUserCon && userCon.IsListString && (item as CmbxUserCon).TxblText == userCon.NameParam)
                        {
                            (item as CmbxUserCon).SelectedItem = userCon.ValueString;
                        }

                        if (item is TxbxDitgitUserCon && userCon.IsDigit && (item as TxbxDitgitUserCon).TxblText == userCon.NameParam)
                        {
                            (item as TxbxDitgitUserCon).TxbxText = userCon.ValueDigit.ToString();
                        }
                    }
                }


            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(_terminalInfo.Terminal))
                _terminalInfo.IsUse = true;

            _terminalInfo.Name = TxbxName.Text;

            if (!(_terminalInfo is TerminalInfo))
            { }   

            if (_absTerminal != null && _terminalInfo is TerminalInfo)
            {
                var term = _terminalInfo as TerminalInfo;
                term.Terminal = _absTerminal.NameUserAdapter;

                SaveUserSettings(term.UserConSettings);
            }

            _absTerminal.IsUpdateInstrument = true;
            DialogResult = true;
            Close();
        }

        private void SaveUserSettings(List<SettingUserCon> userSettings)
        {
            #region Пользовательские настройки
            if (userSettings.Count > 0)
                for (int i = 0; i < userSettings.Count; i++)
                {
                    var itemUserCon = userSettings[i];
                    //term.UserConSettings.Clear();
                    foreach (var item in StcControls.Children)
                    {

                        if (item is ChbxUserCon)
                        {
                            var chbx = (item as ChbxUserCon);
                            if (itemUserCon.NameParam == chbx.TxblText)
                            {
                                itemUserCon.ValueBool = chbx.ChbxChecked;
                            }
                        }

                        if (item is TxbxUserCon)
                        {
                            var txbx = (item as TxbxUserCon);
                            if (itemUserCon.NameParam == txbx.TxblText)
                            {
                                itemUserCon.ValueString = txbx.TxbxText;
                            }
                        }

                        if (item is CmbxUserCon)
                        {
                            var cmbx = (item as CmbxUserCon);
                            if (itemUserCon.NameParam == cmbx.TxblText)
                            {
                                itemUserCon.ValueString = cmbx.SelectedItem.ToString();
                            }
                        }


                        if (item is TxbxDitgitUserCon)
                        {
                            var txbx = (item as TxbxDitgitUserCon);
                            if (itemUserCon.NameParam == txbx.TxblText)
                            {
                                itemUserCon.ValueDigit = ConfigTermins.ConvertToDoubleMy((item as TxbxDitgitUserCon).TxbxText);
                                itemUserCon.IsStirng = false;
                                itemUserCon.IsListString = false;
                                itemUserCon.IsBool = false;
                                itemUserCon.IsDigit = true;
                            }
                        }


                    }

                }
            #endregion

            #region Общие для данного терминала
            if (_absTerminal.CommonConSetting.Count > 0)
            {
                foreach (var item in StcControls.Children)
                {
                    if (item is ChbxUserCon)
                    {
                        var set = new SettingUserCon();
                        set.IsBool = true;
                        set.ValueBool = (item as ChbxUserCon).ChbxChecked;
                        set.NameParam = (item as ChbxUserCon).TxblText;
                        set.Description = (item as ChbxUserCon).TxblToolTipText;
                        foreach (var itemCom in _absTerminal.CommonConSetting)
                        {
                            if (itemCom.NameParam == set.NameParam)
                            {
                                itemCom.IsBool = set.IsBool;
                                itemCom.ValueBool = set.ValueBool;
                            }
                        }
                    }
                    if (item is TxbxUserCon)
                    {
                        var set = new SettingUserCon();
                        set.IsStirng = true;
                        set.ValueString = (item as TxbxUserCon).TxbxText;
                        set.NameParam = (item as TxbxUserCon).TxblText;
                        set.Description = (item as TxbxUserCon).TxblToolTipText;
                        foreach (var itemCom in _absTerminal.CommonConSetting)
                        {
                            if (itemCom.NameParam == set.NameParam)
                            {
                                itemCom.IsStirng = set.IsStirng;
                                itemCom.ValueString = set.ValueString;
                            }
                        }
                    }

                    if (item is CmbxUserCon)
                    {
                        var set = new SettingUserCon();
                        set.IsListString = true;
                        set.ValueString = (item as CmbxUserCon).SelectedItem.ToString();
                        set.NameParam = (item as CmbxUserCon).TxblText;
                        set.Description = (item as CmbxUserCon).TxblToolTipText;
                        foreach (var itemCom in _absTerminal.CommonConSetting)
                        {
                            if (itemCom.NameParam == set.NameParam)
                            {
                                itemCom.IsListString = set.IsListString;
                                itemCom.ValueString = set.ValueString;
                            }
                        }
                    }


                    if (item is TxbxDitgitUserCon)
                    {
                        var set = new SettingUserCon();
                        set.IsDigit = true;
                        set.ValueDigit = ConfigTermins.ConvertToDoubleMy((item as TxbxDitgitUserCon).TxbxText);
                        set.NameParam = (item as TxbxDitgitUserCon).TxblText;
                        foreach (var itemCom in _absTerminal.CommonConSetting)
                        {
                            if (itemCom.NameParam == set.NameParam)
                            {
                                itemCom.ValueDigit = set.ValueDigit;
                            }
                        }
                    }

                    if (item is TrwUserCon)
                    {
                        var set = new SettingUserCon();
                        set.NameParam = (item as TrwUserCon).Name;
                        set.Description = (item as TrwUserCon).Description;
                        set.IsUseInstrumentSort = true;
                        (item as TrwUserCon).Save();

                    }

                    if (item is TrwInstrAdd)
                    {
                        (item as TrwInstrAdd).Save();
                    }

                }
            }
            #endregion

        }

        private void BtnPath_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
