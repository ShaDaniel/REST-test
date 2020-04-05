using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using MySql.Data.MySqlClient;
using Allure.Commons;
using OpenQA.Selenium;

namespace REST_test
{
    [Binding]
    public sealed class Hooks
    {
        [AfterScenario("gui")]
        public void AfterScenario()
        {
            if (ScenarioContext.Current.TestError != null)
            {
                var shot = ((ITakesScreenshot)Browser.ChromeDriver).GetScreenshot().AsByteArray;
                AllureLifecycle.Instance.AddAttachment("GUI error", "image/png", shot);
            }
            Browser.ChromeDriver?.Quit();
        }

        [AfterStep]
        public void AfterStep()
        {
            var attachment = $@"{DateTime.Now}: {ScenarioContext.Current.ScenarioInfo.Title} {ScenarioContext.Current.StepContext.StepInfo.Text} {(ScenarioContext.Current.TestError != null ? "Error" : "OK")}";
            using (var connection = DBUtils.DBConnection())
            {
                if (System.Environment.GetEnvironmentVariable("DbLogEnable") == "true")
                {
                    connection.Open();
                    var sql = $@"INSERT INTO logs VALUES (NOW(), '{ScenarioContext.Current.StepContext.StepInfo.Text}',
                            '{ScenarioContext.Current.ScenarioInfo.Title}', {(ScenarioContext.Current.TestError != null ? 0 : 1)}, '{ScenarioContext.Current.TestError}')";
                    new MySqlCommand(sql, connection).ExecuteNonQuery();
                } 
                //JENKINS
            };
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine(attachment);
            AllureLifecycle.Instance.AddAttachment("Step Result", "text/plain", Encoding.ASCII.GetBytes(attachment));
        }
    }
}
