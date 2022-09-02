using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Adapter;
using Adapter.Config;
using Adapter.Interfaces;
using Adapter.Logic;
//using Adapter.Quik.Model.Tables;
//using Adapter.TransaqConnector.Model;
//using DevExpress.Mvvm.Native;
//using DevExpress.Mvvm.POCO;
//using DevExpress.Xpf.Core;
//using ModulSolution.Model;
//using ModulSolution.View.Service;
using ScriptSolution;
using Adapter.Model;
using ScriptSolution.Model.OrderAndStopOrderModels;
using SourceEts.Table;
using System.Collections.ObjectModel;
using CommonDataContract.AbstractDataTypes;
using SourceEts;
using CommonDataContract;

namespace ModulSolution.View
{
    /// <summary>
    /// Логика взаимодействия для Stakan.xaml
    /// </summary>
    public partial class Stakan
    {
        private AbstractTerminal _term;
        private AllTerminals _allTerminal ;
        //private DataRobots _dataRobots;
        //private GlassSettingModel _glassSettingModel = new GlassSettingModel();

        //private string acc = "";
        private int _qty = 0;
        private DispatcherTimer time = new DispatcherTimer();
        private ObservableCollection<ISecurity> _securitiesTable;
        private static readonly DataStorage dataStorage = DataStorage.Instance;

        public Stakan(AllTerminals allTerminals /*DataRobots dataRobots*/)
        {
            InitializeComponent();

            _allTerminal = allTerminals;
            //_dataRobots = dataRobots;

            CmbxClientCode.ItemsSource = _allTerminal.GetPairClienCodesAccount();
            CmbxClientCode.SelectedIndex = 0;


            _securitiesTable = dataStorage.CurrentParamModelList;
            DataContext = _securitiesTable;

            //CmbxSymbol.ItemsSource = _allTerminal.GetSeccodeList();


            //_term.BaseMon.Raise_OnSomething("Блок 3. Инициализация стакана. Инструмент по умолчанию = " + CmbxSymbol.SelectedItem);


            time.Interval = new TimeSpan(0, 0, 0, 1);
            time.Tick += time_Tick;
            time.Start();

        }

        void time_Tick(object sender, EventArgs e)
        {
            if (_allTerminal == null)
                return;

            if (DtgdStakan.Items.Count == 0)
            {
                if (CmbxSymbol != null && CmbxSymbol.SelectedItem != null)
                {
                    var dd = dataStorage.Glasses;
                    if (dd != null)
                        for (int i = 0; i < dd.Count; i++)
                        {
                            if (dd[i].Symbol == CmbxSymbol.SelectedItem.ToString())
                            {
                                dd[i].IsFromObsCol = true;
                                DtgdStakan.ItemsSource = dd[i].QuotationsFull;
                            }
                        }
                }

                if (CmbxSymbol != null && CmbxSymbol.SelectedItem != null && CmbxClientCode.SelectedItem != null)
                {
                    SetPosition(CmbxSymbol.SelectedItem as ISecurity, (CmbxClientCode.SelectedItem as AccountsPair));
                  

                }
            }
            if (CmbxSymbol != null && CmbxSymbol.SelectedItem != null && CmbxClientCode.SelectedItem != null)
            {
                if (String.IsNullOrEmpty(SymbolPos))
                {
                    TxbxPos.Text = "0";
                    SetPosition((CmbxSymbol.SelectedItem as ISecurity), (CmbxClientCode.SelectedItem as AccountsPair));
                }
            }

        }

        private void CmbxSymbol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                var item = (sender as ComboBox);
                if (item != null && item.SelectedItem != null)
                {
                    // _term.BaseMon.Raise_OnSomething("Блок 5. Смена инструмента  на новый = " + item.SelectedItem);

                    var dd = dataStorage.Glasses;
                    if (dd != null)
                        for (int i = 0; i < dd.Count; i++)
                        {
                            var b = (item.SelectedItem as ISecurity);

                            if (dd[i].Symbol == b.Seccode)
                            {
                                dd[i].IsFromObsCol = true;
                                DtgdStakan.ItemsSource = dd[i].QuotationsFull;
                            }
                        }

                }

                //_term.BaseMon.Raise_OnSomething("Блок 4. Смена инструмента = " + CmbxSymbol.SelectedItem);
                if (_term == null)
                    return;



