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
        private IWebDriver _driver;
        private DelLogger _logger;
        public Companies(DelLogger logger)
        {

            //Initialize the instance
            _driver = new ChromeDriver();
            _logger = logger;
        }


        public List<string> GetCompanies(int pageAmount)
        {
            string url = "https://www.europages.de/unternehmen/Produktion.html";

            var companyList = new List<string>();

            try
            {
                for (int i = 1; i < 1000; i++)
                {
                    if (i > 1)
                    {
                        companyList.AddRange(GetCompaniesFromPage($"https://www.europages.de/unternehmen/pg-{i}/Produktion.html"));
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
            //launch gmail.com
            _driver.Navigate().GoToUrl(url);

            //maximize the browser
            _driver.Manage().Window.Minimize();

            //find the element by xpath and enter the email address which you want to login.
            //driver.FindElement(By.XPath("//input[@aria-label='Email or phone']")).SendKeys("email adress);

            //wait for a seconds
            Task.Delay(1000).Wait();

            //find the Next Button and click on it.

            var all = _driver.FindElements(By.XPath("//a[contains(@class,'company-name')]"));

            List<string> lst = new List<string>();
            foreach (var a in all)
            {
                lst.Add(a.Text);
            }

            return lst;
        }

    }
}
