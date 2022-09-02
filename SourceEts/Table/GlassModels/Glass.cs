using CommonDataContract;
using SourceEts.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SourceEts.Table
{
    /// <summary>
    /// Стакан
    /// </summary>
    public class Glass : ViewModelBase/* IGlass*/
    {
        public List<GlassSaveModel> GlassSaveM = new List<GlassSaveModel>();
        #region private 

        public int Deep { get; set; }
        private string _symbol;
        private string _classCode;
        private List<IGlassQuotation> _quotationsBuy;
        private List<IGlassQuotation> _quotationsSell;
        private bool _isFromObsCol;
        private ObservableCollection<IGlassQuotation> _quotationsFull;

        #endregion

        #region Public





        /// <summary>
        /// Код инструмента
        /// </summary>
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                if (_symbol != value)
                {
                    _symbol = value;
                    RaisePropertyChanged("Symbol");
                }
            }
        }

        /// <summary>
        /// Код класса инструмента
        /// </summary>
        public string ClassCode
        {
            get { return _classCode; }
            set
            {
                if (_classCode != value)
                {
                    _classCode = value;
                    RaisePropertyChanged("ClassCode");
                }
            }
        }

        /// <summary>
        /// Котировки по стакану покупка
        /// </summary>
        public List<IGlassQuotation> QuotationsBuy
        {
            get { return _quotationsBuy ?? (_quotationsBuy = new List<IGlassQuotation>()); }
            set
            {
                if (_quotationsBuy != value)
                {
                    _quotationsBuy = value;
                    RaisePropertyChanged("QuotationsBuy");
                }
            }
        }

        /// <summary>
        /// Котировки по стакану продажа
        /// </summary>
        public List<IGlassQuotation> QuotationsSell
        {
            get { return _quotationsSell ?? (_quotationsSell = new List<IGlassQuotation>()); }
            set
            {
                if (_quotationsSell != value)
                {
                    _quotationsSell = value;
                    RaisePropertyChanged("QuotationsSell");
                }
            }
        }

        /// <summary>
        /// нужно ли формировать полный стакан для вывода на экран
        /// </summary>
        public bool IsFromObsCol
        {
            get { return _isFromObsCol; }
            set
            {
                if (_isFromObsCol != value)
                {
                    _isFromObsCol = value;
                    RaisePropertyChanged("IsFromObsCol");
                }
            }
        }

        /// <summary>
        /// Стакан для вывода на экран, с котировками на продажу и покупку
        /// </summary>
        public ObservableCollection<IGlassQuotation> QuotationsFull
        {
            get { return _quotationsFull ?? (_quotationsFull = new ObservableCollection<IGlassQuotation>()); }
            set
            {
                if (_quotationsFull != value)
                {
                    _quotationsFull = value;
                    RaisePropertyChanged("QuotationsFull");
                }
            }
        }
        #endregion

    }
}
