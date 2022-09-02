using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using CommonDataContract;

namespace SourceEts.Config
{
    [Serializable]
    public class RebalanceDirectoryModel : ViewModelBase, ISerializable
    {
        public RebalanceWalletInfoModel GetWallet(string coin)
        {
            for (int i = ListWalletInfo.Count - 1; i >= 0; i--)
            {
                if (ListWalletInfo[i].Currency == coin.ToUpper())
                    return ListWalletInfo[i];
            }

            return null;
        }

        #region private

        private string _name;
        private string _exchange;
        private string _account;
        private string _clientCode;
        private ObservableCollection<RebalanceWalletInfoModel> _listWalletInfo;

        #endregion

        #region public

        /// <summary>
        /// . 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public string Exchange
        {
            get { return _exchange; }
            set
            {
                if (_exchange != value)
                {
                    _exchange = value;
                    RaisePropertyChanged("Exchange");
                }
            }
        }

        /// <summary>
        /// . 
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
        /// . 
        /// </summary>
        public string ClientCode
        {
            get { return _clientCode; }
            set
            {
                if (_clientCode != value)
                {
                    _clientCode = value;
                    RaisePropertyChanged("ClientCode");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public ObservableCollection<RebalanceWalletInfoModel> ListWalletInfo
        {
            get { return _listWalletInfo ?? (_listWalletInfo = new ObservableCollection<RebalanceWalletInfoModel>()); }
            set
            {
                if (_listWalletInfo != value)
                {
                    _listWalletInfo = value;
                    RaisePropertyChanged("ListWalletInfo");
                }
            }
        }




        #endregion


        public RebalanceDirectoryModel()
        {
        }


        public RebalanceDirectoryModel(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Exchange = info.GetString("Exchange");
            Account = info.GetString("Account");
            ClientCode = info.GetString("ClientCode");
            ListWalletInfo = (ObservableCollection<RebalanceWalletInfoModel>)info.GetValue("ListWalletInfo", typeof(ObservableCollection<RebalanceWalletInfoModel>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Exchange", Exchange);
            info.AddValue("Account", Account);
            info.AddValue("ClientCode", ClientCode);
            info.AddValue("ListWalletInfo", ListWalletInfo);

        }
    }
}