                if (item != null && item.SelectedItem != null && CmbxClientCode.SelectedItem != null)
                {
                    // _term.BaseMon.Raise_OnSomething("Блок 6. Запрос позиции и смена инструмента  на новый = " + item.SelectedItem);

                    SetPosition(item.SelectedItem as ISecurity, (CmbxClientCode.SelectedItem as AccountsPair));
      
                }
            }
            catch (Exception)
            {

            }
        }

        private void CmbxClientCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ComboBox);
            //_term.BaseMon.Raise_OnSomething("Блок 1. Смена клиентского счета");

            if (item != null && item.SelectedItem != null && item.SelectedItem is AccountsPair)
            {
                if (CmbxSymbol.SelectedItem != null)
                {
                    SetPosition(CmbxSymbol.SelectedItem as ISecurity, (item.SelectedItem as AccountsPair) );
                    //_term = _allTerminal.GetTerminal(null, (item.SelectedItem as AccountsPair).Account, (item.SelectedItem as AccountsPair).ClientCode);
                    //_term.BaseMon.Raise_OnSomething("Блок 2. Смена клиентского счета прошла удачно");
                }

            }

            //if (item != null && item.SelectedItem != null)
            //{
            //    acc = _allTerminal.GetAccount(item.SelectedItem.ToString());

            //    if (CmbxSymbol.SelectedItem != null)
            //    {
            //        SetPosition(CmbxSymbol.SelectedItem.ToString(), item.SelectedItem.ToString());
            //        _term = _allTerminal.GetTerminal(null, item.SelectedItem.ToString(), acc);
            //        //_term.BaseMon.Raise_OnSomething("Блок 2. Смена клиентского счета прошла удачно");
            //    }

            //}

        }

        /// <summary>
        /// Символ по которому был получен ответ по позиции
        /// </summary>
        public string SymbolPos { get; set; }
        private void SetPosition(ISecurity sec, AccountsPair client)
        {
            // _term.BaseMon.Raise_OnSomething("Блок 7. Запрос на позицию инстумента = " + symbol + " по счету " + client);


            if (_term != null && sec!=null && client!=null )
            {
                SymbolPos = "";
                // _term.BaseMon.Raise_OnSomething("Блок 7.1. ");

                //var pos = _term.GetPositionSharesItem(client, symbol);
                var pos = _allTerminal.GetPositionSharesItem(client.ClientCode, client.Account, sec.Seccode);

                if (pos!=null && !String.IsNullOrEmpty(pos.Symbol))
                {
                    // _term.BaseMon.Raise_OnSomething("Блок 8. Информация по акциям получена. Инструмент = " + symbol + " по счету " + client);

                    SymbolPos = pos.Symbol;
                    var posShare = _allTerminal.GetPositionSharesItem(client.ClientCode, client.Account, sec.Seccode);
                    Binding binding = new Binding();
                    // задаем объект-источник
                    binding.Source = posShare;
                    // задаем свойство-источник
                    binding.Path = new PropertyPath("Balance");
                    binding.Mode = BindingMode.TwoWay;
                    // добавляем к нему свойство приемник
                    TxbxPos.SetBinding(TextBox.TextProperty, binding);
                }
                //var posFut = _term.GetPositionFuturesItem(client, symbol);
                var posFut = _allTerminal.GetPositionFuturesItem(client.ClientCode, sec.Seccode);
                if (posFut!=null && !String.IsNullOrEmpty(posFut.Symbol))
                {
                    // _term.BaseMon.Raise_OnSomething("Блок 9. Информация по фьючерсам получена. Инструмент = " + symbol + " по счету " + client);

                    SymbolPos = posFut.Symbol;
                    Binding binding = new Binding();
                    // задаем объект-источник
                    binding.Source = posFut;
                    // задаем свойство-источник
                    binding.Path = new PropertyPath("Balance");
                    binding.Mode = BindingMode.TwoWay;
                    // добавляем к нему свойство приемник
                    TxbxPos.SetBinding(TextBox.TextProperty, binding);
                }
            }
        }

        private void BtnTopMost_Click(object sender, RoutedEventArgs e)
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri(
               "StyleAndControls;component/Dictionary1.xaml", UriKind.Relative);

            if (Topmost == false)
            {
                Topmost = true;
                BtnTopMost.Style = (Style)resourceDictionary["TopMost_ButtonStyle"];
                //cfg.SetLog(this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + ": Свойство окна \"Поверх всех\" - поставлено", MainWindow.StaticMainWin.ListBoxLogs, GetType(), MethodBase.GetCurrentMethod());
            }
            else
            {
                Topmost = false;
                BtnTopMost.Style = (Style)resourceDictionary["TopMostOut_ButtonStyle"];
                //cfg.SetLog(this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + ": Свойство окна \"Поверх всех\" - снято", ListBoxLogs, GetType(), MethodBase.GetCurrentMethod());
            }
        }






        private double GetMarketPrice(ISecurity sec, bool isMarket, string oper)
        {
            double result = 0;
            if (isMarket)
            {
                if (sec.ClassCode.ToUpper().Contains(CfgSourceEts.ClasscodeFut))
                    result = oper == ConfigTermins.Buy ? sec.MaxPrice : sec.MinPrice;
                if (!sec.ClassCode.ToUpper().Contains(CfgSourceEts.ClasscodeFut) && sec.ClassCode != CfgSourceEts.ClassCodeSpbopt)
                    result = oper == ConfigTermins.Buy ? sec.LastPrice + sec.MinStep * 20 : sec.LastPrice - sec.MinStep * 20;
            }

            return result;
        }

        private void TxbxQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as TextBox);
            if (text != null && !String.IsNullOrEmpty(text.Text))
                _qty = Convert.ToInt32(text.Text);
        }


        #region Кнопки
        private void BtnSellMarket_Click(object sender, RoutedEventArgs e)
        {
            if (_term != null && _term.IsConnect)
                if (CmbxSymbol.SelectedItem != null && CmbxClientCode.SelectedItem != null)
                {
                    if (!String.IsNullOrEmpty(CmbxSymbol.SelectedItem.ToString()) &&
                        !String.IsNullOrEmpty(CmbxClientCode.SelectedItem.ToString()))
                    {
                        //_term.BaseMon.Raise_OnSomething("Блок 10. Отправка рыночной заявки на продажу по инструменту = " + CmbxSymbol.SelectedItem + " по счету " + CmbxClientCode.SelectedItem);


                        var param = GetParamOfTranscationModelOrder((CmbxClientCode.SelectedItem as AccountsPair),
                            CmbxSymbol.SelectedItem as ISecurity , ConfigTermins.Sell,
                            CfgSourceEts.TypeRequestForTransactionOrder,
                            true, 0, _qty);
                        //_term.TransactionOrder(param, null);


                    }
                    //  
                }
        }

        private void BtnBuyMarket_Click(object sender, RoutedEventArgs e)
        {
            if (_term != null && _term.IsConnect)
                if (CmbxSymbol.SelectedItem != null && CmbxClientCode.SelectedItem != null)
                {
                    if (!String.IsNullOrEmpty(CmbxSymbol.SelectedItem.ToString()) &&
                        !String.IsNullOrEmpty(CmbxClientCode.SelectedItem.ToString()))
                    {
                        //_term.BaseMon.Raise_OnSomething("Блок 11. Отправка рыночной заявки на покупку по инструменту = " + CmbxSymbol.SelectedItem + " по счету " + CmbxClientCode.SelectedItem);


                        var param = GetParamOfTranscationModelOrder((CmbxClientCode.SelectedItem as AccountsPair),
                            CmbxSymbol.SelectedItem as ISecurity, ConfigTermins.Buy,
                            CfgSourceEts.TypeRequestForTransactionOrder,
                            true, 0, _qty);
                        //_term.TransactionOrder(param, null);

                    }
                    //  
                }
        }

        private void DtgdStakan_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = sender as DataGrid;

            if (item != null)
            {
                var qoute = item.CurrentCell.Item as IGlassQuotation;
                if (qoute != null)
                {
                    if (_term != null && _term.IsConnect)
                        if (CmbxSymbol.SelectedItem != null && CmbxClientCode.SelectedItem != null)
                        {
                            if (!String.IsNullOrEmpty(CmbxSymbol.SelectedItem.ToString()) &&
                                !String.IsNullOrEmpty(CmbxClientCode.SelectedItem.ToString()))
                            {
                                var finInfo = CmbxSymbol.SelectedItem as ISecurity;

                                if (item.CurrentCell.Column.Header.ToString() == "Sell" && qoute.SellQty > 0)
                                {
                                    //  _term.BaseMon.Raise_OnSomething("Блок 12. Отправка заявки на продажу из стакана по инструменту = " + CmbxSymbol.SelectedItem + " по счету " + CmbxClientCode.SelectedItem);

                                    var param = GetParamOfTranscationModelOrder((CmbxClientCode.SelectedItem as AccountsPair),
                                        CmbxSymbol.SelectedItem as ISecurity, ConfigTermins.Sell,
                                        CfgSourceEts.TypeRequestForTransactionOrder,
                                        false, qoute.Price, _qty);
                                    //_term.TransactionOrder(param, null);

                                }


                                if (item.CurrentCell.Column.Header.ToString() == "Buy" && qoute.SellQty > 0)
                                {
                                    // _term.BaseMon.Raise_OnSomething("Блок 13. Отправка стоп-заявки на покупку из стакана по инструменту = " + CmbxSymbol.SelectedItem + " по счету " + CmbxClientCode.SelectedItem);
                                    double otstup = 0;
                                    //double otstup = _glassSettingModel.SlOtstup <= 0
                                    //    ? finInfo.MinStep * 20
                                    //    : _glassSettingModel.SlOtstup * finInfo.MinStep;
                                    var param = GetParamOfTranscationModelStopOrder((CmbxClientCode.SelectedItem as AccountsPair),
                                                CmbxSymbol.SelectedItem as ISecurity, ConfigTermins.Buy,
                                                qoute.Price, qoute.Price + otstup, _qty);
                                    //_term.TransactionOrder(param, null);
                                }

                                if (item.CurrentCell.Column.Header.ToString() == "Sell" && qoute.BuyQty > 0)
                                {
                                    // _term.BaseMon.Raise_OnSomething("Блок 14. Отправка стоп-заявки на продажу из стакана по инструменту = " + CmbxSymbol.SelectedItem + " по счету " + CmbxClientCode.SelectedItem);
                                    double otstup = 0;
                                    //double otstup = _glassSettingModel.SlOtstup <= 0
                                    //    ? finInfo.MinStep * 20
                                    //    : _glassSettingModel.SlOtstup * finInfo.MinStep;
                                    var param = GetParamOfTranscationModelStopOrder((CmbxClientCode.SelectedItem as AccountsPair),
                                                CmbxSymbol.SelectedItem as ISecurity, ConfigTermins.Sell,
                                                qoute.Price, qoute.Price - otstup, _qty);
                                    //_term.TransactionOrder(param, null);
                                }

                                if (item.CurrentCell.Column.Header.ToString() == "Buy" && qoute.BuyQty > 0)
                                {
                                    // _term.BaseMon.Raise_OnSomething("Блок 14. Отправка заявки на покупку из стакана по инструменту = " + CmbxSymbol.SelectedItem + " по счету " + CmbxClientCode.SelectedItem);

                                    var param = GetParamOfTranscationModelOrder(CmbxClientCode.SelectedItem as AccountsPair,
                                        CmbxSymbol.SelectedItem as ISecurity, ConfigTermins.Buy,
                                        CfgSourceEts.TypeRequestForTransactionOrder,
                                        false, qoute.Price, _qty);
                                    //_term.TransactionOrder(param, null);
                                }
                            }
                        }
                }
            }
        }

        #region Переворот и закрытие позиции
        /// <summary>
        /// Переворот позиции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReversMarket_Click(object sender, RoutedEventArgs e)
        {
            //_term.BaseMon.Raise_OnSomething("Блок 15. Переворот позиции");

            if (_term != null && _term.IsConnect)
                if (CmbxSymbol.SelectedItem != null && CmbxClientCode.SelectedItem != null)
                {
                    if (!String.IsNullOrEmpty(CmbxSymbol.SelectedItem.ToString()) &&
                        !String.IsNullOrEmpty(CmbxClientCode.SelectedItem.ToString()) &&
                        !String.IsNullOrEmpty(TxbxPos.Text) && Convert.ToInt32(TxbxPos.Text) != 0)
                    {
                        // _term.BaseMon.Raise_OnSomething("Блок 16. Переворот позиции по инструменту = " + CmbxSymbol.SelectedItem + " по счету " + CmbxClientCode.SelectedItem);

                        var param = GetParamOfTranscationModelOrder(CmbxClientCode.SelectedItem as AccountsPair,
                            CmbxSymbol.SelectedItem as ISecurity, Convert.ToInt32(TxbxPos.Text) > 0 ? ConfigTermins.Sell : ConfigTermins.Buy,
                            CfgSourceEts.TypeRequestForTransactionOrder,
                            true, 0, Math.Abs(Convert.ToInt32(TxbxPos.Text)) + _qty);
                        //_term.TransactionOrder(param, null);


                    }
                    //  
                }
        }


        /// <summary>
        /// Закрытие позиции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClosePos_Click(object sender, RoutedEventArgs e)
        {
            if (_term != null && _term.IsConnect)
                if (CmbxSymbol.SelectedItem is ISecurity && CmbxClientCode.SelectedItem is AccountsPair)
                {
                    // _term.BaseMon.Raise_OnSomething("Блок 17. Закрытие позиции");
                    var accPair = CmbxClientCode.SelectedItem as AccountsPair;

                    if (!String.IsNullOrEmpty(CmbxSymbol.SelectedItem.ToString()) &&
                        !String.IsNullOrEmpty(CmbxClientCode.SelectedItem.ToString()))
                    {
                        // _term.BaseMon.Raise_OnSomething("Блок 18. Закрытие позиции по инструменту = " + CmbxSymbol.SelectedItem + " по счету " + CmbxClientCode.SelectedItem);

                        var sec = CmbxSymbol.SelectedItem as ISecurity;

                        var param = GetParamOfTranscationModelOrder(CmbxClientCode.SelectedItem as AccountsPair,
                            sec, Convert.ToInt32(TxbxPos.Text) > 0 ? ConfigTermins.Sell : ConfigTermins.Buy,
                            CfgSourceEts.TypeRequestForTransactionOrder,
                            true, 0, Math.Abs(Convert.ToInt32(TxbxPos.Text)));
                        if (!String.IsNullOrEmpty(TxbxPos.Text) && Convert.ToInt32(TxbxPos.Text) != 0)
                            //_term.TransactionOrder(param, null);

                        // var orders = _term.GetOrders(CmbxClientCode.SelectedItem.ToString(), CmbxSymbol.SelectedItem.ToString());

                        for (int i = 0; i < dataStorage.OrdersList.Count; i++)
                        {
                            var item = dataStorage.OrdersList[i];
                            if (item.Account == accPair.Account && item.ClientCode == accPair.ClientCode &&
                                item.Status == ConfigTermins.Active && item.Symbol == sec.Seccode)
                            {
                                param = GetParamOfTranscationModelForKillOrder(accPair, item);
                                //_term.TransactionOrder(param, null);
                            }
                        }

                        //var stops = _term.GetStops(CmbxClientCode.SelectedItem.ToString(), CmbxClientCode.SelectedItem.ToString());
                        for (int i = dataStorage.StopOrderList.Count - 1; i >= 0; i--)
                        {
                            var item = dataStorage.StopOrderList[i];
                            if (item.Account == accPair.Account && item.ClientCode == accPair.ClientCode &&
                                item.Status == ConfigTermins.Active && item.Symbol == sec.Seccode)
                            {
                                param = GetParamOfTranscationModelForKillStop(accPair, item);
                               // _term.TransactionOrder(param, null);
                            }
                        }



                    }
                    //  
                }
        }
        #endregion

        #region Снятие заявок
        /// <summary>
        /// Снятие всех заявок на покупку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBuyOrderKill_Click(object sender, RoutedEventArgs e)
        {
            if (_term != null && _term.IsConnect)
                if (CmbxSymbol.SelectedItem is ISecurity && CmbxClientCode.SelectedItem is AccountsPair)
                {
                    if (!String.IsNullOrEmpty(CmbxSymbol.SelectedItem.ToString()) &&
                        !String.IsNullOrEmpty(CmbxClientCode.SelectedItem.ToString()))
                    {
                        //var orders = _term.GetOrders(CmbxClientCode.SelectedItem.ToString(), CmbxSymbol.SelectedItem.ToString());
                        ////_term.BaseMon.Raise_OnSomething("Блок 19. Снятие заявок на покупку по инструменту = " + CmbxSymbol.SelectedItem + " по счету " + CmbxClientCode.SelectedItem);

                        //for (int i = 0; i < orders.Count; i++)
                        //{
                        //    var item = orders[i];
                        //    if (item.Status == ConfigTermins.Active && item.Operation == ConfigTermins.Buy)
                        //    {
                        //        var param = GetParamOfTranscationModelForKillOrder(CmbxClientCode.SelectedItem as AccountsPair, item);
                        //        _term.TransactionOrder(param, null);

                        //    }
                        //}
                        var sec = CmbxSymbol.SelectedItem as ISecurity;
                        var accPair = CmbxClientCode.SelectedItem as AccountsPair;

                        for (int i = 0; i < dataStorage.OrdersList.Count; i++)
                        {
                            var item = dataStorage.OrdersList[i];
                            if (item.Account == accPair.Account && item.ClientCode == accPair.ClientCode && item.Operation == ConfigTermins.Buy &&
                                item.Status == ConfigTermins.Active && item.Symbol == sec.Seccode)
                            {
                                var param = GetParamOfTranscationModelForKillOrder(accPair, item);
                                //_term.TransactionOrder(param, null);
                            }
                        }
                    }
                }
        }


        /// <summary>
        /// Снятие всех заявок на продажу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSellOrderKill_Click(object sender, RoutedEventArgs e)
        {
            if (_term != null && _term.IsConnect)
                if (CmbxSymbol.SelectedItem is ISecurity && CmbxClientCode.SelectedItem is AccountsPair)
                {
                    if (!String.IsNullOrEmpty(CmbxSymbol.SelectedItem.ToString()) &&
                        !String.IsNullOrEmpty(CmbxClientCode.SelectedItem.ToString()))
                    {
                        //var orders = _term.GetOrders(CmbxClientCode.SelectedItem.ToString(), CmbxSymbol.SelectedItem.ToString());
                        ////_term.BaseMon.Raise_OnSomething("Блок 20. Снятие заявок на продажу по инструменту = " + CmbxSymbol.SelectedItem + " по счету " + CmbxClientCode.SelectedItem);

                        //for (int i = 0; i < orders.Count; i++)
                        //{
                        //    var item = orders[i];
                        //    if (item.Status == ConfigTermins.Active && item.Operation == ConfigTermins.Sell)
                        //    {
                        //        var param = GetParamOfTranscationModelForKillOrder(CmbxClientCode.SelectedItem as AccountsPair, item);
                        //        _term.TransactionOrder(param, null);
                        //    }
                        //}

                        var accPair = CmbxClientCode.SelectedItem as AccountsPair;
                        var sec = CmbxSymbol.SelectedItem as ISecurity;
                        for (int i = 0; i < dataStorage.OrdersList.Count; i++)
                        {
                            var item = dataStorage.OrdersList[i];
                            if (item.Account == accPair.Account && item.ClientCode == accPair.ClientCode && item.Operation == ConfigTermins.Sell &&
                                item.Status == ConfigTermins.Active && item.Symbol == sec.Seccode)
                            {
                                var param = GetParamOfTranscationModelForKillOrder(accPair, item);
                                //_term.TransactionOrder(param, null);
                            }
                        }
                    }
                }
        }

        /// <summary>
        /// Снятие всех заявок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAllKillOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_term != null && _term.IsConnect)
                if (CmbxSymbol.SelectedItem is ISecurity && CmbxClientCode.SelectedItem is AccountsPair)
                {
                    if (!String.IsNullOrEmpty(CmbxSymbol.SelectedItem.ToString()) &&
                        !String.IsNullOrEmpty(CmbxClientCode.SelectedItem.ToString()))
                    {

                        var accPair = CmbxClientCode.SelectedItem as AccountsPair;
                        var sec = CmbxSymbol.SelectedItem as ISecurity;
                        for (int i = 0; i < dataStorage.OrdersList.Count; i++)
                        {
                            var item = dataStorage.OrdersList[i];
                            if (item.Account == accPair.Account && item.ClientCode == accPair.ClientCode && 
                                item.Status == ConfigTermins.Active && item.Symbol == sec.Seccode)
                            {
                                var param = GetParamOfTranscationModelForKillOrder(accPair, item);
                                //_term.TransactionOrder(param, null);
                            }
                        }
                    }
                }
        }
        #endregion



        #endregion


        #region Транзакции


        /// <summary>
        /// Получаем параметры для транзакции
        /// </summary>
        /// <returns></returns>
        public ParamOfTransactionModel GetParamOfTranscationModelOrder(AccountsPair client, ISecurity finInfo,
            string oper, string typeOrder, bool isMarket, double price, int qty)
        {

            var paramOfTransaction = new ParamOfTransactionModel();
            paramOfTransaction.Operation = oper == ConfigTermins.Buy ? 'B' : 'S';
            paramOfTransaction.IsMarketOrder = "L";
            paramOfTransaction.Account = client.Account;
            paramOfTransaction.ClientCode = client.ClientCode;
            paramOfTransaction.TrandId = 555.ToString();
            paramOfTransaction.Symbol = finInfo.Seccode;
            paramOfTransaction.ClassCode = finInfo.ClassCode;

            paramOfTransaction.Price = price;
            paramOfTransaction.Quantity = qty;
            if (isMarket)
                paramOfTransaction.Price = GetMarketPrice(finInfo, true, oper);


            paramOfTransaction.TypeOrder = typeOrder;

            return paramOfTransaction;
        }


        /// <summary>
        /// Параметры для стоп-заявки
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public ParamOfTransactionModel GetParamOfTranscationModelStopOrder(AccountsPair client, ISecurity finInfo,
            string oper, double price, double priceFilled, int qty)
        {

            var paramOfTransaction = new ParamOfTransactionModel();
            paramOfTransaction.Account = client.Account;
            paramOfTransaction.ClientCode = client.ClientCode;
            paramOfTransaction.TrandId = 666.ToString();
            paramOfTransaction.TypeOrder = CfgSourceEts.TypeRequestForTransactionStopLimit;
            paramOfTransaction.Operation = oper == ConfigTermins.Buy ? 'B' : 'S';
            paramOfTransaction.Symbol = finInfo.Seccode;
            paramOfTransaction.ClassCode = finInfo.ClassCode;

            paramOfTransaction.Quantity = qty;
            paramOfTransaction.StopPrice = price;
            paramOfTransaction.StopPriceFilled = priceFilled;


            return paramOfTransaction;
        }


        #region Снятие заявок
        /// <summary>
        /// Снятие заявки    
        /// </summary>
        /// <param name="brmModel"></param>
        /// <param name="term"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ParamOfTransactionModel GetParamOfTranscationModelForKillOrder(AccountsPair client, IOrders order)
        {
            var paramOfTransaction = new ParamOfTransactionModel();
            paramOfTransaction.Account = client.Account;
            paramOfTransaction.ClientCode = client.ClientCode;
            paramOfTransaction.TrandId = 500.ToString();
            paramOfTransaction.OrderNumberForKill = order.Number;
            paramOfTransaction.OrderIdForKill = order.Id;
            paramOfTransaction.Symbol = order.Symbol;
            paramOfTransaction.ClassCode = order.ClassCode;
            //paramOfTransaction.ClassCode = _term.GetClasscode(paramOfTransaction.Symbol);
            paramOfTransaction.TypeOrder = CfgSourceEts.TypeRequestForTransactionKillOrder;

            return paramOfTransaction;
        }

        /// <summary>
        /// Получаем параметры для транзакции для снятия стоп-заявки
        /// </summary>
        /// <returns></returns>
        public ParamOfTransactionModel GetParamOfTranscationModelForKillStop(AccountsPair client, IStop order)
        {
            var paramOfTransaction = new ParamOfTransactionModel();
            paramOfTransaction.Account = client.Account;
            paramOfTransaction.ClientCode = client.ClientCode;
            paramOfTransaction.TrandId = 400.ToString();
            paramOfTransaction.StopOrderNumberForKill = order.Number;
            paramOfTransaction.StopOrderIdForKill = order.Id;
            paramOfTransaction.Symbol = order.Symbol;
            paramOfTransaction.ClassCode = order.ClassCode;
            //paramOfTransaction.ClassCode = _term.GetClasscode(paramOfTransaction.Symbol);
            paramOfTransaction.TypeOrder = CfgSourceEts.TypeRequestForTransactionKillStopOrder;

            return paramOfTransaction;
        }

        #endregion

        //private void BtnSetting_Click(object sender, RoutedEventArgs e)
        //{
        //        GlassSetting galss = new GlassSetting(_allTerminal, _dataRobots);
        //        galss.Show();
        //}

        #endregion


    }

}
