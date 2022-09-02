using System.Globalization;

namespace ProviderContract.Services
{
    public static class StringService
    {
        /// <summary>
        /// Преобразует double-число в строку
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Строчное представление числа с точкой</returns>
        public static string GetString(double value)
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }
    }
}
