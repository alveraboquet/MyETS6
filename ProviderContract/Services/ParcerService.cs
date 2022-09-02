using Newtonsoft.Json;
using System;
using System.Globalization;

namespace ProviderContract.Services
{
    /// <summary>
    /// Json-парсер
    /// </summary>
    public static class ParcerService
    {
        private const string API_JSON_PARSE_ERROR = "Api json parse error: ";
        private static readonly JsonSerializerSettings ignoreSettings;

        static ParcerService()
        {
            ignoreSettings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }
        /// <summary>
        /// Преобразует JSON-строку в объект
        /// </summary>
        /// <typeparam name="T">Тип данных объекта</typeparam>
        /// <param name="response">JSON-строка</param>
        /// <returns>Объект типа T</returns>
        public static T StringJsonToObject<T>(string response)
        {
            T result;
            try
            {
                result = JsonConvert.DeserializeObject<T>(response, ignoreSettings);
            }
            catch (Exception ex)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, API_JSON_PARSE_ERROR + ex.Message));
            }
            return result;
        }
    }
}
