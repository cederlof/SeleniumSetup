using System;
using GuiTests.Extensions;
using GuiTests.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GuiTests.Tests
{
    [TestClass]
    public class Example : TestBase
    {
        [TestMethod]
        [TestCategory("GuiTest")]
        public void DataIsLoadedTest()
        {
            RunTestWithScreenShotOnException(WebDriver, driver =>
            {
                var careDocumentationPage = driver.GoToPage<Pages.Example>();
                careDocumentationPage.AssertTextExists("testText");
            });
        }
    }
}
