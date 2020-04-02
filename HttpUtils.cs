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
        /// <summary> URI получения животных по статусу </summary>
        public string PetStatusUri { get; set; } = $"https://petstore.swagger.io/v2/pet/findByStatus?status=";
        /// <summary> URI добавления животного </summary>
        public string PetCreateUri { get; } = "https://petstore.swagger.io/v2/pet";
        /// <summary>  Получение животного по ID </summary>
        public string PetGetUri { get; set; } = $"https://petstore.swagger.io/v2/pet/";
        /// <summary> Создание заказа на животное </summary>
        public string PetOrderCreateUri { get; } = "https://petstore.swagger.io/v2/store/order";
        /// <summary> Получение заказа на животное по ID заказа </summary>
        public string PetGetOrderUri { get; set; } = $"https://petstore.swagger.io/v2/store/order/";


        /// <summary> Метод работы с веб-запросами в общем виде </summary>
        /// <param name="method">Метод запроса</param>
        /// <param name="url">URI адрес запроса</param>
        /// <param name="json">Опциональный json для POST запроса</param>
        /// <returns> Сообщение с ответом </returns>
        public async Task<HttpResponseMessage> Request(string method, string url, string json = "")
        {
            using var client = new HttpClient();

            var response = method switch
            {
                ("get") => await client.GetAsync(url),
                ("post") => await client.PostAsJsonAsync(url, new StringContent(json, Encoding.UTF8, "application/json")),
                ("put") => await client.PutAsJsonAsync(url, new StringContent(json, Encoding.UTF8, "application/json")),
                ("delete") => await client.DeleteAsync(url),
                _ => throw new Exception("Указан неверный тип запроса"),
            };
            return response;
        }
    }
}
