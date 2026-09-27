using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Locators_for_Web_Elements
{
    [TestFixture]
    public class Tests
    {

        private IWebDriver _driver = null!;
        private WebDriverWait _wait = null!;

        private string WebsiteUrl
        {
            get
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                return configuration["WebsiteUrl"]!;
            }
        }

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new();
            this._driver = new ChromeDriver(options);
            this._wait = new WebDriverWait(this._driver, TimeSpan.FromSeconds(10));

            this._driver.Manage().Window.Maximize();
        }

        [Test]
        [TestCase("Python", "Armenia")]
        public void Test1(string language, string country)
        {
            this._driver.Navigate().GoToUrl(this.WebsiteUrl);

            IWebElement careersLink = this._driver.FindElement(
    By.LinkText("Careers"));

            careersLink.Click();

            //Thread.Sleep(3000);

            IWebElement acceptCookies = this._wait.Until(
        driver => driver.FindElement(
            By.Id("onetrust-accept-btn-handler")));

            acceptCookies.Click();

            IWebElement searchLink = this._wait.Until(
                 driver => driver.FindElement(
                    By.CssSelector("a.button-body")));

            searchLink.Click();
            // 

            IWebElement searchField = this._wait.Until(
    driver => driver.FindElement(
        By.Name("search")));

            searchField.Clear();
            searchField.SendKeys(language);

             acceptCookies = this._wait.Until(
    driver => driver.FindElement(
        By.Id("onetrust-accept-btn-handler")));

            acceptCookies.Click();

            IWebElement countryField = this._wait.Until(
    driver => driver.FindElement(
        By.CssSelector("input[aria-label='Choose your country']")));

            countryField.Click();
            countryField.SendKeys(country);

            IWebElement remoteCheckbox = this._wait.Until(
    driver => driver.FindElement(
        By.XPath("//label[.//span[normalize-space()='Remote']]")));

            remoteCheckbox.Click();

            //        acceptCookies = this._wait.Until(
            //driver => driver.FindElement(
            //    By.Id("onetrust-accept-btn-handler")));

            //        acceptCookies.Click();

            TestContext.Progress.WriteLine(
    $"Current URL: {this._driver.Url}");

            TestContext.Progress.WriteLine(
                $"Page title: {this._driver.Title}");
Thread.Sleep(5000);

            IWebElement overlay = this._driver.FindElement(
    By.CssSelector("div.StickySearchBox_overlay___j_z8"));

            this._wait.Until(
                driver => !overlay.Displayed);

            IWebElement searchButton = this._wait.Until(
    driver => driver.FindElement(
        By.CssSelector("button[type='submit']")));

            this._wait.Until(
                driver => searchButton.Displayed && searchButton.Enabled);


            searchButton.Click();

            IReadOnlyCollection<IWebElement> jobCards = this._wait.Until(
    driver => driver.FindElements(
        By.CssSelector("[data-testid='job-card-link']")));

            //        IWebElement lastPageButton = this._wait.Until(
            //driver => driver.FindElement(
            //   By.CssSelector("button[aria-label='next page']")));

            //  lastPageButton.Click();

            IWebElement pagination = this._wait.Until(
    driver => driver.FindElement(
        By.CssSelector("[data-testid='pagination']")));

            IWebElement nextPageButton = pagination.FindElement(
    By.CssSelector("button[aria-label='next page']"));

            ((IJavaScriptExecutor)this._driver).ExecuteScript(
    "arguments[0].scrollIntoView({block: 'center'});",
    nextPageButton);
            ((IJavaScriptExecutor)this._driver).ExecuteScript(
    "arguments[0].click();",
    nextPageButton);
            // nextPageButton.Click();
            IWebElement lastJob = jobCards.Last();

           // lastJob.Click();

            Thread.Sleep(5000);
        }

        [TearDown]
        public void TearDown()
        {
            this._driver.Quit();
            this._driver.Dispose();
        }
    }
}
