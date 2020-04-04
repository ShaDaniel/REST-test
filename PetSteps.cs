using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;
using FluentAssertions;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace REST_test
{
    public static class Browser
    {
        public static IWebDriver ChromeDriver;

        public static ChromeOptions Options;
    }

    [Binding]
    public class PetSteps
    {
        [Given(@"change pet info and ensure it is changed")]
        public void GivenChangePetInfoAndEnsureItSChanged()
        {
            var requestutil = new GeneralHttpRequest();
            var jsonClass = requestutil.JsonPetInfo;
            var json = JsonConvert.SerializeObject(jsonClass);

            var response = requestutil.Request("put", requestutil.PetCreateUri, json);
            response.Wait();

            // Проверяем, что обновление произошло успешно
            Assert.AreEqual(200, (int)response.Result.StatusCode, "Возвращен неверный код ответа");

            // Проверяем в базе, что данные перезаписались
            var responseJson = response.Result.Content.ReadAsStringAsync().Result;
            var id = Convert.ToInt64(JObject.Parse(responseJson)["id"]);
            var message = requestutil.Request("get", requestutil.PetGetUri + id.ToString());
            message.Wait();
            var responseCode = message.Result.StatusCode;

            // Проверяем код ответа
            Assert.AreEqual(200, (int)responseCode, "Информация по данному животному не может быть получена");

            // Проверяем, что посланное на обновление сообщение совпадает с сообщением ответа из сервиса
            var answerJsonClass = JsonConvert.DeserializeObject<GeneralHttpRequest.PetInfo>(message.Result.Content.ReadAsStringAsync().Result);
            answerJsonClass.Should().BeEquivalentTo(jsonClass, options => options.WithoutStrictOrdering());
            //FLUENTASSERTIONS
        }
        [Given(@"create order for a pet and check in base")]
        public long GivenCreateOrderForAPetAndCheckInBase()
        {
            var requestutil = new GeneralHttpRequest();
            var jsonClass = requestutil.JsonOrderPet;
            var json = JsonConvert.SerializeObject(jsonClass);

            // Отправляем запрос на создание заказа и ждем ответа
            var response = requestutil.Request("post", requestutil.PetOrderCreateUri, json);
            response.Wait();

            // Проверяем, что код ответа 200
            Assert.AreEqual(200, (int)response.Result.StatusCode, "Возвращен неверный код ответа");

            // Проверяем, что пет был занесен в базу и мы можем его извлечь
            // Извлекаем Id для запроса
            var responseJson = response.Result.Content.ReadAsStringAsync().Result;
            var id = Convert.ToInt64(JObject.Parse(responseJson)["id"]);

            // Непосредственно проверяем по id
            var message = requestutil.Request("get", requestutil.PetGetUri + id.ToString());
            var responseCode = message.Result.StatusCode;
            
            // Проверяем, что код ответа 200 на проверку наличия пета
            Assert.AreEqual(200, (int)responseCode, "Созданный заказ на животное не найден");

            // Проверяем, что пришли все поля как и при создании пета
            var answerJsonClass = JsonConvert.DeserializeObject<GeneralHttpRequest.OrderPet>(message.Result.Content.ReadAsStringAsync().Result);
            answerJsonClass.Should().BeEquivalentTo(jsonClass, options => options.WithoutStrictOrdering());

            return id;
        }
        [Given(@"delete existing pet twice")]
        public void GivenDeletePetAndCheck()
        {
            var id = GivenCreatePetWithNameAndPhotourls("test", "test");
            var requestutil = new GeneralHttpRequest();

            // Удаляем созданное животное первый раз, ждем положительный результат
            var response = requestutil.Request("delete", requestutil.PetGetUri + id.ToString());
            response.Wait();
            Assert.AreEqual(200, (int)response.Result.StatusCode, "Удаление произошло с ошибкой");

            // Убеждаемся, что пета больше нет в базе
            response = requestutil.Request("get", requestutil.PetGetUri + id.ToString());
            response.Wait();
            Assert.AreEqual(404, (int)response.Result.StatusCode, "Удаленный питомец найден через сервис");

            // Удаляем то же животное второй раз, ждем ошибку
            response = requestutil.Request("delete", requestutil.PetGetUri + id.ToString());
            response.Wait();
            Assert.AreEqual(404, (int)response.Result.StatusCode, "Удаление произошло два раза");
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
            var expectedCode = legalStatuses.Contains(status) ? 200 : 400;
            Assert.AreEqual(expectedCode, (int)response.Result.StatusCode, "Возвращен неверный код ответа");
        }
        [Given(@"create pet with name ""(.*)"" and photourls ""(.*)""")]
        public long GivenCreatePetWithNameAndPhotourls(string name, string photourl)
        {
            // Считаем пустые строки за NULL - отсутствие аргумента
            name = name == "" ? null : name;
            photourl = photourl == "" ? null : photourl;

            // Вставляем аргументы в json
            var requestutil = new GeneralHttpRequest();
            var jsonClass = requestutil.JsonPetInfo;
            jsonClass.Name = name;
            jsonClass.PhotoUrls.Add(photourl);

            var json = JsonConvert.SerializeObject(jsonClass);

            var response = requestutil.Request("post", requestutil.PetCreateUri, json);
            response.Wait();

            // Если хотя бы одно из обязательных полей отсутствует
            var expectedCode = name != null && photourl != null ? 200 : 405;
            Assert.AreEqual(expectedCode, (int)response.Result.StatusCode, "Возвращен неверный код ответа");

            // Если успешно создано выше - проверка получения пета по id
            var message = response.Result.Content.ReadAsStringAsync().Result;
            var id = Convert.ToInt64(JObject.Parse(message)["id"]);

            // Проверяем код ответа на запрос по получению пета
            var responseGet = requestutil.Request("get", requestutil.PetGetUri + id.ToString());
            var responseGetCode = responseGet.Result.StatusCode;
            Assert.AreEqual(200, (int)responseGetCode, "Новосозданное животное недоступно по ид");

            // Проверяем, что полученный json соответствует тому, что мы отправляли
            var responseGetClass = JsonConvert.DeserializeObject<GeneralHttpRequest.PetInfo>(responseGet.Result.Content.ReadAsStringAsync().Result);
            responseGetClass.Should().BeEquivalentTo(jsonClass, options => options.WithoutStrictOrdering());

            return id;
        }
        [Given(@"send empty json body")]
        public void GivenSendEmptyJsonBody()
        {
            var requestutil = new GeneralHttpRequest();

            var response = requestutil.Request("post", requestutil.PetCreateUri, "{}");
            response.Wait();

            Assert.AreEqual(405, (int)response.Result.StatusCode, "Возвращен неверный код ответа");
        }
        [Given(@"delete pet order")]
        public void GivenDeletePetOrder()
        {
            var id = GivenCreateOrderForAPetAndCheckInBase();
            var requestutils = new GeneralHttpRequest();

            // Удаляем животное по id
            var response = requestutils.Request("delete", requestutils.PetGetOrderUri + id.ToString());
            response.Wait();

            // Проверка, что удаление успешно (по коду)
            Assert.AreEqual(200, (int)response.Result.StatusCode, "Удаление заказа провалилось");

            // Проверка, что удаление успешно (по запросу заказа по id)
            response = requestutils.Request("get", requestutils.PetGetOrderUri + id.ToString());
            response.Wait();
            Assert.AreEqual(404, (int)response.Result.StatusCode, "Заказ на животное найден после удаления");

            // Пытаемся удалить тот же заказ еще раз
            response = requestutils.Request("delete", requestutils.PetGetOrderUri + id.ToString());
            response.Wait();
            Assert.AreEqual(404, (int)response.Result.StatusCode, "Удаление заказа произошло 2 раза");
        }

        [Given(@"open Chrome")]
        public void GivenOpenChrome()
        {
            Browser.Options = new ChromeOptions();
            Browser.Options.BinaryLocation = Environment.CurrentDirectory + @"\..\..\..\GoogleChromePortable\GoogleChromePortable.exe";
            Browser.ChromeDriver = new ChromeDriver(Environment.CurrentDirectory + @"\..\..\..\GoogleChromePortable");
            Browser.ChromeDriver.Manage().Window.Maximize();
            Browser.ChromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        }

        [Given(@"go to ""(.*)""")]
        public void GivenGoTo(string url)
        {
            Browser.ChromeDriver.Navigate().GoToUrl(url);
        }

        [Given(@"ensure search bar visible")]
        public void GivenEnsureSearchBarVisible()
        {
            try
            {
                Browser.ChromeDriver.FindElement(By.XPath("//input[@name = 'text']"));
            }
            catch (NoSuchElementException)
            {
                throw new NoSuchElementException("Строка поиска не найдена");
            }
        }

    }
}
