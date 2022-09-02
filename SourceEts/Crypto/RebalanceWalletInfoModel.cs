using System;
using System.Runtime.Serialization;
using CommonDataContract;

namespace SourceEts.Config
{
    [Serializable]
    public class RebalanceWalletInfoModel : ViewModelBase, ISerializable
    {
        #region private


        private string _currency;
        private string _wallet;
        private string _tag;
        private double _minVol;
        private int _minVolAccuracy;
        private double _minInputOfFunds;
        private double _minDeposit;
        private double _comisInputOfFunds;
        private double _comisWithdrawal;
        private int _timeAwaitDeposit;

        #endregion

        #region public

     
        /// <summary>
        /// . 
        /// </summary>
        public string Currency
        {
            get { return _currency; }
            set
            {
                if (_currency != value)
                {
                    _currency = value;
                    RaisePropertyChanged("Currency");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public string Wallet
        {
            get { return _wallet; }
            set
            {
                if (_wallet != value)
                {
                    _wallet = value;
                    RaisePropertyChanged("Wallet");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public string Tag
        {
            get { return _tag; }
            set
            {
                if (_tag != value)
                {
                    _tag = value;
                    RaisePropertyChanged("Tag");
                }
            }
        }

        /// <summary>
        /// Минимальный объем для перевода
        /// </summary>
        public double MinVol
        {
            get { return _minVol; }
            set
            {
                if (Math.Abs(_minVol - value) > CfgSourceEts.MyEpsilon)
                {
                    _minVol = value;
                    RaisePropertyChanged("MinVol");
                }
            }
        }


        /// <summary>
        /// Округление минимального перевода
        /// </summary>
        public int MinVolAccuracy
        {
            get { return _minVolAccuracy; }
            set
            {
                if (Math.Abs(_minVolAccuracy - value) > CfgSourceEts.MyEpsilon)
                {
                    _minVolAccuracy = value;
                    RaisePropertyChanged("MinVolAccuracy");
                }
            }
        }

        /// <summary>
        /// Минимальная сумма пополнения
        /// </summary>
        public double MinInputOfFunds
        {
            get { return _minInputOfFunds; }
            set
            {
                if (Math.Abs(_minInputOfFunds - value) > CfgSourceEts.MyEpsilon)
                {
                    _minInputOfFunds = value;
                    RaisePropertyChanged("MinInputOfFunds");
                }
            }
        }

        /// <summary>
        /// Мнимальный остаток на счете, при котором будет осуществлен перевод
        /// </summary>
        public double MinDeposit
        {
            get { return _minDeposit; }
            set
            {
                if (Math.Abs(_minDeposit - value) > CfgSourceEts.MyEpsilon)
                {
                    _minDeposit = value;
                    RaisePropertyChanged("MinDeposit");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public double ComisInputOfFunds
        {
            get { return _comisInputOfFunds; }
            set
            {
                if (Math.Abs(_comisInputOfFunds - value) >CfgSourceEts.MyEpsilon)
                {
                    _comisInputOfFunds = value;
                    RaisePropertyChanged("ComisInputOfFunds");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public double ComisWithdrawal
        {
            get { return _comisWithdrawal; }
            set
            {
                if (Math.Abs(_comisWithdrawal - value) > CfgSourceEts.MyEpsilon)
                {
                    _comisWithdrawal = value;
                    RaisePropertyChanged("ComisWithdrawal");
                }
            }
        }

        /// <summary>
        /// Время перевода монеты с одной биржи на другую, секунды 
        /// </summary>
        public int TimeAwaitDeposit
        {
            get { return _timeAwaitDeposit; }
            set
            {
                if (_timeAwaitDeposit != value)
                {
                    _timeAwaitDeposit = value;
                    RaisePropertyChanged("TimeAwaitDeposit");
                }
            }
        }

        #endregion

        public RebalanceWalletInfoModel()
        {
        }

        public RebalanceWalletInfoModel(SerializationInfo info, StreamingContext context)
        {
            Currency = info.GetString("Currency");
            Wallet = info.GetString("Wallet");
            Tag = info.GetString("Tag");
            MinVol = info.GetDouble("MinVol");
            MinVolAccuracy = info.GetInt32("MinVolAccuracy");
            MinInputOfFunds = info.GetDouble("MinInputOfFunds");
            MinDeposit = info.GetDouble("MinDeposit");
            ComisInputOfFunds = info.GetDouble("ComisInputOfFunds");
            ComisWithdrawal = info.GetDouble("ComisWithdrawal");
            TimeAwaitDeposit = info.GetInt32("TimeAwaitDeposit");

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Currency", Currency);
            info.AddValue("Wallet", Wallet);
            info.AddValue("Tag", Tag);
            info.AddValue("MinVol", MinVol);
            info.AddValue("MinVolAccuracy", MinVolAccuracy);
            info.AddValue("MinInputOfFunds", MinInputOfFunds);
            info.AddValue("MinDeposit", MinDeposit);
            info.AddValue("ComisInputOfFunds", ComisInputOfFunds);
            info.AddValue("ComisWithdrawal", ComisWithdrawal);
            info.AddValue("TimeAwaitDeposit", TimeAwaitDeposit);

        }
    }
}
