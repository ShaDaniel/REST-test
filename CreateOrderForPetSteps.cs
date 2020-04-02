using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using TechTalk.SpecFlow;

namespace REST_test
{
    [Binding]
    public class CreateOrderForPetSteps
    {
        [Given(@"create order for a pet and check in base")]
        public void GivenCreateOrderForAPetAndCheckInBase()
        {
            var json = "{ \"id\": 0, \"petId\": 0, \"quantity\": 0, \"shipDate\": \"2020-04-01T23:35:59.495Z\", \"status\": \"placed\", \"complete\": true}";
            var requestutil = new GeneralHttpRequest();

            // Отправляем запрос на создание заказа и ждем ответа
            var response = requestutil.Request("post", requestutil.PetOrderCreateUri, json);
            response.Wait();

            // Проверяем, что код ответа 200
            Assert.AreEqual("OK", response.Result.StatusCode.ToString(), "Возвращен неверный код ответа");

            // Проверяем, что пет был занесен в базу и мы можем его извлечь
            var message = response.Result.Content.ReadAsStringAsync().Result;
            var id = Convert.ToInt64(JObject.Parse(message)["petId"]);
            var responseCode = requestutil.Request("get", String.Format(requestutil.PetGetOrderUri, id)).Result.StatusCode.ToString();

            Assert.AreEqual("OK", responseCode, "Созданный заказ на животное не найден");
        }
    }
}
