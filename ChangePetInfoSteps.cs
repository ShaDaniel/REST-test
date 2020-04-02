using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using TechTalk.SpecFlow;

namespace REST_test
{
    [Binding]
    public class ChangePetInfoSteps
    {
        [Given(@"change pet info and ensure it is changed")]
        public void GivenChangePetInfoAndEnsureItSChanged()
        {
            var json = "{ \"id\": 0, \"category\": { \"id\": 0, \"name\": \"string\" }, \"name\": \"doggie\", \"photoUrls\": [ \"string\" ], \"tags\": [ { \"id\": 0, \"name\": \"string\" } ], \"status\": \"available\"}";
            var requestutil = new GeneralHttpRequest();

            var response = requestutil.Request("put", requestutil.PetCreateUri, json);
            response.Wait();

            // Проверяем, что обновление произошло успешно
            Assert.AreEqual("OK", response.Result.StatusCode.ToString(), "Возвращен неверный код ответа");

            // Проверяем в базе, что данные перезаписались
            var expectedMessage = response.Result.Content.ReadAsStringAsync().Result;
            var id = Convert.ToInt64(JObject.Parse(expectedMessage)["id"]);
            var message = requestutil.Request("get", requestutil.PetGetUri + id.ToString());
            message.Wait();
            var responseCode = message.Result.StatusCode.ToString();

            // Проверяем код ответа
            Assert.AreEqual("OK", responseCode, "Информация по данному животному не может быть получена");
            // Проверяем, что посланное на обновление сообщение совпадает с сообщением ответа из сервиса
            Assert.AreEqual(expectedMessage, message.Result.Content.ReadAsStringAsync().Result, "Обновление информации по животному прошло некорректно");
        }
    }
}
