using System;
using TechTalk.SpecFlow;

namespace REST_test
{
    [Binding]
    public class ChangePetInfoSteps
    {
        [Given(@"change pet info and ensure it's changed")]
        public void GivenChangePetInfoAndEnsureItSChanged()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
