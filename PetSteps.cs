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
            var jsonClass = requestutil.JsonPetInfo; //PETINFO
            jsonClass.Id = (long)ScenarioContext.Current["CreatedPetId"];
            jsonClass.Name = "winnie-the-pooh";
            var json = JsonConvert.SerializeObject(jsonClass);

            var response = requestutil.Request("put", requestutil.PetCreateUri, json);
            response.Wait();

            // Заносим код ответа и json обновления инфы
            ScenarioContext.Current["CodeResponse"] = (int)response.Result.StatusCode;
            ScenarioContext.Current["UpdateMessage"] = jsonClass;
        }

        [Given(@"ensure pet info was changed")]
        public void GivenEnsurePetInfoWasChanged()
        {
            var requestutil = new GeneralHttpRequest();
            // Проверяем в базе, что данные перезаписались
            var message = requestutil.Request("get", requestutil.PetGetUri + ScenarioContext.Current["CreatedPetId"].ToString());
            message.Wait();
            var responseCode = (int)message.Result.StatusCode;

            // Заносим код ответа и десериализованный message 
            ScenarioContext.Current["CodeResponse"] = responseCode;
            ScenarioContext.Current["UpdateMessageChecking"] = JsonConvert.DeserializeObject<GeneralHttpRequest.PetInfo>(message.Result.Content.ReadAsStringAsync().Result);
        }

        [Given(@"check if both messages match")]
        public void GivenCheckIfBothMessagesMatch()
        {
            // Проверяемое и образцовое сообщение
            var checking = ScenarioContext.Current["UpdateMessageChecking"];
            var criterion = ScenarioContext.Current["UpdateMessage"];
            // Проверяем, что посланное на обновление сообщение совпадает с сообщением ответа из сервиса
            checking.Should().BeEquivalentTo(criterion, options => options.WithoutStrictOrdering());
        }


        [Given(@"create order for a pet and check in base")]
        public void GivenCreateOrderForAPetAndCheckInBase()
        {
            var requestutil = new GeneralHttpRequest();
            var jsonClass = requestutil.JsonOrderPet; //ORDERPET
            var json = JsonConvert.SerializeObject(jsonClass);

            // Отправляем запрос на создание заказа и ждем ответа
            var response = requestutil.Request("post", requestutil.PetOrderCreateUri, json);
            response.Wait();

            // Извлекаем Id для запроса
            var responseJson = response.Result.Content.ReadAsStringAsync().Result;
            var id = Convert.ToInt64(JObject.Parse(responseJson)["id"]);

            // Заносим id заказа, код ответа и json обновления инфы
            ScenarioContext.Current["CreatedOrderId"] = id;
            ScenarioContext.Current["CodeResponse"] = (int)response.Result.StatusCode;
            ScenarioContext.Current["UpdateMessage"] = jsonClass;
        }

        [Given(@"ensure order creation is ok")]
        public void GivenEnsureOrderCreationIsOk()
        {
            var requestutils = new GeneralHttpRequest();
            // Проверка, что создание успешно (по запросу заказа по id)
            var response = requestutils.Request("get", requestutils.PetGetOrderUri + ScenarioContext.Current["CreatedOrderId"].ToString());
            response.Wait();
            Assert.AreEqual(200, (int)response.Result.StatusCode, "Ошибка в получении заказа на животное");
            // Заносим десериализованное сообщение для дальнейшей проверки
            ScenarioContext.Current["UpdateMessageChecking"] = JsonConvert.DeserializeObject<GeneralHttpRequest.PetInfo>(response.Result.Content.ReadAsStringAsync().Result);
        }

        [Given(@"delete pet")]
        public void GivenDeletePetAndCheck()
        {
            var id = ScenarioContext.Current["CreatedPetId"];
            //var id = GivenCreatePetWithNameAndPhotourls("test", "test");
            var requestutil = new GeneralHttpRequest();

            // Удаляем созданное животное
            var response = requestutil.Request("delete", requestutil.PetGetUri + id.ToString());
            response.Wait();

            // Записываем код ответа
            ScenarioContext.Current["CodeResponse"] = (int)response.Result.StatusCode;


            // Удаляем то же животное второй раз, ждем ошибку
            response = requestutil.Request("delete", requestutil.PetGetUri + id.ToString());
            response.Wait();
            Assert.AreEqual(404, (int)response.Result.StatusCode, "Удаление произошло два раза");
        }

        [Given(@"ensure pet is gone")]
        public void GivenEnsurePetIsGone()
        {
            var requestutil = new GeneralHttpRequest();
            // Убеждаемся, что пета больше нет в базе
            var response = requestutil.Request("get", requestutil.PetGetUri + ScenarioContext.Current["CreatedPetId"].ToString());
            response.Wait();
            Assert.AreEqual(404, (int)response.Result.StatusCode, "Удаленный питомец найден через сервис");
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
        public void GivenCreatePetWithNameAndPhotourls(string name, string photourl)
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

            // Присваиваем полученную созданную id
            jsonClass.Id = id;

            // Проверяем код ответа на запрос по получению пета
            var responseGet = requestutil.Request("get", requestutil.PetGetUri + id.ToString());
            var responseGetCode = responseGet.Result.StatusCode;
            Assert.AreEqual(200, (int)responseGetCode, "Новосозданное животное недоступно по ид");

            // Проверяем, что полученный json соответствует тому, что мы отправляли
            var responseGetClass = JsonConvert.DeserializeObject<GeneralHttpRequest.PetInfo>(responseGet.Result.Content.ReadAsStringAsync().Result);
            responseGetClass.Should().BeEquivalentTo(jsonClass, options => options.WithoutStrictOrdering());

            ScenarioContext.Current["CreatedPetId"] = id;
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
            var id = ScenarioContext.Current["CreatedOrderId"];
            var requestutils = new GeneralHttpRequest();

            // Удаляем животное по id
            var response = requestutils.Request("delete", requestutils.PetGetOrderUri + id.ToString());
            response.Wait();

            // Заношу код ответа
            ScenarioContext.Current["CodeResponse"] = (int)response.Result.StatusCode;
        }

        [Given(@"ensure code is (.*)")]
        public void GivenEnsureCodeIs(int code)
        {
            // Проверка, что код возврата соответствует
            Assert.AreEqual(code, (int)ScenarioContext.Current["CodeResponse"], "Код ответа не соответствует ожидаемому");
        }

        [Given(@"ensure that deletion order is ok")]
        public void GivenEnsureThatDeletionOrderIsOk()
        {
            var requestutils = new GeneralHttpRequest();
            // Проверка, что удаление успешно (по запросу заказа по id)
            var response = requestutils.Request("get", requestutils.PetGetOrderUri + ScenarioContext.Current["CreatedOrderId"].ToString());
            response.Wait();
            Assert.AreEqual(404, (int)response.Result.StatusCode, "Заказ на животное найден после удаления");
        }


        [Given(@"open Chrome")]
        public void GivenOpenChrome()
        {
            Browser.Options = new ChromeOptions();
            Browser.Options.BinaryLocation = Environment.CurrentDirectory + @"\..\..\..\GoogleChromePortable\GoogleChromePortable.exe";
            Browser.ChromeDriver = new ChromeDriver(Environment.CurrentDirectory + @"\..\..\..\GoogleChromePortable");
            Browser.ChromeDriver.Manage().Window.Maximize();
            Browser.ChromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
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

        [Given(@"clear all base with pet IDs")]
        public void GivenClearAllBaseWithPetIDs()
        {
            var requestutils = new GeneralHttpRequest();

            // Удаляем животное по id
            for (int i = 200; i < 10000; i++)
            {
                var response = requestutils.Request("delete", requestutils.PetGetOrderUri + (1845563262948988700 + i).ToString());
                response.Wait();
            }
        }

    }
}
