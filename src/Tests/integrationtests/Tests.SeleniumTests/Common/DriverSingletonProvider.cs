﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Tests.SeleniumTests.Common
{
    internal static class DriverSingletonProvider
    {
        private static IWebDriver driver;

        public static IWebDriver GetDriver()
        {
            return driver;
        }

        public static void Dispose()
        {
            if (driver == null)
            {
                return;
            }

            driver.Dispose();
        }

        static DriverSingletonProvider()
        {
            DriverSingletonProvider.driver = GetNewDriverInstance();
        }

        private static IWebDriver GetNewDriverInstance()
        {
            var options = new ChromeOptions
            {
                UnhandledPromptBehavior = UnhandledPromptBehavior.Dismiss,
                AcceptInsecureCertificates = true,
            };

            return new ChromeDriver(options);
        }
    }
}