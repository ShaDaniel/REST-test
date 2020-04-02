using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace REST_test
{
    [Binding]
    public class PetSteps
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
            var responseCode = requestutil.Request("get", requestutil.PetGetUri + id.ToString()).Result.StatusCode.ToString();

            Assert.AreEqual("OK", responseCode, "Созданный заказ на животное не найден");
        }
        [Given(@"delete existing pet twice")]
        public void GivenDeletePetAndCheck()
        {
            var id = GivenCreatePetWithNameAndPhotourls("test", "test");
            var requestutil = new GeneralHttpRequest();

            // Удаляем созданное животное первый раз, ждем положительный результат
            var response = requestutil.Request("delete", requestutil.PetGetUri + id.ToString());
            response.Wait();
            Assert.AreEqual("OK", response.Result.StatusCode.ToString(), "Удаление произошло с ошибкой");

            // Удаляем то же животное второй раз, ждем ошибку
            response = requestutil.Request("delete", requestutil.PetGetUri + id.ToString());
            response.Wait();
            Assert.AreEqual("NotFound", response.Result.StatusCode.ToString(), "Удаление произошло два раза");
        }
        [Given(@"get pet by status ""(.*)""")]
        public void GivenGetPetByStatus(string status)
        {
            // Возможные статусы животного
            List<string> legalStatuses = new List<string>() { "available", "pending", "sold" };
            var requestUtil = new GeneralHttpRequest();

            var response = requestUtil.Request("get", requestUtil.PetGetUri + status);
            response.Wait();
            // Входит ли статус в список допустимых
            var expectedCode = legalStatuses.Contains(status) ? "OK" : "BadRequest";
            Assert.AreEqual(expectedCode, response.Result.StatusCode.ToString(), "Возвращен неверный код ответа");
        }
        [Given(@"create pet with name ""(.*)"" and photourls ""(.*)""")]
        public long GivenCreatePetWithNameAndPhotourls(string name, string photourl)
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
            return id;
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
