using CommonDataContract.AbstractDataTypes;
using SourceEts.Table;
using SourceEts.Table.TableClass;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace SourceEts
{
    public class DataStorage
    {
        #region Singleton
        private DataStorage() { }
        private Dispatcher _mainDispatcher;
        static DataStorage()
        {
            Instance = new DataStorage();
        }
        public static DataStorage Instance { get; private set; }
        #endregion Singleton

        #region DataCollections
        public ObservableList<ISecurity> CurrentParamModelList { get; } = new ObservableList<ISecurity>();
        public ObservableList<IPositionShares> PositionSharesList { get; } = new ObservableList<IPositionShares>();
        public ObservableList<IPositionFutures> PositionFuturesList { get; } = new ObservableList<IPositionFutures>();
        public ObservableList<IMoneyShares> LimitMoneySharesList { get; } = new ObservableList<IMoneyShares>();
        public ObservableList<IMoneyFutures> LimitMoneyFuturesList { get; } = new ObservableList<IMoneyFutures>();
        public ObservableList<IOrders> OrdersList { get; } = new ObservableList<IOrders>();
        public ObservableList<IDeals> DealsList { get; } = new ObservableList<IDeals>();
        public ObservableList<IStop> StopOrderList { get; } = new ObservableList<IStop>();
        public ObservableList<Glass> Glasses { get; } = new ObservableList<Glass>();
        #endregion DataCollections

        #region Add
        public void AddISecurityCommonTable(Dispatcher dispatcher, ISecurity sec)
        {
            if (dispatcher.CheckAccess())
            {
                CurrentParamModelList.Add(sec);
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() =>
                CurrentParamModelList.Add(sec)
                    ), DispatcherPriority.Normal, null);
            }

            //InvokeDispatcher(() => CurrentParamModelList.Add(sec));
        }

        public void AddIPositionSharesCommonTable(IPositionShares sharePos)
        {
            InvokeDispatcher(() => PositionSharesList.Add(sharePos));
        }
        public void AddIPositionFuturesCommonTable(IPositionFutures futuresPos)
        {
            InvokeDispatcher(() => PositionFuturesList.Add(futuresPos));
        }

        public void AddOrdersCommonTable(Dispatcher dispatcher, IOrders order)
        {
            if (dispatcher.CheckAccess())
            {
                OrdersList.Add(order);
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() =>
                OrdersList.Add(order)
                    ), DispatcherPriority.Normal, null);
            }

            //InvokeDispatcher(() => OrdersList.Add(order));
        }
        public void AddDealsCommonTable(Dispatcher dispatcher, IDeals deal)
        {
            if (dispatcher.CheckAccess())
            {
                DealsList.Add(deal);
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() =>
                DealsList.Add(deal)
                    ), DispatcherPriority.Normal, null);
            }

            //InvokeDispatcher(() => DealsList.Add(deal));
        }

        public void AddMoneySharesCommonTable(Dispatcher dispatcher, IMoneyShares moneyShares) 
        {
            if (dispatcher.CheckAccess())
            {
                LimitMoneySharesList.Add(moneyShares);
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() =>
                LimitMoneySharesList.Add(moneyShares)
                    ), DispatcherPriority.Normal, null);
            }

            //InvokeDispatcher(() => LimitMoneySharesList.Add(moneyShares));
        }

        public void AddMoneyFuturesCommonTable(IMoneyFutures moneyFutures)
        {
            InvokeDispatcher(() => LimitMoneyFuturesList.Add(moneyFutures));
        }

        public void AddIStopCommonTable(IStop stop)
        {
            InvokeDispatcher(() => StopOrderList.Add(stop));
        }
        #endregion Add

        private void InvokeDispatcher(Action action)
        {
            Dispatcher.CurrentDispatcher.Invoke(action);
        }

        public void AddGlass(Dispatcher dispatcher, Glass glass)
        {

            Glasses.Add(glass);

            if (dispatcher.CheckAccess())
            {
                CreteFullGlass(glass);
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() =>
                CreteFullGlass(glass))
            , DispatcherPriority.Normal, null);
            }
        }

        private void CreteFullGlass(Glass glass)
        {
            for (int i = 0; i < glass.Deep * 2; i++)
            {
                glass.QuotationsFull.Add(new GlassQuotation { });
            }

            for (int i = 0; i < glass.Deep; i++)
            {
                glass.QuotationsFull[glass.Deep + i] = glass.QuotationsBuy[i];
                glass.QuotationsFull[glass.Deep - 1 - i] = glass.QuotationsSell[i];
            }
        }
    }
}
