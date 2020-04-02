using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using System.Net.Http;
using NUnit.Framework;
using MySql.Data.MySqlClient;
using Allure.Commons;

namespace REST_test
{
    [Binding]
    public class GetPetByStatusSteps
    {
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
    }
}
