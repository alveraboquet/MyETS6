using System;

namespace ProviderContract.Services
{
    public static class DoubleService
    {
        /// <summary>
        /// Преобразует строку в Double
        /// </summary>
        /// <param name="source">Исходная строка</param>
        /// <returns>Значение Double</returns>
        public static double GetDouble(string source)
        {
            _ = double.TryParse(source,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double res);

            return res;
        }
    }
}
