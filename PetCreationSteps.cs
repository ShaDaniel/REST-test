using NUnit.Framework;
using System;
using TechTalk.SpecFlow;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace REST_test
{
    [Binding]
    public class PetCreationSteps
    {
        [Given(@"create pet with name ""(.*)"" and photourls ""(.*)""")]
        public void GivenCreatePetWithNameAndPhotourls(string name, string photourl)
        {
            // Считаем пустые строки за NULL - отсутствие аргумента
            name = name == "" ? null : name;
            photourl = photourl == "" ? null : photourl;

            // Параметризированный json для изменения всех обязательных полей
            var json = $"{{ \"id\": 0, \"category\": {{ \"id\": 0, \"name\": \"{name}\" }}, \"name\": \"string\", \"photoUrls\": [ \"{photourl}\" ], \"tags\": [ {{ \"id\": 0, \"name\": \"string\" }} ], \"status\": \"available\"}}";
            //var json = "{ \"id\": 0, \"category\": { \"id\": 0, \"name\": \"string\" }, \"name\": \"@name\", \"photoUrls\": [ \"string\" ], \"tags\": [ { \"id\": 0, \"name\": \"string\" } ], \"status\": \"available\"}";
            var requestutil = new GeneralHttpRequest();

            var response = requestutil.Request("post", requestutil.PetCreateUri, json);
            response.Wait();

            // Если хотя бы одно из обязательных полей отсутствует
            var expectedCode = name != null && photourl != null ? "OK" : "MethodNotAllowed";
            Assert.AreEqual(expectedCode, response.Result.StatusCode.ToString(), "Возвращен неверный код ответа");

            // Если успешно создано выше - проверка получения пета по id
            var message = response.Result.Content.ReadAsStringAsync().Result;
            var id = Convert.ToInt64(JObject.Parse(message)["id"]);
            // Проверяем код ответа на запрос по получению пета
            var responseCode = requestutil.Request("get", requestutil.PetGetUri + id.ToString()).Result.StatusCode.ToString();
            Assert.AreEqual("OK", responseCode, "Новосозданное животное недоступно по ид");
        }
        [Given(@"send empty json body")]
        public void GivenSendEmptyJsonBody()
        {
            var requestutil = new GeneralHttpRequest();

            var responseCode = requestutil.Request("post", requestutil.PetCreateUri, "{}");
            responseCode.Wait();

            Assert.AreEqual("MethodNotAllowed", responseCode.Result.ToString(), "Возвращен неверный код ответа");
        }
    }
}
