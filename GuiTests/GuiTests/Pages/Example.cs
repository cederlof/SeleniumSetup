using GuiTests.Attributes;
using GuiTests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace GuiTests.Pages
{
    [PageUrl("Home/Index")]
    public class Example : PageBase
    {
        private readonly IWebDriver _driver;

        public Example(IWebDriver driver)
            : base(driver)
        {
            _driver = driver;
        }

        internal void AssertTextExists(string expectedText)
        {
            _driver.WaitForPageToLoad(By.Id("StatusText"));
            var element = _driver.FindElement(By.Id("StatusText"));
            Assert.IsTrue(expectedText == element.Text);
        }
    }
}