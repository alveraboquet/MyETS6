using CommonDataContract.AbstractDataTypes;
using SourceEts;
using System.Collections.Generic;

namespace Adapter
{
    public class SortSymbols
    {
        private int _index = 0;
        /// <summary>
        /// Формируем список инструментов
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="terminal"></param>
        public void AddSortInst(AbstractTerminal terminal, ISecurity sec, string sortBy)
        {

            List<AllInstrForTrwModel> allInstrForTrwModelList = terminal.AllInstrForTrwModelList;
            List<AvalibleInstrumentsModel> avalibleInstruments = terminal.AvalibleInstruments;
            bool selected = false;
            foreach (var item in avalibleInstruments)
            {
                if (item.Security == null)
                    if (sec.Seccode == item.Symbol && sec.ClassCode == item.ClassCode)
                    {
                        item.ShortName = sec.ShortName;
                        item.Security = sec;
                        selected = true;
                    }
            }

            bool add = true;
            string name = sec.Seccode;
            string classCodeVis = sortBy;


            foreach (var classCode in allInstrForTrwModelList)
            {
                if (classCode.ClassCodeVisible == classCodeVis)
                {
                    int insertIndex = 0;
                    for (int i = 0; i < classCode.SeccodeList.Count; i++)
                    {
                        var secCode = classCode.SeccodeList[i];
                        string nameSecCode = secCode.Symbol;
                        if (secCode != null)
                        {
                            if (nameSecCode == name)
                            {
                                add = false;
                                if (secCode.Security != null)
                                {
                                    secCode.Security.Accuracy = sec.Accuracy;
                                    secCode.Security.MinAmount = sec.MinAmount;
                                    secCode.Security.MinNational = sec.MinNational;
                                    secCode.Security.MinPrice = sec.MinPrice;
                                    secCode.Security.MinStep = sec.MinStep;
                                    secCode.Security.LotSize = sec.LotSize;
                                    secCode.Security.MaxAmount = sec.MaxAmount;
                                    secCode.Security.MaxPrice = sec.MaxPrice;
                                    secCode.Security.PointCost = sec.PointCost;
                                }

                                break;
                            }

                            if (nameSecCode.CompareTo(name) > 0)
                                break;
                        }
                        insertIndex = i + 1;
                    }

                    if (add)
                        classCode.SeccodeList.Insert(insertIndex,
                            new AvalibleInstrumentsModel
                            {
                                ClassCodeVisible = classCodeVis,
                                Symbol = sec.Seccode,
                                ClassCode = sec.ClassCode,
                                ShortName = sec.ShortName,
                                IsSelected = selected,
                                Security = sec
                            });

                    add = false;
                }
            }
            if (add)
            {
                var mod = new AvalibleInstrumentsModel
                {
                    ClassCodeVisible = classCodeVis,

                    Symbol = sec.Seccode,
                    ClassCode = sec.ClassCode,
                    ShortName = sec.ShortName,
                    IsSelected = selected,
                    Security = sec
                };
                _index = 0;

                allInstrForTrwModelList.Insert(_index, new AllInstrForTrwModel
                {
                    ClassCode = sec.ClassCode,
                    ClassCodeVisible = classCodeVis,
                    SeccodeList = new List<AvalibleInstrumentsModel> { mod }
                });
            }
        }


    }
}
