using System.Linq;
using OpenQA.Selenium;

namespace GuiTests.Extensions
{
    public static class WebElementExtensions
    {
        public static bool ElementIsPresent(this IWebElement element, IWebDriver driver, By by)
        {
            var present = false;
            driver.Manage().Timeouts().ImplicitlyWait(SeleniumConfig.NoWait);
            try
            {
                present = element.FindElement(by).Displayed;
            }
            catch (NoSuchElementException)
            {
            }
            driver.Manage().Timeouts().ImplicitlyWait(SeleniumConfig.ImplicitWait);
            return present;
        }

        public static bool HasCssClass(this IWebElement element, string cssClass)
        {
            var classes = new string[0];
            var isOk = false;
            while (!isOk)
            {
                try
                {
                    classes = element.GetAttribute("class").Split(' ');
                    isOk = true;
                }
                catch (StaleElementReferenceException)
                {
                }
            }
            return classes.Any(x => x == cssClass);
        }
    }
}