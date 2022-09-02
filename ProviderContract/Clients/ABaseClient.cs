using ProviderContract.Data;
using ProviderContract.Services;
using RestSharp;
using System;

namespace ProviderContract.Clients
{
    /// <summary>
    /// Базовый клиент для выполнения запросов, преобразования запросов и названий инструментов
    /// </summary>
    public abstract class ABaseClient : IBaseClient
    {
        private readonly RestClient restClient;
        protected KeysData keys;
        protected MarketTypeConfiguration config;

        /// <summary>
        //  Базовый клиент для обработки и выпонения запросов
        /// </summary>
        /// <param name="configuration">Конфигурация </param>
        /// <param name="keysData">API-ключи</param>
        public ABaseClient(MarketTypeConfiguration configuration, KeysData keysData = null) 
        { 
            config = configuration;
            keys = keysData;
            restClient = new RestClient(new Uri(config.BaseURL));
        }
        /// <summary>
        /// Статус авторизации (наличие ключей)
        /// </summary>
        public bool IsAuthorized => keys != null;

        /// <summary>
        /// Создание подписи и выполнение остальных требований приватного запроса
        /// </summary>
        /// <param name="request">Запрос</param>
        public virtual void DoSign(ref Request request) { }

        /// <summary>
        /// Выполнение запроса на API-сервер
        /// </summary>
        /// <typeparam name="T">Модель ответа сервера</typeparam>
        /// <param name="request">Запрос</param>
        /// <returns>Ответ сервера с данными, статусом запроса и текстом ошибки (если есть)</returns>
        public Response<T> SendRequest<T>(Request request)
        {
            Response<T> response = new Response<T>();

            Method method = (Method)request.Method;
            RestRequest requestRest = new RestRequest(request.Endpoint, method);

            // Если запрос требует подпись, то добавляем ее
            if (request.IsSigned)
            {
                DoSign(ref request);
            }

            // Определяем тип контента JSON или WWW-FORM-URLENCODED
            // Добавляем строку параметров или тело запроса соответственно
            if (request.Method == Data.Enums.HttpApiMethod.POST && request.IsJson)
            { 
                requestRest.AddJsonBody(request.QueryParams);
            }
            else
            {
                foreach (var param in request.QueryParams)
                {
                    requestRest.AddParameter(param.Key, param.Value.ToString());
                }
            }

            // Добавляем заголовки
            foreach (var header in request.Headers)
            {
                requestRest.AddHeader(header.Key, header.Value.ToString());
            }

            IRestResponse responseRest = default;

            // Получаем ответ сервера
            try
            {
                responseRest = restClient.Execute(requestRest);
            }
            catch
            {
                response.HasResponseError = true;
                response.Error = "Failed to execute request";
            }

            response.RawJson = responseRest.Content;

            // Проверяем наличие ошибки запроса
            if (!responseRest.IsSuccessful)
            {
                response.HasResponseError = true;
                response.Error = $"Request execution error - {responseRest.StatusCode}";
                return response;
            }
            // Проверяем, что содержимое ответа не пустое
            if (string.IsNullOrEmpty(responseRest.Content) && responseRest.Content != "[]")
            {
                response.HasResponseError = true;
                response.Error = $"Empty response content";
                return response;
            }

            // Парсинг содержимого в объект
            try
            {
                response.Content = ParcerService.StringJsonToObject<T>(responseRest.Content);
            } 
            catch (Exception ex)
            {
                response.HasResponseError = true;
                response.Error = $"Parce error: {ex.Message}; {response.RawJson}";
            }

            return response;
        }

        /// <summary>
        /// Преобразование названия инструмента из формата биржи в стандартный
        /// </summary>
        /// <param name="symbol">Инструмент в формате биржи</param>
        /// <returns>Инструмент в стандартном формате TOOL1-TOOL2</returns>
        public virtual string ToPair(string symbol)
        {
            return symbol;
        }

        /// <summary>
        /// Преобразование названия инструмента из стандартного формата в формат биржи 
        /// </summary>
        /// <param name="pair">Инструмент в стандартном формате TOOL1-TOOL2</param>
        /// <returns>Инструмент в формате биржи</returns>
        public virtual string ToSymbol(string pair)
        {
            return pair;
        }
    }
}
