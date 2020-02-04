using System;
using TechTalk.SpecFlow;
using Example;
using NUnit.Framework;

namespace REST_test
{
    [Binding]
    public class CalculatorSteps
    {
        private int result;
        Calculator calc = new Calculator();
        [Given(@"I have entered (.*) into the calculator")]
        public void GivenIHaveEnteredIntoTheCalculator(int p0)
        {
            calc.FirstNum = p0;
        }
        
        [Given(@"I have also entered (.*) into the calculator")]
        public void GivenIHaveAlsoEnteredIntoTheCalculator(int p0)
        {
            calc.SecondNum = p0;
        }
        
        [When(@"I press add")]
        public void WhenIPressAdd()
        {
            result = calc.Add();
        }
        
        [Then(@"the result should be (.*) on the screen")]
        public void ThenTheResultShouldBeOnTheScreen(int p0)
        {
            Assert.AreEqual(p0, result);
        }
    }
}
