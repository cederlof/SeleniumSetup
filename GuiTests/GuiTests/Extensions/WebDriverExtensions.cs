using System;
using System.Linq;
using GuiTests.Attributes;
using GuiTests.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;

namespace GuiTests.Extensions
{
    public static class WebDriverExtensions
    {
        public static int CountElements(this IWebDriver driver, By by)
        {
            var count = 0;
            driver.Manage().Timeouts().ImplicitlyWait(SeleniumConfig.NoWait);
            try
            {
                count = driver.FindElements(@by).Count;
            }
            catch (NoSuchElementException)
            {
            }
            driver.Manage().Timeouts().ImplicitlyWait(SeleniumConfig.ImplicitWait);
            return count;
        }

        public static int CountElements(this IWebDriver driver, IWebElement element, By by)
        {
            var count = 0;
            driver.Manage().Timeouts().ImplicitlyWait(SeleniumConfig.NoWait);
            try
            {
                count = element.FindElements(@by).Count;
            }
            catch (NoSuchElementException)
            {
            }
            driver.Manage().Timeouts().ImplicitlyWait(SeleniumConfig.ImplicitWait);
            return count;
        }

        /// <summary>
        /// Checks if an element is present (e.g. in DOM structure AND visible).
        /// </summary>
        /// <param name="driver">The webdriver</param>
        /// <param name="by">Critieria for element to search for</param>
        /// <param name="waitSec">Max number of seconds to wait for element to appear in DOM structure. Default is 0.</param>
        /// <returns></returns>
        public static bool ElementIsPresent(this IWebDriver driver, By by, int waitSec = 0)
        {
            var present = false;
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(waitSec));

            var isOk = false;
            while (!isOk)
            {
                try
                {
                    present = driver.FindElement(@by).Displayed;
                    isOk = true;
                }
                catch (NoSuchElementException)
                {
                    isOk = true;
                }
                catch (StaleElementReferenceException)
                {
                }
            }
            driver.Manage().Timeouts().ImplicitlyWait(SeleniumConfig.ImplicitWait);
            return present;
        }

