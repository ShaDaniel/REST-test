using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using System.Net.Http;
using NUnit.Framework;
using MySql.Data.MySqlClient;

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

            var responseCode = requestUtil.Request("get", String.Format(requestUtil.PetStatusUri, status));
            responseCode.Wait();
            // Входит ли статус в список допустимых
            var expectedCode = legalStatuses.Contains(status) ? "OK" : "BadRequest";
            Assert.AreEqual(expectedCode, responseCode.Result.ToString(), "Возвращен неверный код ответа");
        }
    }
}
