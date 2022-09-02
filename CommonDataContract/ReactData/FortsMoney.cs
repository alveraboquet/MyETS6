using CommonDataContract.AbstractDataTypes;
using System;
using System.Collections.Generic;

namespace CommonDataContract.ReactData
{
    public class FortsMoney : ViewModelBase, IMoneyFutures
    {
        #region Private

        private string _account;
        private List<int> _marketsList;
        private string _market;

        // private string _shortname;
        // private double _current;
        // private double _blocked;
        // private double _free;
        // private double _varmargin;

        private String _typeLimit;//Тип лимита для рынка FORTS: «Ден.средства» - стоимость денежных средств в обеспечении, 
        private double _kmolotiluas;//Коэффициент, определяющий какая часть средств блокируется из залогового, а какая из собственного денежного лимита. Коэффициент представляет собой число от 0 до 1. Например, если коэффициент равен 0.7, то заблокируется 70% в  собственном денежном лимите и 30% - в залоговом. Параметр относится к рынку FORTS.
        private double _lastLimitOpenPosition;//Лимит открытых позиций по всем инструментам предыдущей торговой сессии в денежном выражении
        private double _limitOpenPosition;//Текущий лимит открытых позиций по всем инструментам в денежном выражении Для рынка RTS Standard отображается лимит на покупку спот-активов
        private double _currentEmptyPosition;//Совокупное денежное обеспечение, резервируемое под открытые позиции и торговые операции текущей сессии. Для рынка RTS Standard учитываются только позиции по главным спот-активам*
        private double _currentEmptyOrder;//Величина гарантийного обеспечения, зарезервированного под активные заявки, в денежном выражении
        private double _currentEmptyOpen;//Величина гарантийного обеспечения, зарезервированного под открытые позиции, в денежном выражении
        private double _planEmptyPosition;//Планируемые чистые позиции по всем инструментам в денежном выражении Соответствует параметру «Свободные средства» рынка FORTS.
        private double _variableMarga;//Вариационная маржа по позициям клиента, по всем инструментам
        private double _dohod;//Накопленный доход на клиентском счете, рассчитываемый для операций со срочными контрактами
        private double _bonusByOption;//Премия по опционным позициям, рассчитанная по правилам торговой системы. Для типа лимита «Клиринговые ден.средства» и «Клиринговые залоговые ден.средства» соответствует параметру «Опционная премия» рынка FORTS.
        private double _exchangeFee;//Сумма, взимаемая биржевым комитетом за проведение биржевых сделок. Параметр рынка FORTS.
        private double _countGo; //Коэффициент клиентского гарантийного обеспечения


        #endregion

        #region Public

        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public string Account
        {
            get { return _account; }
            set
            {
                if (_account != value)
                {
                    _account = value;
                    RaisePropertyChanged("Account");
                }
            }
        }

        /// <summary>
        /// рынок
        /// </summary>
        public string Market
        {
            get { return _market; }
            set
            {
                if (_market != value)
                {
                    _market = value;
                    RaisePropertyChanged("Market");
                }
            }
        }

        /// <summary>
        /// Внутренний код рынка
        /// </summary>
        public List<int> MarketsList
        {
            get { return _marketsList; }
            set
            {
                if (_marketsList != value)
                {
                    _marketsList = value;
                    RaisePropertyChanged("MarketsList");
                }
            }
        }

        /// <summary>
        /// Тип лимита для рынка FORTS: «Ден.средства»  стоимость денежных средств в обеспечении,. 
        /// </summary>
        public String TypeLimit
        {
            get { return _typeLimit; }
            set
            {
                if (_typeLimit != value)
                {
                    _typeLimit = value;
                    RaisePropertyChanged("TypeLimit");
                }
            }
        }

        ///// <summary>
        ///// Наименование вида средств
        ///// </summary>
        //public string Shortname
        //{
        //    get { return _shortname; }
        //    set
        //    {
        //        if (_shortname != value)
        //        {
        //            _shortname = value;
        //            RaisePropertyChanged("Shortname");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Текущие
        ///// </summary>
        //public double Current
        //{
        //    get { return _current; }
        //    set
        //    {
        //        if (_current != value)
        //        {
        //            _current = value;
        //            RaisePropertyChanged("Current");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Заблокировано
        ///// </summary>
        //public double Blocked
        //{
        //    get { return _blocked; }
        //    set
        //    {
        //        if (_blocked != value)
        //        {
        //            _blocked = value;
        //            RaisePropertyChanged("Blocked");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Свободные
        ///// </summary>
        //public double Free
        //{
        //    get { return _free; }
        //    set
        //    {
        //        if (_free != value)
        //        {
        //            _free = value;
        //            RaisePropertyChanged("Free");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Опер. маржа
        ///// </summary>
        //public double Varmargin
        //{
        //    get { return _varmargin; }
        //    set
        //    {
        //        if (_varmargin != value)
        //        {
        //            _varmargin = value;
        //            RaisePropertyChanged("Varmargin");
        //        }
        //    }
        //}

        /// <summary>
        /// Коэффициент, определяющий какая часть средств блокируется из залогового, а какая из собственного денежного лимита. Коэффициент представляет собой число от 0 до 1. Например, если коэффициент равен 0.7, то заблокируется 70% в  собственном денежном лимите и 30%  в залоговом. Параметр относится к рынку FORTS.. 
        /// </summary>
        public double Kmolotiluas
        {
            get { return _kmolotiluas; }
            set
            {
                if (Math.Abs(_kmolotiluas - value) > CfgSourceEts.MyEpsilon)
                {
                    _kmolotiluas = value;
                    RaisePropertyChanged("Kmolotiluas");
                }
            }
        }

