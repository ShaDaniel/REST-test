using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace REST_test
{
    class GeneralHttpRequest
    {
        // URI получения животных по статусу
        public string PetStatusUri { get; set; } = $"https://petstore.swagger.io/v2/pet/findByStatus?status={0}";

        /// <summary>
        /// Метод работы с веб-запросами в общем виде
        /// </summary>
        /// <param name="method">Метод запроса</param>
        /// <param name="url">URI адрес запроса</param>
        /// <param name="json">Опциональный json для POST запроса</param>
        /// <returns>Код ответа</returns>
        public async Task<HttpStatusCode> Request(string method, string url, string json = "")
        {
            using var client = new HttpClient();

            var response = method switch
            {
                ("get") => await client.GetAsync(url),
                ("post") => await client.PostAsJsonAsync(url, json),
                _ => throw new Exception("Указан неверный тип запроса"),
            };
            return response.StatusCode;
        }
    }
}
