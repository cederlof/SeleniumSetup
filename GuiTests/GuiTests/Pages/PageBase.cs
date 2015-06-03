using System;
using GuiTests.Attributes;
using GuiTests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace GuiTests.Pages
{
    /// <summary>
    /// A base page providing some standard assertions and easy functionality to go to a web page
    /// </summary>
    [PageUrl("http://localhost:8080/")]
    public abstract class PageBase
    {
        private readonly IWebDriver _driver;

        protected PageBase(IWebDriver driver)
        {
            _driver = driver;
        }

        protected void AssertElementHasSubElement(IWebElement element, By by)
        {
            _driver.WaitForPageToLoad(by, element);
            Assert.IsTrue(element.ElementIsPresent(_driver, by));
        }

        protected void AssertElementHasSpecificNumberOfSubElements(IWebElement element, By by, int allowedCount)
        {
            _driver.WaitForPageToLoad(by, element);
            var actualCount = element.FindElements(by).Count;
            Assert.AreEqual(allowedCount, actualCount, String.Format("Expected '{0}' elements (using selector '{2}') but there were '{1}'.", allowedCount, actualCount, by));
        }

        public virtual void AssertAllInformationIsLoaded()
        {
            var startCheck = DateTime.Now;
            while (!_driver.ElementIsPresent(By.Id("lazyload-complete"), 1))
            {
                var diff = DateTime.Now - startCheck;
                if (diff > TimeSpan.FromSeconds(20))
                {
                    break;
                }
                _driver.WaitForElementToDisappear(By.ClassName("loading"));
                _driver.ScrollToBottom();
            }
            Assert.IsTrue(_driver.ElementIsPresent(By.Id("lazyload-complete")));
        }
    }
}