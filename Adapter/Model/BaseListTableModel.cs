using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommonDataContract;
using CommonDataContract.AbstractDataTypes;
using SourceEts;
using SourceEts.Table;

namespace Adapter.Model
{
    /// <summary>
    /// Название таблиц с параметрами для абстрактного терминала
    /// </summary>
    public class BaseListTableModel : ViewModelBase
    {
        #region private

        private List<ITick> _tickList;
        private List<ISecurity> _currentParamModelList;
        private List<IOrders> _ordersList;
        private List<IDeals> _dealsList;
        private List<IStop> _stopsList;
        private List<IPositionFutures> _positionFuturesList;
        private List<IPositionShares> _positionSharesList;
        private List<IMoneyShares> _limitMoneySharesList;
        private List<IMoneyFutures> _limitMoneyFuturesList;
        private List<ISecurity> _allcurrentParamModelList;

        #endregion

        #region public
        /// <summary>
        /// Таблица позиций по фьючерсам и опционам
        /// </summary>
        public List<IPositionFutures> PositionFuturesList
        {
            get { return _positionFuturesList ?? (_positionFuturesList = new List<IPositionFutures>()); }
            set
            {
                if (_positionFuturesList != value)
                {
                    _positionFuturesList = value;
                    RaisePropertyChanged("PositionFuturesList");
                }
            }
        }


        /// <summary>
        /// Таблица позиций по акциям
        /// </summary>
        public List<IPositionShares> PositionSharesList
        {
            get { return _positionSharesList ?? (_positionSharesList = new List<IPositionShares>()); }
            set
            {
                if (_positionSharesList != value)
                {
                    _positionSharesList = value;
                    RaisePropertyChanged("PositionSharesList");
                }
            }
        }

        /// <summary>
        /// Таблица лимитов денежных средств по акциям. 
        /// </summary>
        public List<IMoneyShares> LimitMoneySharesList
        {
            get
            {
                if (_limitMoneySharesList == null)
                    _limitMoneySharesList = new List<IMoneyShares>();
                return _limitMoneySharesList;
            }
            set
            {
                if (_limitMoneySharesList != value)
                {
                    _limitMoneySharesList = value;
                    RaisePropertyChanged("LimitMoneySharesList");
                }
            }
        }

        /// <summary>
        /// Таблица лимитов по фьючерсам
        /// </summary>
        public List<IMoneyFutures> LimitMoneyFuturesList
        {
            get
            {
                if (_limitMoneyFuturesList == null)
                    _limitMoneyFuturesList = new List<IMoneyFutures>();
                return _limitMoneyFuturesList;
            }
            set
            {
                if (_limitMoneyFuturesList != value)
                {
                    _limitMoneyFuturesList = value;
                    RaisePropertyChanged("LimitMoneyFuturesList");
                }
            }
        }


        /// <summary>
        /// Коллекция всех инструментов
        /// </summary>
        public List<ISecurity> AllCurrentParamModelList
        {
            get { return _currentParamModelList ?? (_currentParamModelList = new List<ISecurity>()); }
            set
            {
                if (_currentParamModelList != value)
                {
                    _currentParamModelList = value;
                    RaisePropertyChanged("AllcurrentParamModelList");
                }
            }
        }


        /// <summary>
        /// Таблица сделок
        /// </summary>
        public List<IDeals> DealsList
        {
            get { return _dealsList ?? (_dealsList = new List<IDeals>()); }
            set
            {
                if (_dealsList != value)
                {
                    _dealsList = value;
                    RaisePropertyChanged("DealsList");
                }
            }
        }

        /// <summary>
        /// Таблица стоп-заявок
        /// </summary>
        public List<IStop> StopsList
        {
            get { return _stopsList ?? (_stopsList = new List<IStop>()); }
            set
            {
                if (_stopsList != value)
                {
                    _stopsList = value;
                    RaisePropertyChanged("StopsList");
                }
            }
        }

        /// <summary>
        /// Таблица заявок
        /// </summary>
        public List<IOrders> OrdersList
        {
            get { return _ordersList ?? (_ordersList = new List<IOrders>()); }
            set
            {
                if (_ordersList != value)
                {
                    _ordersList = value;
                    RaisePropertyChanged("OrdersList");
                }
            }
        }

        /// <summary>
        /// коллекция тиков
        /// </summary>
        public List<ITick> TickList
        {
            get { return _tickList ?? (_tickList = new List<ITick>()); }
            set
            {
                if (_tickList != value)
                {
                    _tickList = value;
                    RaisePropertyChanged("TickList");
                }
            }
        }


        /// <summary>
        ///Таблица текущих парамтеров, только рабочих элементов
        /// </summary>
        public List<ISecurity> CurrentParamModelList
        {
            get
            {
                return _allcurrentParamModelList ??
                       (_allcurrentParamModelList = new List<ISecurity>());
            }
            set
            {
                if (_allcurrentParamModelList != value)
                {
                    _allcurrentParamModelList = value;
                    RaisePropertyChanged("CurrentParamModelList");
                }
            }
        }
        #endregion
    }
}
