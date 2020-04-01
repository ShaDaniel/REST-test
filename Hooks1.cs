using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using MySql.Data.MySqlClient;

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
        public void AfterScenario()
        {
            using (var connection = DBUtils.DBConnection())
            {
                //var exception = ScenarioContext.Current[""]
                //if (System.Environment.GetEnvironmentVariable("Log") == "Yes")
                //{
                    connection.Open();

                    new MySqlCommand($"INSERT INTO logs VALUES (NOW(), '{ScenarioContext.Current.StepContext.StepInfo.Text}'," +
                        $" '{ScenarioContext.Current.ScenarioInfo.Title}', -44, '{ScenarioContext.Current.TestError}')", connection).ExecuteNonQuery();
                //} 
                //JENKINS
            };
        }
    }
}
