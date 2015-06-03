using System;
using GuiTests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;

namespace GuiTests
{
    /// <summary>
    /// Base class for testing pages with Selenium
    /// </summary>
    public abstract class TestBase
    {
        public static IWebDriver WebDriver;

        public TestContext TestContext { get; set; }

        /// <summary>
        /// Initializes the tests.
        /// 
        /// Requires the test method to be decorated with <see cref="DataSourceAttribute"/> to find which drivers the test should be executed with.
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            var driver = "Chrome";
            var driverPath = ""; //todo add path
            try
            {
                switch (driver)
                {
                    case "Chrome":
                        WebDriver = new ChromeDriver(driverPath);
                        break;
                    case "IE":
                        WebDriver = new InternetExplorerDriver(driverPath);
                        break;
                    case "Safari":
                        WebDriver = new SafariDriver();
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Drivertype '{0}' is currently not supported.", driver));
                }
            }
            catch (WebDriverException ex)
            {
                throw new WebDriverException(string.Format("There's a problem initalizing driver '{0}'.", driver), ex);
            }

            WebDriver.Manage().Timeouts().ImplicitlyWait(SeleniumConfig.ImplicitWait);
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            try
            {
                WebDriver.Close();
                WebDriver.Quit();
            }
            catch
            {
            }
        }

        protected delegate void TestImplDelegate(IWebDriver driver);
        protected void RunTestWithScreenShotOnException(IWebDriver driver, TestImplDelegate testImpl, bool performLogin = false, bool waitForLoad = false)
        {
            try
            {
                if (performLogin)
                {
                    //driver.Login(); todo: perform the common denominators for all tests here, for example logging in.
                }
                if (waitForLoad)
                {
                    driver.WaitForPageToLoad();
                }
                testImpl(driver);
            }
            catch (Exception e)
            {
                SeleniumConfig.SaveScreenShot((ITakesScreenshot)driver, e);
                throw;
            }
        }

    }
}