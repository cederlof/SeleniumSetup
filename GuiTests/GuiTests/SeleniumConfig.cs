using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GuiTests
{
    public static class SeleniumConfig
    {
        private const string ScreenShotPath = @"C:\Temp\";
        public static readonly TimeSpan ImplicitWait = new TimeSpan(0, 0, 0, 10);
        public static readonly TimeSpan NoWait = new TimeSpan(0, 0, 0, 0);

        /// <summary>
        /// Takes a screenshot of the current web-page.
        /// There's a 1 sec delay, to allow the page to fully load.
        /// 
        /// Notice that each driver works different.
        ///		IE: Takes a screenshot of whole page (or atleast visited parts of page).
        ///		Chrome: Takes a screenshot of the currently seen part of the page.
        ///		Safari: Takes a screenshot of the top part of the page.
        /// </summary>
        /// <param name="webDriver"></param>
        public static void SaveScreenShot(ITakesScreenshot webDriver, Exception e = null)
        {
            try
            {
                StackTrace trace = new StackTrace();
                int i = 1;
                var method = trace.GetFrame(i).GetMethod();
                while (method.DeclaringType.Name.StartsWith("Base"))
                {
                    i += 1;
                    method = trace.GetFrame(i).GetMethod();
                }
                string methodName = method.Name;
                string className = method.DeclaringType.Name;

                SaveScreenShot(webDriver, String.Format("{2}_{0}_{1}_", className, methodName, webDriver.GetType().Name), e);
            }
            catch
            {
            }
        }

        private static void SaveScreenShot(ITakesScreenshot webDriver, string screenshotFirstName, Exception e)
        {
            try
            {
                // Wait for a while, so page (and eventual error messages) have time to load and appear on page.
                var screenshot1 = webDriver.GetScreenshot();
                Wait((IWebDriver)webDriver, 1000, 1000);
                var screenshot2 = webDriver.GetScreenshot();
                var baseFileName = new StringBuilder(ScreenShotPath);
                baseFileName.Append(screenshotFirstName);
                baseFileName.Append(DateTime.Now.ToString("dd-MM-yyyy HH_mm_ss"));
                var fileName = new StringBuilder(baseFileName.ToString());
                fileName.Append("_1.png");
                screenshot1.SaveAsFile(fileName.ToString(), ImageFormat.Png);
                fileName = new StringBuilder(baseFileName.ToString());
                fileName.Append("_2.png");
                screenshot2.SaveAsFile(fileName.ToString(), ImageFormat.Png);
                if (e != null)
                {
                    fileName = new StringBuilder(baseFileName.ToString());
                    fileName.Append(".txt");
                    var exceptionContent = GetExceptionAsString(0, e);
                    File.WriteAllText(fileName.ToString(), exceptionContent);
                }
            }
            catch
            {
            }
        }

        private static string GetExceptionAsString(int tabIndex, Exception e)
        {
            if (e == null)
            {
                return "";
            }
            string tabs = "";
            while (tabIndex > 0)
            {
                tabs += "\t";
                tabIndex--;
            }

            return String.Format("{0}Exception:{5}{1}{6}{0}Message:{5}{2}{6}{0}StackTrace:{6}{0}{3}{6}{6}{4}", tabs, e.GetType(), e.Message, e.StackTrace.Replace(Environment.NewLine, Environment.NewLine + tabs), GetExceptionAsString(tabIndex + 1, e.InnerException), "\t", Environment.NewLine);
        }

        private static void Wait(IWebDriver driver, double delay, double interval)
        {
            // Causes the WebDriver to wait for at least a fixed delay
            var now = DateTime.Now;
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(delay));
            wait.PollingInterval = TimeSpan.FromMilliseconds(interval);
            wait.Until(wd => (DateTime.Now - now) - TimeSpan.FromMilliseconds(delay) > TimeSpan.Zero);
        }
    }
}
