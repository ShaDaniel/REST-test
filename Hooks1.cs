using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using MySql.Data.MySqlClient;
using Allure.Commons;

namespace REST_test
{
    [Binding]
    public sealed class Hooks1
    {
        [BeforeStep]
        public void BeforeScenario()
        {
            //TODO: implement logic that has to run before executing each scenario
        }

        [AfterStep]
        public void AfterStep()
        {
            var attachment = $@"{DateTime.Now}: {ScenarioContext.Current.ScenarioInfo.Title} {ScenarioContext.Current.StepContext.StepInfo.Text} {(ScenarioContext.Current.TestError != null ? "Error" : "OK")}";
            using (var connection = DBUtils.DBConnection())
            {
                //var exception = ScenarioContext.Current[""]
                //if (System.Environment.GetEnvironmentVariable("Log") == "Yes")
                //{
                    connection.Open();

                    new MySqlCommand($"INSERT INTO logs VALUES (NOW(), '{ScenarioContext.Current.StepContext.StepInfo.Text}'," +
                        $" '{ScenarioContext.Current.ScenarioInfo.Title}', {(ScenarioContext.Current.TestError != null ? 0 : 1)}, '{ScenarioContext.Current.TestError}')", connection).ExecuteNonQuery();
                //} 
                //JENKINS
            };
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            AllureLifecycle.Instance.AddAttachment("Step Result", "text/plain", Encoding.ASCII.GetBytes(attachment));
        }
    }
}
