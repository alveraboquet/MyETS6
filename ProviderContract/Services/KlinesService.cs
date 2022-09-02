using ProviderContract.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ProviderContract.Services
{
    public static class KlinesService
    {
        /// <summary>
        /// Конвертирует группу свечей в свечи с большим таймфреймом
        /// </summary>
        /// <param name="sourceList">Исходные свечи</param>
        /// <param name="multiplier">Множитель</param>
        /// <returns>Список свечей, в котором свечей в multiplier раз меньше, таймфрейм в multiplier раз больше</returns>
        public static List<Kline> GetMultipliedTimeframe(List<Kline> sourceList, int multiplier)
        {
            if (multiplier == 1)
            {
                return sourceList;
            }

            List<Kline> result = new List<Kline>();

            // Переворачиваем лист, чтобы группировать от самой ранней к самой поздней
            sourceList.Reverse();

            // Разбиваем свечи на группы по multiplier штук
            // Если нужно из 1 минуты сделать 5, то соответственно
            // multiplier = 5 и нужно 5 свечей по 1 минуте, для того, чтобы
            // получить одну 5-минутную
            List<List<Kline>> klines = sourceList.SplitList(multiplier).ToList();

            foreach (List<Kline> klineGroup in klines)
            {
                Kline aggKline = new Kline()
                {
                    Close = klineGroup.Last().Close,
                    High = klineGroup.Max(k => k.High),
                    Low = klineGroup.Min(k => k.Low),
                    Open = klineGroup.First().Open,
                    OpenTime = klineGroup.First().OpenTime,
                    Pair = klineGroup.First().Pair,
                    Volume = klineGroup.Select(k => k.Volume).Aggregate((x, y) => x + y),
                    ClassCode = klineGroup.First().ClassCode
                };
                result.Add(aggKline);
            }

            // Переворачиваем лист с результами, чтобы пользователю представить свечи
            // от самой новой к самой старой
            result.Reverse();
            return result;
        }
    }
}
