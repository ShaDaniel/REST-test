using System;
using TechTalk.SpecFlow;

namespace REST_test
{
    [Binding]
    public class DeletePetSteps
    {
        [Given(@"delete existing pet 2 times")]
        public void GivenDeletePetAndCheck()
        {
            var id = GivenCreatePetWithNameAndPhotourls("test", "test");
            var requestutil = new GeneralHttpRequest();
        }
    }
}