        /// <summary>
        /// Лимит открытых позиций по всем инструментам предыдущей торговой сессии в денежном выражении. 
        /// </summary>
        public double LastLimitOpenPosition
        {
            get { return _lastLimitOpenPosition; }
            set
            {
                if (Math.Abs(_lastLimitOpenPosition - value) > CfgSourceEts.MyEpsilon)
                {
                    _lastLimitOpenPosition = value;
                    RaisePropertyChanged("LastLimitOpenPosition");
                }
            }
        }

        /// <summary>
        /// Текущий лимит открытых позиций по всем инструментам в денежном выражении Для рынка RTS Standard отображается лимит на покупку спотактивов. 
        /// </summary>
        public double LimitOpenPosition
        {
            get { return _limitOpenPosition; }
            set
            {
                if (Math.Abs(_limitOpenPosition - value) > CfgSourceEts.MyEpsilon)
                {
                    _limitOpenPosition = value;
                    RaisePropertyChanged("LimitOpenPosition");
                }
            }
        }

        /// <summary>
        /// Совокупное денежное обеспечение, резервируемое под открытые позиции и торговые операции текущей сессии. Для рынка RTS Standard учитываются только позиции по главным спотактивам*. 
        /// </summary>
        public double CurrentEmptyPosition
        {
            get { return _currentEmptyPosition; }
            set
            {
                if (Math.Abs(_currentEmptyPosition - value) > CfgSourceEts.MyEpsilon)
                {
                    _currentEmptyPosition = value;
                    RaisePropertyChanged("CurrentEmptyPosition");
                }
            }
        }

        /// <summary>
        /// Величина гарантийного обеспечения, зарезервированного под активные заявки, в денежном выражении. 
        /// </summary>
        public double CurrentEmptyOrder
        {
            get { return _currentEmptyOrder; }
            set
            {
                if (Math.Abs(_currentEmptyOrder - value) > CfgSourceEts.MyEpsilon)
                {
                    _currentEmptyOrder = value;
                    RaisePropertyChanged("CurrentEmptyOrder");
                }
            }
        }

        /// <summary>
        /// Величина гарантийного обеспечения, зарезервированного под открытые позиции, в денежном выражении. 
        /// </summary>
        public double CurrentEmptyOpen
        {
            get { return _currentEmptyOpen; }
            set
            {
                if (Math.Abs(_currentEmptyOpen - value) > CfgSourceEts.MyEpsilon)
                {
                    _currentEmptyOpen = value;
                    RaisePropertyChanged("CurrentEmptyOpen");
                }
            }
        }

        /// <summary>
        /// Планируемые чистые позиции по всем инструментам в денежном выражении Соответствует параметру «Свободные средства» рынка FORTS.. 
        /// </summary>
        public double PlanEmptyPosition
        {
            get { return _planEmptyPosition; }
            set
            {
                if (Math.Abs(_planEmptyPosition - value) > CfgSourceEts.MyEpsilon)
                {
                    _planEmptyPosition = value;
                    RaisePropertyChanged("PlanEmptyPosition");
                }
            }
        }

        /// <summary>
        /// Вариационная маржа по позициям клиента, по всем инструментам. 
        /// </summary>
        public double VariableMarga
        {
            get { return _variableMarga; }
            set
            {
                if (Math.Abs(_variableMarga - value) > CfgSourceEts.MyEpsilon)
                {
                    _variableMarga = value;
                    RaisePropertyChanged("VariableMarga");
                }
            }
        }

        /// <summary>
        /// Накопленный доход на клиентском счете, рассчитываемый для операций со срочными контрактами. 
        /// </summary>
        public double Dohod
        {
            get { return _dohod; }
            set
            {
                if (Math.Abs(_dohod - value) > CfgSourceEts.MyEpsilon)
                {
                    _dohod = value;
                    RaisePropertyChanged("Dohod");
                }
            }
        }

        /// <summary>
        /// Премия по опционным позициям, рассчитанная по правилам торговой системы. Для типа лимита «Клиринговые ден.средства» и «Клиринговые залоговые ден.средства» соответствует параметру «Опционная премия» рынка FORTS.. 
        /// </summary>
        public double BonusByOption
        {
            get { return _bonusByOption; }
            set
            {
                if (Math.Abs(_bonusByOption - value) > CfgSourceEts.MyEpsilon)
                {
                    _bonusByOption = value;
                    RaisePropertyChanged("BonusByOption");
                }
            }
        }

        /// <summary>
        /// Сумма, взимаемая биржевым комитетом за проведение биржевых сделок. Параметр рынка FORTS.. 
        /// </summary>
        public double ExchangeFee
        {
            get { return _exchangeFee; }
            set
            {
                if (Math.Abs(_exchangeFee - value) > CfgSourceEts.MyEpsilon)
                {
                    _exchangeFee = value;
                    RaisePropertyChanged("ExchangeFee");
                }
            }
        }

        /// <summary>
        /// Коэффициент клиентского гарантийного обеспечения. 
        /// </summary>
        public double CountGo
        {
            get { return _countGo; }
            set
            {
                if (Math.Abs(_countGo - value) > CfgSourceEts.MyEpsilon)
                {
                    _countGo = value;
                    RaisePropertyChanged("CountGo");
                }
            }
        }

        #endregion
    }
}
