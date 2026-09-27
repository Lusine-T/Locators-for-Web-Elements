using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Xml.Xsl;

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
        [TestCase("Java", "Armenia")]
        public void Test1(string language, string country)
        {
            this._driver.Navigate().GoToUrl(this.WebsiteUrl);

            IWebElement careersLink = this._driver.FindElement(By.LinkText("Careers"));

            careersLink.Click();

            Thread.Sleep(3000);

            IWebElement acceptCookies = this._wait.Until(
                driver => driver.FindElement(By.Id("onetrust-accept-btn-handler")));

            acceptCookies.Click();

            IWebElement searchLink = this._wait.Until(driver => driver.FindElement(
                    By.CssSelector("a.button-body")));

            searchLink.Click();
            //
            Thread.Sleep(3000);
            acceptCookies = this._wait.Until(driver => driver.FindElement(By.Id("onetrust-accept-btn-handler")));
            acceptCookies.Click();
            Thread.Sleep(1000);

            IWebElement languageField = this._wait.Until(
                driver => driver.FindElement(By.Name("search")));

            languageField.Clear();
            languageField.SendKeys(language);

            IWebElement countryField = this._wait.Until(
                driver => driver.FindElement(
                    By.CssSelector("input[aria-label='Choose your country']")));

            countryField.Click();
            countryField.SendKeys(country);

            IWebElement remoteCheckbox = this._wait.Until(
                driver => driver.FindElement(
                    By.XPath("//label[.//span[normalize-space()='Remote']]")));

            remoteCheckbox.Click();

            Thread.Sleep(3000);

            IWebElement overlay = this._driver.FindElement(
                By.CssSelector("div.StickySearchBox_overlay___j_z8"));

            this._wait.Until(driver => !overlay.Displayed);

            IWebElement searchButton = this._wait.Until(
                driver => driver.FindElement(By.CssSelector("button[type='submit']")));

            Thread.Sleep(3000);
            
            this._wait.Until(driver => searchButton.Displayed && searchButton.Enabled);
            searchButton.Click();

            Thread.Sleep(1000);

            while (true)
            {
                IWebElement nextPageButton = this._wait.Until(
                    driver => driver.FindElement(
                        By.CssSelector(
                            "[data-testid='pagination'] button[aria-label='next page']")));

                string? disabled = nextPageButton.GetAttribute("disabled");

                if (disabled != null)
                {
                    break;
                }

                ((IJavaScriptExecutor)this._driver).ExecuteScript(
                    "arguments[0].click();",
                    nextPageButton);

                this._wait.Until(driver =>
                    driver.FindElement(
                        By.CssSelector("[data-testid='pagination']")));
            }
            Thread.Sleep(1000);

            IReadOnlyCollection<IWebElement> jobCards = this._wait.Until(
                driver => driver.FindElements(
                    By.CssSelector("[data-testid='job-card-link']")));

            

            //IReadOnlyCollection<IWebElement> jobCards = this._wait.Until(
            //    driver => driver.FindElements(
            //        By.CssSelector("[data-testid='job-card-link']")));

            IWebElement lastJob = jobCards.Last();
            lastJob.Click();
            Thread.Sleep(1000);
           

            string resultsUrl = this._driver.Url;
            this._wait.Until(driver =>
            {
                IReadOnlyCollection<IWebElement> preloaders =
                    driver.FindElements(
                        By.CssSelector("div.Preloader_fullSize__jIIky"));

                return preloaders.Count == 0 ||
                       preloaders.All(preloader => !preloader.Displayed);
            });

            
            Thread.Sleep(1000);
                        
            this._wait.Until(driver => driver.FindElement(By.TagName("body")).Displayed);

            string pageText = this._driver.FindElement(
                By.TagName("body")).Text;

            Assert.That(
                pageText,
                Does.Contain(language));


            Thread.Sleep(8000);
        }

        [Test]
        [TestCase("BLOCKCHAIN")]
        [TestCase("Cloud")]
        [TestCase("Automation")]
        public void GlobalSearch_ValidKeyword_SearchResultsContainKeyword(string searchKeyword)
        {
            this._driver.Navigate().GoToUrl(this.WebsiteUrl);

            IWebElement searchButton = this._driver.FindElement(
                By.CssSelector("button.header-search__button"));

            searchButton.Click();

            IWebElement searchInput = this._wait.Until(
    driver =>
    {
        IWebElement element = driver.FindElement(
            By.Id("new_form_search"));

        return element.Displayed && element.Enabled
            ? element
            : null;
    });

            searchInput.Clear();
            searchInput.SendKeys(searchKeyword);

             IWebElement acceptCookies = this._wait.Until(
                driver => driver.FindElement(By.Id("onetrust-accept-btn-handler")));

            acceptCookies.Click();

            IWebElement findButton = this._driver.FindElement(
                By.CssSelector("button.custom-search-button"));

            findButton.Click();   
            
            Thread.Sleep(5000);         

            this._wait.Until(driver => driver.FindElement(
                By.CssSelector("article.search-results__item")));

            //a.search-results__title-link - by the task the search string should be
            //only in the title (h3), but indeed the string is also in the search-results__description
            //and even in the contetnt of the link


            // Get all result links
            IReadOnlyCollection<IWebElement> links = this._driver.FindElements(
                By.CssSelector("article.search-results__item"));

            foreach (IWebElement link in links)
            {
                TestContext.WriteLine($"RESULT: '{link.Text}'");
            }

            bool allLinksContainKeyword = links.All(link => link.Text.Contains(
                searchKeyword,
                StringComparison.OrdinalIgnoreCase));

            Assert.That(allLinksContainKeyword, Is.True);

           // Thread.Sleep(5000);
        }


        [TearDown]
        public void TearDown()
        {
            this._driver.Quit();
            this._driver.Dispose();
        }
    }
}
