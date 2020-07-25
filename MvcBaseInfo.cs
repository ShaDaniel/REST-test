using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace REST_test
{
    public static class MvcBaseUtils
    {
        public static string Pass = "http://localhost:9999/pass/";
        public static string PassValidate = "http://localhost:9999/pass/validate/";
        public static GeneralHttpRequest Requests = new GeneralHttpRequest();

        /// <summary> Метод работы с веб-запросами в общем виде </summary>
        /// <param name="method">Метод запроса</param>
        /// <param name="url">URI адрес запроса</param>
        /// <param name="json">Опциональный json для POST запроса</param>
        /// <returns> Сообщение с ответом </returns>
        public static Task<HttpResponseMessage> Request(string method, string url, string json = "")
        {
            var response = Requests.Request(method, url, json);
            response.Wait();
            return response;
        }

        public static T Deserialize<T>(Task<HttpResponseMessage> response)
        {
            T res = JsonConvert.DeserializeObject<T>(response.Result.Content.ReadAsStringAsync().Result);

            return res;
        }
    }
}
