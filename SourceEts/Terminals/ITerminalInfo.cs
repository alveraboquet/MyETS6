using SourceEts.Table;
using System.Collections.ObjectModel;

namespace SourceEts.Terminals
{
    public interface ITerminalInfo
    {
        bool IsConnect { get; set; }
        bool IsUse { get; set; }
        string Comment { get; set; }
        /// <summary>
        /// Какая-то дополнительная информация
        /// </summary>
        string AddInfo { get; set; }
        string Terminal { get; set; }
        /// <summary>
        /// Поставщик, Транзакции, все
        /// </summary>
        string TypeTerminal { get; set; }

        string AccountsString { get; set; }
        string Name { get; set; }

        ObservableCollection<AccountsPair> AccountsPairs { get; set; }

        double TotalSecond { get; set; }

        int CurCounTransaction { get; set; }

        string ColorStatusConnection { get; set; }


    }
}
