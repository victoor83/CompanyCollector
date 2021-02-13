using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace CompanyCollector
{
    public class Companies
    {
        private ChromeOptions _options;
        private IWebDriver _driver;
        private DelLogger _logger;
        public Companies(DelLogger logger)
        {
            //Initialize options for Driver
            _options = new ChromeOptions();
            _options.AddArguments("headless","--blink-settings=imagesEnabled=false");

            //Initialize the instance
            _driver = new ChromeDriver(_options);
            _logger = logger;
        }


        public List<string> GetCompanies(int pageAmount)
        {
            string url = "https://www.europages.de/unternehmen/Deutschland/fenster.html";

            var companyList = new List<string>();

            try
            {
                for (int i = 0; i <= pageAmount; i++)
                {
                    if (i > 0)
                    {
                        companyList.AddRange(GetCompaniesFromPage($"https://www.europages.de/unternehmen/pg-{i}/Deutschland/fenster.html"));
                    }
                    else
                    {
                        companyList.AddRange(GetCompaniesFromPage(url));
                    }

                    _logger("Proceed page nr:" + i);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                //close the driver instance.
                _driver.Close();

                //quit
                _driver.Quit();
            }

            return companyList;

        }

        private List<string> GetCompaniesFromPage(string url)
        {
            _driver.Navigate().GoToUrl(url);

            //wait for 0.1 seconds
            Task.Delay(100).Wait();

            
            var pages = _driver.FindElements(By.XPath("//a[contains(@class,'company-name')]"));

             List<string> lst = new List<string>();

            foreach(var a in pages)
            {
                //Collecting cata of all companies from single page
                var temp = _driver.CurrentWindowHandle;
                string link = a.GetAttribute("href");
                ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
                _driver.SwitchTo().Window(_driver.WindowHandles[1]);
                _driver.Navigate().GoToUrl(link);
                Task.Delay(200).Wait();
                lst.Add(string.Join(",", GetDataPerCompany())); //Compiling data of a single company into one string
                _driver.Close();
                _driver.SwitchTo().Window(_driver.WindowHandles[0]);
                Task.Delay(200).Wait();
            }          

            return lst;
        }

        private List<string> GetDataPerCompany()
        {
            //Collecting data of a single company
            List<string> line = new List<string>();
            line.Add(_driver.FindElement(By.XPath("//div[contains(@class, 'company-content')]/h3[contains(@itemprop, 'name')]")).GetAttribute("innerText").Replace(",","").Replace("\r","").Replace("\n"," "));
            _logger(line[0]);
            line.Add(_driver.FindElement(By.XPath("//dd[contains(@class, 'company-country')]/span[2]")).GetAttribute("innerText").Replace(",","").Replace("\r","").Replace("\n"," "));
            line.Add(_driver.FindElement(By.XPath("//dd[contains(@itemprop, 'addressLocality')]/pre")).GetAttribute("innerText").Replace(",","").Replace("\r","").Replace("\n"," "));
            try
            {
                line.Add(_driver.FindElement(By.XPath("//div[contains(@class, 'page__layout-sidebar--container-desktop')]/a[contains(@itemprop, 'url')]")).GetAttribute("href"));
            }
            catch (OpenQA.Selenium.NoSuchElementException)
            {
                
                line.Add("This company has no website.");
            }
            
            line.Add(_driver.FindElement(By.XPath("//p[contains(@class, 'company-description')]")).GetAttribute("innerText").Replace(",","").Replace("\r","").Replace("\n"," "));
            return line;
        }

    }
}
