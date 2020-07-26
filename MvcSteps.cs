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
    [Binding]
    public class MvcSteps
    {
        [Given(@"add pass with random person")]
        public void GivenAddPassWithRandomPerson()
        {
            var pass = new MvcPass();

            var response = MvcBaseUtils.Request("post", MvcBaseUtils.Pass, MvcPass.ToJson(pass));

            ScenarioContext.Current["guid"] = MvcBaseUtils.Deserialize<string>(response);
            ScenarioContext.Current["pass"] = pass;
            ScenarioContext.Current["lastCode"] = (int)response.Result.StatusCode;
        }

        [Given(@"get by guid and assert return code is (.*)")]
        public void GivenGetByGuidAndAssertReturnCodeIs(int expectedCode)
        {
            var guid = (string)ScenarioContext.Current["guid"];

            var response = MvcBaseUtils.Request("get", MvcBaseUtils.Pass + guid);
            Assert.AreEqual(expectedCode, (int)response.Result.StatusCode, "Только что созданный пропуск не найден");

            MvcPass passAct = MvcBaseUtils.Deserialize<MvcPass>(response);
            if (ScenarioContext.Current.ContainsKey("pass"))
            {
                MvcPass passExp = (MvcPass)ScenarioContext.Current["pass"];
                passAct.Should().BeEquivalentTo(passExp, options => options.WithoutStrictOrdering().Excluding(o => o.Guid));
            }
        }

        [Given(@"delete the pass by guid")]
        public void GivenDeleteThePassByGuid()
        {
            var guid = (string)ScenarioContext.Current["guid"];

            var response = MvcBaseUtils.Request("delete", MvcBaseUtils.Pass + guid);
            ScenarioContext.Current["lastCode"] = (int)response.Result.StatusCode;
        }

        [Given(@"generate random guid")]
        public void GivenGenerateRandomGuid()
        {
            ScenarioContext.Current["guid"] = System.Guid.NewGuid().ToString();
        }

        [Given(@"assert code is (.*)")]
        public void GivenAssertCodeIs(int expectedCode)
        {
            var actualCode = (int)ScenarioContext.Current["lastCode"];
            Assert.AreEqual(expectedCode, actualCode, @"Фактический код ответа @actualCode не совпадает с ожидаемым @expectedCode");
        }

        [Given(@"update pass with random info")]
        public void GivenUpdatePassWithRandomInfo()
        {
            var newPass = new MvcPass
            {
                Guid = (string)ScenarioContext.Current["guid"]
            };

            var response = MvcBaseUtils.Request("put", MvcBaseUtils.Pass, MvcPass.ToJson(newPass));

            ScenarioContext.Current["pass"] = newPass;
            ScenarioContext.Current["lastCode"] = (int)response.Result.StatusCode;
        }

        [Given(@"get the pass status")]
        public void GivenGetThePassStatus()
        {
            var guid = (string)ScenarioContext.Current["guid"];

            var response = MvcBaseUtils.Request("get", MvcBaseUtils.PassValidate + guid);

            ScenarioContext.Current["lastCode"] = (int)response.Result.StatusCode;
        }

    }
}
