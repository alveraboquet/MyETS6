using ClassControlsAndStyle.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SourceEts.UserConnector.UserConnectorControl
{
    /// <summary>
    /// Логика взаимодействия для TrwInstrAdd.xaml
    /// </summary>
    public partial class TrwInstrAdd : UserControl
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public TrwInstrAdd()
        {
            InitializeComponent();
        }

        public void SetBackground()
        {
            var brush = new SolidColorBrush(Color.FromArgb(255, (byte)65, (byte)81, (byte)97));
            Dtgd.Background = brush;
            TrvInstruments.Background = brush;
            TrvAvalibleInstruments.Background = brush;
        }

        /// <summary>
        /// Инструменты для торговли
        /// </summary>
        ObservableCollection<AllInstrForTrwModel> TreeViewAvalibleInstr = new ObservableCollection<AllInstrForTrwModel>();
        ObservableCollection<AllInstrForTrwModel> _allInstruments = new ObservableCollection<AllInstrForTrwModel>();
        private TerminalInfo _terminal;
        public void Load(TerminalInfo terminal)
        {
            InitializeComponent();
            //ClearIsSelected(terminal.AllInstruments);
            _allInstruments = terminal.AllSymbolsSave;
            _terminal = terminal;


            foreach (var item in terminal.AvalibleInstruments)
            {
                string name = item.Symbol;
                bool add = true;
                foreach (var itemAval in TreeViewAvalibleInstr)
                {
                    if (String.IsNullOrEmpty(item.ClassCodeVisible) && itemAval.ClassCode == item.ClassCode ||
                        !String.IsNullOrEmpty(item.ClassCodeVisible) && itemAval.ClassCodeVisible == item.ClassCodeVisible)
                    {
                        int insertIndex = 0;
                        for (int i = 0; i < itemAval.SeccodeListForForm.Count; i++)
                        {
                            var secCode = itemAval.SeccodeListForForm[i];
                            string nameSecCode = secCode.Symbol;
                            if (secCode != null)
                                if (nameSecCode.CompareTo(name) > 0)
                                    break;

                            insertIndex = i + 1;
                        }
                        itemAval.SeccodeListForForm.Insert(insertIndex,

                            item);

                        SetCheckOnInst(terminal.AllInstruments, item.ClassCode, item.ShortName, item.Symbol);
                        add = false;
                    }
                }
                if (add)
                {
                    AllInstrForTrwModel allInstr = new AllInstrForTrwModel();
                    allInstr.ClassCode = item.ClassCode;
                    allInstr.ClassCodeVisible = item.ClassCodeVisible;
                    allInstr.SeccodeListForForm = new ObservableCollection<AvalibleInstrumentsModel> { item };
                    TreeViewAvalibleInstr.Add(allInstr);

                    SetCheckOnInst(terminal.AllInstruments, item.ClassCode, item.ShortName, item.Symbol);
                }
            }


            TrvAvalibleInstruments.ItemsSource = TreeViewAvalibleInstr;
            TrvInstruments.ItemsSource = _allInstruments;
        }


        private void ClearIsSelected(List<AllInstrForTrwModel> col)
        {
            for (int i = 0; i < col.Count; i++)
            {
                for (int j = 0; j < col[i].SeccodeListSave.Count; j++)
                {
                    col[i].SeccodeListSave[j].IsSelected = false;
                }

            }
        }

        private void SetCheckOnInst(List<AllInstrForTrwModel> col, string classCode, string shortName, string symbol)
        {
            for (int i = 0; i < col.Count; i++)
            {
                if (col[i].ClassCode == classCode)
                {
                    for (int j = 0; j < col[i].SeccodeListSave.Count; j++)
                    {
                        var item = col[i].SeccodeListSave[j];
                        if (item.Symbol == symbol)
                        {
                            item.IsSelected = true;
                            break;
                        }
                    }
                }
            }
        }




        public void Save()
        {
            _terminal.AvalibleInstruments.Clear();
            foreach (var item in TreeViewAvalibleInstr)
            {
                foreach (var sec in item.SeccodeListForForm)
                {
                    _terminal.AvalibleInstruments.Add(sec);
                }
            }
        }


        private void TrvAvalibleInstruments_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as TreeView;

            if (item != null && item.SelectedItem is AllInstrForTrwModel)
            {
                if (new DialogOkCancel("Вы действительно хотите УДАЛИТЬ данный класс инструментов?", "Внимание!")
        .Result == MessageBoxResult.OK)
                {
                    var secs = (item.SelectedItem as AllInstrForTrwModel);
                    {
                        for (int j = secs.SeccodeListForForm.Count - 1; j >= 0; j--)
                        {
                            var sec = secs.SeccodeListForForm[j];
                            DelInstrFromAvalible(sec);

                            bool bryak = false;
                            foreach (var itemAll in _allInstruments)
                            {
                                if (itemAll.ClassCode == sec.ClassCode)
                                {
                                    for (int i = 0; i < itemAll.SeccodeListSave.Count; i++)
                                    {
                                        if (sec.Symbol == itemAll.SeccodeListSave[i].Symbol && sec.ClassCode == itemAll.SeccodeListSave[i].ClassCode)
                                        {
                                            itemAll.SeccodeListSave[i].IsSelected = false;
                                            bryak = true;
                                            break;
                                        }
                                    }
                                    if (bryak)
                                        break;
                                }

                            }
                        }
                    }
                }
            }

            if (item != null && item.SelectedItem is AvalibleInstrumentsModel)
            {

                var sec = (item.SelectedItem as AvalibleInstrumentsModel);
                {
                    DelInstrFromAvalible(sec);

                    bool bryak = false;
                    foreach (var itemAll in _allInstruments)
                    {
                        // if (itemAll.ClassCode == sec.ClassCode)
                        if (String.IsNullOrEmpty(itemAll.ClassCodeVisible) && sec.ClassCode == itemAll.ClassCode ||
                            !String.IsNullOrEmpty(itemAll.ClassCodeVisible) && sec.ClassCodeVisible == itemAll.ClassCodeVisible)
                        {
                            for (int i = 0; i < itemAll.SeccodeListSave.Count; i++)
                            {
                                if (sec.Symbol == itemAll.SeccodeListSave[i].Symbol)
                                {
                                    itemAll.SeccodeListSave[i].IsSelected = false;
                                    bryak = true;
                                    break;
                                }
                            }
                            if (bryak)
                                break;
                        }

                    }
                }
            }
        }

        private void TrvInstruments_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as TreeView;
            if (item != null && item.SelectedItem is AvalibleInstrumentsModel)
                AddNewInstrToAvalable((item.SelectedItem as AvalibleInstrumentsModel));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var chbx = sender as CheckBox;
            if (chbx != null && chbx.Tag != null && !String.IsNullOrEmpty(chbx.Tag.ToString()))
            {
                //var all = MainWindow.StaticMainWin.AllTerminal.TrCon.GetAllSecurity();
                var item = chbx.Tag as AvalibleInstrumentsModel;

                if (chbx.IsChecked == true)
                {
                    AddNewInstrToAvalable(item);
                }
                else
                {
                    DelInstrFromAvalible(item);
                }
            }
        }

        private void ButtonClassCode_OnClick(object sender, RoutedEventArgs e)
        {
            var chbx = sender as CheckBox;
            if (chbx != null && chbx.Tag != null)
            {
                var list = chbx.Tag as ObservableCollection<AvalibleInstrumentsModel>;
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (chbx.IsChecked == true)
                        {
                            AddNewInstrToAvalable(item);
                            item.IsSelected = true;
                        }
                        else
                        {
                            DelInstrFromAvalible(item);
                            item.IsSelected = false;
                        }
                    }
                }
            }
        }

        private void DelInstrFromAvalible(AvalibleInstrumentsModel avalIstr)
        {
            bool bryak = false;
            foreach (var itemAval in TreeViewAvalibleInstr)
            {
                //if (itemAval.ClassCode == avalIstr.ClassCode)
                if (String.IsNullOrEmpty(avalIstr.ClassCodeVisible) && itemAval.ClassCode == avalIstr.ClassCode ||
    !String.IsNullOrEmpty(avalIstr.ClassCodeVisible) && itemAval.ClassCodeVisible == avalIstr.ClassCodeVisible)
                {
                    for (int i = 0; i < itemAval.SeccodeListForForm.Count; i++)
                    {
                        if (avalIstr.Symbol == itemAval.SeccodeListForForm[i].Symbol)
                        {
                            itemAval.SeccodeListForForm.RemoveAt(i);
                            bryak = true;
                            break;
                        }
                    }
                    if (bryak)
                        break;
                }

            }
        }

        private void AddNewInstrToAvalable(AvalibleInstrumentsModel item)
        {
            if (item == null)
                return;

            //string name = item.ShortName;// + " (" + item.Symbol + ")";
            string name = item.Symbol;// + " (" + item.Symbol + ")";
            bool add = true;
            foreach (var itemAval in TreeViewAvalibleInstr)
            {
                if (itemAval.ClassCodeVisible == item.ClassCodeVisible)
                {
                    int insertIndex = 0;
                    bool exist = false;
                    for (int i = 0; i < itemAval.SeccodeListForForm.Count; i++)
                    {
                        var secCode = itemAval.SeccodeListForForm[i];
                        //string nameSecCode = secCode.ShortName;//+ " (" + secCode.Symbol + ")";
                        string nameSecCode = secCode.Symbol;//+ " (" + secCode.Symbol + ")";
                        if (String.IsNullOrEmpty(secCode.Symbol))
                            nameSecCode = secCode.Symbol;

                        if (secCode != null)
                        {
                            if (nameSecCode == item.Symbol)
                            {
                                exist = true;
                                break;
                            }

                            //if (String.Compare(nameSecCode, name, StringComparison.Ordinal) > 0)
                            if (nameSecCode.CompareTo(name) > 0)
                                break;
                        }


                        insertIndex = i + 1;
                    }

                    if (!exist)
                    {

                        var instr = new AvalibleInstrumentsModel
                        {
                            Symbol = item.Symbol,
                            ClassCode = item.ClassCode,
                            ShortName = item.ShortName,
                            ClassCodeVisible = item.ClassCodeVisible
                        };
                        if (item.Security != null)
                            instr.Security = item.Security;
                        itemAval.SeccodeListForForm.Insert(insertIndex, instr);


                    }
                    add = false;
                }
            }
            if (add)
            {
                AllInstrForTrwModel allInstr = new AllInstrForTrwModel();
                allInstr.ClassCode = item.ClassCode;
                allInstr.ClassCodeVisible = item.ClassCodeVisible;
                allInstr.SeccodeListForForm = new ObservableCollection<AvalibleInstrumentsModel>();
                var intst = new AvalibleInstrumentsModel
                {
                    Symbol = item.Symbol,
                    ClassCode = item.ClassCode,
                    ShortName = item.ShortName,
                    ClassCodeVisible = item.ClassCodeVisible,
                };
                if (item.Security != null)
                    intst.Security = item.Security;


                allInstr.SeccodeListForForm.Add(intst);
                TreeViewAvalibleInstr.Add(allInstr);

            }

        }

        //#region Источники данных

        //public void Load(TerminalInfo terminal, ObservableCollection<AvalibleInstrumentsModel> avalibleInstruments)
        //{
        //    InitializeComponent();
        //    ClearIsSelected(terminal.AllInstruments);
        //    _allInstruments = terminal.AllInstruments;
        //    _terminal = terminal;

        //    foreach (var item in avalibleInstruments)
        //    {
        //        string name = item.Symbol;
        //        bool add = true;
        //        foreach (var itemAval in TreeViewAvalibleInstr)
        //        {
        //            if (String.IsNullOrEmpty(item.ClassCodeVisible) && itemAval.ClassCode == item.ClassCode ||
        //                !String.IsNullOrEmpty(item.ClassCodeVisible) && itemAval.ClassCodeVisible == item.ClassCodeVisible)
        //            {
        //                int insertIndex = 0;
        //                for (int i = 0; i < itemAval.SeccodeListForForm.Count; i++)
        //                {
        //                    var secCode = itemAval.SeccodeListForForm[i];
        //                    string nameSecCode = secCode.Symbol;
        //                    if (secCode != null)
        //                        if (nameSecCode.CompareTo(name) > 0)
        //                            break;

        //                    insertIndex = i + 1;
        //                }
        //                itemAval.SeccodeListForForm.Insert(insertIndex,

        //                    item);

        //                SetCheckOnInst(terminal.AllInstruments, item.ClassCode, item.ShortName, item.Symbol);
        //                add = false;
        //            }
        //        }
        //        if (add)
        //        {
        //            AllInstrForTrwModel allInstr = new AllInstrForTrwModel();
        //            allInstr.ClassCode = item.ClassCode;
        //            allInstr.ClassCodeVisible = item.ClassCodeVisible;
        //            allInstr.SeccodeListForForm = new ObservableCollection<AvalibleInstrumentsModel> { item };
        //            TreeViewAvalibleInstr.Add(allInstr);

        //            SetCheckOnInst(terminal.AllInstruments, item.ClassCode, item.ShortName, item.Symbol);
        //        }
        //    }


        //    TrvAvalibleInstruments.ItemsSource = TreeViewAvalibleInstr;
        //    TrvInstruments.ItemsSource = _allInstruments;
        //}

        ///// <summary>
        ///// Для источников данных
        ///// </summary>
        ///// <param name="avalibleInstruments"></param>
        //public void Save(ObservableCollection<AvalibleInstrumentsModel> avalibleInstruments)
        //{
        //    avalibleInstruments.Clear();
        //    foreach (var item in TreeViewAvalibleInstr)
        //    {
        //        foreach (var sec in item.SeccodeListForForm)
        //        {
        //            int indexInsert = 0;
        //            foreach (var symbol in avalibleInstruments)
        //            {
        //                if (sec.Symbol.CompareTo(symbol.Symbol) < 0)
        //                {
        //                    break;
        //                }
        //                else
        //                {
        //                    indexInsert += 1;
        //                }
        //            }
        //            avalibleInstruments.Insert(indexInsert, sec);
        //        }
        //    }
        //}
        //#endregion

        private void Button_AddRow_Click(object sender, RoutedEventArgs e)
        {
            AvalibleInstrumentsModel model = new AvalibleInstrumentsModel();
            AddNewForeignInstr add = new AddNewForeignInstr(model);
            add.ShowDialog();

            if (add.DialogResult == true)
            {
                AddInstrToAll(model, _allInstruments);
            }

        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {


        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {


        }


        /// <summary>
        /// Данные по всем инструментам рынка
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="allInstrForTrwModelList"></param>
        private void AddInstrToAll(AvalibleInstrumentsModel sec, ObservableCollection<AllInstrForTrwModel> allInstrForTrwModelList)
        {
            bool add = true;
            string name = sec.Symbol;
            string classCodeVis = sec.ClassCode;


            foreach (var classCode in allInstrForTrwModelList)
            {
                if (classCode.ClassCodeVisible == classCodeVis)
                {
                    int insertIndex = 0;
                    for (int i = 0; i < classCode.SeccodeListSave.Count; i++)
                    {
                        var secCode = classCode.SeccodeListSave[i];
                        string nameSecCode = secCode.Symbol;
                        if (secCode != null)
                        {

                            if (nameSecCode.CompareTo(name) > 0)
                                break;
                        }
                        insertIndex = i + 1;
                    }
                    classCode.SeccodeListSave.Insert(insertIndex, sec);

                    add = false;
                }
            }
            if (add)
            {



                allInstrForTrwModelList.Insert(0, new AllInstrForTrwModel
                {
                    ClassCode = sec.ClassCode,
                    ClassCodeVisible = classCodeVis,
                    SeccodeListSave = new ObservableCollection<AvalibleInstrumentsModel> { sec }
                });
            }
        }
    }
}