        /// <summary>
        /// Tries to locate an element, if it doesn't exist it can wait for it to appear.
        /// </summary>
        /// <param name="driver">The webdriver</param>
        /// <param name="by">Critiria for element to search for</param>
        /// <param name="waitSec">Max number of seconds to wait for element to appear. If not applied, the current default value for the driver is used.</param>
        /// <returns></returns>
        public static IWebElement FindElement(this IWebDriver driver, By by, int? waitSec = null)
        {
            if (waitSec.HasValue)
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(waitSec.Value));
            }
            IWebElement element;
            try
            {
                element = driver.FindElement(@by);
            }
            finally
            {
                if (waitSec.HasValue)
                {
                    driver.Manage().Timeouts().ImplicitlyWait(SeleniumConfig.ImplicitWait);
                }
            }
            return element;
        }

        /// <summary>
        /// Method for navigating to a page.
        /// </summary>
        /// <typeparam name="T">Type of page to navigate to</typeparam>
        /// <param name="driver">Selenium WebDriver</param>
        /// <param name="testCase">Name of the testcase/pagetype to go to.</param>
        /// <param name="waitLoadCompleteBy"></param>
        /// <returns></returns>
        public static T GoToPage<T>(this IWebDriver driver, string testCase = null, By waitLoadCompleteBy = null) where T : PageBase
        {
            var urlAttributes = Attribute.GetCustomAttributes(typeof(T), typeof(PageUrlAttribute), false).Cast<PageUrlAttribute>().ToList();
            if (!urlAttributes.Any())
            {
                throw new Exception(String.Format("Type '{0}' has no attribute of type '{1}'.", typeof(T), typeof(PageUrlAttribute)));
            }
            var myAttribute = urlAttributes.SingleOrDefault(x => x.TestCase == testCase);
            if (myAttribute == null)
            {
                myAttribute = urlAttributes.SingleOrDefault(x => x.Default);
                if (myAttribute == null)
                {
                    throw new Exception(String.Format("Type '{0}' has no attribute of type '{2}' with TestCase='{1}'.", typeof(T), testCase, typeof(PageUrlAttribute)));
                }
            }

            var baseType = typeof(T).BaseType;
            if (baseType == null)
            {
                throw new Exception(string.Format("No base type on type '{0}'.", typeof(T)));
            }
            var baseUrlAttributes = (PageUrlAttribute)Attribute.GetCustomAttribute(baseType, typeof(PageUrlAttribute));
            if (baseUrlAttributes == null)
            {
                throw new Exception(string.Format("Expected base class '{0}' to have an attribute of type '{1}'.", baseType, typeof(PageUrlAttribute)));
            }

            var baseUri = new Uri(baseUrlAttributes.PartUrl);
            if (myAttribute.PartUrl.StartsWith("http"))
            {
                driver.Navigate().GoToUrl(myAttribute.PartUrl);
            }
            else
            {
                var uri = new Uri(baseUri, myAttribute.PartUrl);
                driver.Navigate().GoToUrl(uri);
            }
            driver.WaitForPageToLoad(waitLoadCompleteBy);
            var page = (T)Activator.CreateInstance(typeof(T), driver);
            return page;
        }

        /// <summary>
        /// Waits for the page to load fully. 
        /// 
        /// Method waits for an element (default id "jpnHeaderContainer") to exist. If it hasn't appeared after the defined time (default 20 seconds), it will timeout.
        /// If wanted, it's possible to look for the specified element within another element.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by">What to look for. If null it will use By.Id("jpnHeaderContainer") (default)</param>
        /// <param name="element">Element in which to search for the new element in. "null" if whole page should be searched (default).</param>
        /// <param name="maxWaitSeconds">Number of seconds before timeout (default 20sec)</param>
        public static void WaitForPageToLoad(this IWebDriver driver, By by = null, IWebElement element = null, int maxWaitSeconds = 20)
        {
            if (@by == null)
            {
                //@by = By.Id("jpnHeaderContainer");
                @by = By.TagName("footer");
            }
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(maxWaitSeconds));
            if (element == null)
            {
                wait.Until(x => driver.ElementIsPresent(@by) || driver.ElementIsPresent(By.Id("main-error")));
            }
            else
            {
                wait.Until(x => element.ElementIsPresent(driver, @by) || driver.ElementIsPresent(By.Id("main-error")));
            }
            if (driver.ElementIsPresent(By.Id("main-error")))
            {
                throw new Exception("An error is visible on page.");
            }
        }

        public static void WaitForElementToDisappear(this IWebDriver driver, By by, int maxWaitSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(maxWaitSeconds));
            wait.Until(x => !driver.ElementIsPresent(@by));
        }

        /// <summary>
        /// Scrolls to the bottom of the page using javascript
        /// </summary>
        public static void ScrollToBottom(this IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight - 10);");
        }

        /// <summary>
        /// Scrolls to the top of the page using javascript
        /// </summary>
        public static void ScrollToTop(this IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 0);");
        }

        public static void PerformTabToNextControl(this IWebDriver driver)
        {
            if (driver is SafariDriver)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("var activeElement = document.activeElement; " +
                                                            "var tabableElements = $(':input, a'); " +
                                                            "var currentIndex = tabableElements.index(activeElement); " +
                                                            "if(currentIndex >= 0) { " +
                                                            "	var nextElement = tabableElements.get(currentIndex + 1); " +
                                                            "	if(nextElement) { nextElement.focus(); }" +
                                                            "}");
            }
            else
            {
                var actions = new Actions(driver);
                actions.SendKeys(Keys.Tab).Perform();
            }
        }

        public static void PerformReturnKeyStroke(this IWebDriver driver)
        {
            var actions = new Actions(driver);
            actions.SendKeys(Keys.Return).Perform();
        }

        public static void PerformClickOnControl(this IWebDriver driver, By by)
        {
            driver.PerformClickOnControl(driver.FindElement(by));
        }


        public static void PerformClickOnControl(this IWebDriver driver, IWebElement element)
        {
            //if (driver is InternetExplorerDriver)
            //    driver.SwitchTo().Window(driver.CurrentWindowHandle);

            if (driver is SafariDriver)
            {
                var id = element.GetAttribute("id");
                if (String.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("Element does not have an Id, not possible to perform click.");
                }
                ((IJavaScriptExecutor)driver).ExecuteScript("$('#" + id + "').click();");
            }
            else
            {
                if (element.TagName == "a" && !element.HasCssClass("btn"))
                {
                    element.Click();
                }
                else
                {
                    var actions = new Actions(driver);
                    actions.Click(element).Perform();
                }
            }
        }

        public static void SetTextToTextbox(this IWebDriver driver, IWebElement element, string text)
        {
            var id = element.GetAttribute("id");
            if (driver is SafariDriver)
            {
                if (!driver.ElementIsPresent(By.Id(id)))
                {
                    throw new Exception("Element is not present!");
                }
                ((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById('" + id + "').value = '" + text + "';");
            }
            else
            {
                var actions = new Actions(driver);
                actions.SendKeys(element, text).Perform();
            }
        }
    }
}