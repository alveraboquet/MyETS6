using Newtonsoft.Json;
using ProviderContract.Data.Enums;
using System.Collections.Generic;
using System.Text;

namespace ProviderContract.Data
{
    /// <summary>
    /// Http-запрос
    /// </summary>
    public class Request
    {
        private const string APPLICATION_JSON_CONTENT_TYPE = "application/json";
        private const string X_WWW_FORM_URLENCODED_CONTENT_TYPE = "application/x-www-form-urlencoded";
        private string contentType;

        /// <summary>
        /// Http-запрос
        /// </summary>
        /// <param name="endpoint">Путь к методу API</param>
        /// <param name="method">Http-метод</param>
        /// <param name="isJson">Выбор ContentType запроса</param>
        public Request(string endpoint, HttpApiMethod method = HttpApiMethod.GET, bool isJson = true)
        {
            Endpoint = endpoint;
            Method = method;
            QueryParams = new Dictionary<string, string>();

            IsJson = isJson;
            contentType = isJson ? APPLICATION_JSON_CONTENT_TYPE : X_WWW_FORM_URLENCODED_CONTENT_TYPE;
            Headers = new Dictionary<string, string>()
            {
                { "Content-Type", contentType }
            };
        }
        
        /// <summary>
        /// True, если ContentType="application/json"
        /// </summary>
        internal bool IsJson { get; }
        public bool IsSigned { get; set; }
        /// <summary>
        /// Путь запроса
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        /// Заголовки запроса
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// HTTP-метод
        /// </summary>
        public HttpApiMethod Method { get; set; }
        /// <summary>
        /// Параметры запроса
        /// </summary>
        public Dictionary<string, string> QueryParams { get; set; }

        /// <summary>
        /// Возвращает строку запроса
        /// </summary>
        /// <returns>Строка запроса в формате "param1=value1&amp;param2=value2"</returns>
        public string GetQueryString()
        {
            StringBuilder queryStringBuilder = new StringBuilder();

            foreach (KeyValuePair<string, string> kvp in QueryParams)
            {
                queryStringBuilder.Append($"{kvp.Key}={kvp.Value}&");
            }
            if (queryStringBuilder.Length > 0)
            {
                queryStringBuilder.Remove(queryStringBuilder.Length - 1, 1);
            }

            return queryStringBuilder.ToString();
        }

        /// <summary>
        /// Возвращает тело запроса
        /// </summary>
        /// <returns>Строка в формате "{"param1":"value1", "param2":"value2"}"</returns>
        public string GetJsonBody()
        {
            return JsonConvert.SerializeObject(QueryParams);
        }
    }
}
